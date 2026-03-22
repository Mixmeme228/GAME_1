using UnityEngine;

/// <summary>
/// Обычный зомби — патрулирует, преследует игрока, атакует.
/// Параметры аниматора: Up_w, Down_w, Left_w, Right_w, Idle_zom,
///                      Up_a, Down_a, Left_a, Right_a, Death_2
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Zombie : MonoBehaviour
{
    [Header("Характеристики")]
    public float moveSpeed = 2f;
    public float chaseRange = 5f;   // дистанция обнаружения игрока
    public float attackRange = 0.8f; // дистанция атаки
    public float attackCooldown = 1.2f;
    public float attackDamage = 1;
    public int maxHealth = 3;

    [Header("Патруль")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 1.5f;

    [Header("Ссылки")]
    [SerializeField] private string playerTag = "Player";

    // ── Приватные ─────────────────────────────────────────────
    private Rigidbody2D _rb;
    private Animator _anim;
    private Transform _player;

    private float _health;
    private bool _isDead = false;
    private float _attackTimer = 0f;

    private int _patrolIndex = 0;
    private float _patrolTimer = 0f;
    private bool _waitingAtPoint = false;

    private Vector2 _moveDir = Vector2.zero;

    // Аниматор — названия параметров
    private static readonly int P_Up_w = Animator.StringToHash("Up_w");
    private static readonly int P_Down_w = Animator.StringToHash("Down_w");
    private static readonly int P_Left_w = Animator.StringToHash("Left_w");
    private static readonly int P_Right_w = Animator.StringToHash("Right_w");
    private static readonly int P_Idle = Animator.StringToHash("Idle_zom");
    private static readonly int P_Up_a = Animator.StringToHash("Up_a");
    private static readonly int P_Down_a = Animator.StringToHash("Down_a");
    private static readonly int P_Left_a = Animator.StringToHash("Left_a");
    private static readonly int P_Right_a = Animator.StringToHash("Right_a");
    private static readonly int P_Death = Animator.StringToHash("Death_2");

    // =========================================================================
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _health = maxHealth;

        var p = GameObject.FindGameObjectWithTag(playerTag);
        if (p) _player = p.transform;
    }

    void Update()
    {
        if (_isDead) return;

        _attackTimer -= Time.deltaTime;

        float distToPlayer = _player ? Vector2.Distance(transform.position, _player.position) : Mathf.Infinity;

        if (distToPlayer <= attackRange)
        {
            // Атака
            _moveDir = Vector2.zero;
            TryAttack();
        }
        else if (distToPlayer <= chaseRange)
        {
            // Преследование
            _moveDir = (_player.position - transform.position).normalized;
        }
        else
        {
            // Патруль
            Patrol();
        }

        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (_isDead) return;
        _rb.linearVelocity = _moveDir * moveSpeed;
    }

    // ── Патруль ───────────────────────────────────────────────
    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            _moveDir = Vector2.zero;
            return;
        }

        if (_waitingAtPoint)
        {
            _moveDir = Vector2.zero;
            _patrolTimer -= Time.deltaTime;
            if (_patrolTimer <= 0f)
            {
                _waitingAtPoint = false;
                _patrolIndex = (_patrolIndex + 1) % patrolPoints.Length;
            }
            return;
        }

        Transform target = patrolPoints[_patrolIndex];
        Vector2 dir = (target.position - transform.position);
        if (dir.magnitude < 0.15f)
        {
            _waitingAtPoint = true;
            _patrolTimer = patrolWaitTime;
            _moveDir = Vector2.zero;
        }
        else
        {
            _moveDir = dir.normalized;
        }
    }

    // ── Атака ────────────────────────────────────────────────
    private void TryAttack()
    {
        if (_attackTimer > 0f) return;
        _attackTimer = attackCooldown;

        // Направление на игрока для анимации атаки
        if (_player)
        {
            Vector2 dir = (_player.position - transform.position).normalized;
            PlayAttackAnim(dir);

            var playerHealth = _player.GetComponent<PlayerHealth>();
            playerHealth?.TakeDamage(attackDamage);
        }
    }

    // ── Анимация движения ────────────────────────────────────
    private void UpdateAnimation()
    {
        // Сбрасываем все
        _anim.SetBool(P_Up_w, false);
        _anim.SetBool(P_Down_w, false);
        _anim.SetBool(P_Left_w, false);
        _anim.SetBool(P_Right_w, false);
        _anim.SetBool(P_Idle, false);

        if (_moveDir == Vector2.zero)
        {
            _anim.SetBool(P_Idle, true);
            return;
        }

        // Определяем доминирующую ось
        if (Mathf.Abs(_moveDir.x) >= Mathf.Abs(_moveDir.y))
        {
            if (_moveDir.x > 0) _anim.SetBool(P_Right_w, true);
            else _anim.SetBool(P_Left_w, true);
        }
        else
        {
            if (_moveDir.y > 0) _anim.SetBool(P_Up_w, true);
            else _anim.SetBool(P_Down_w, true);
        }
    }

    // ── Анимация атаки ───────────────────────────────────────
    private void PlayAttackAnim(Vector2 dir)
    {
        _anim.SetBool(P_Up_a, false);
        _anim.SetBool(P_Down_a, false);
        _anim.SetBool(P_Left_a, false);
        _anim.SetBool(P_Right_a, false);

        if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
        {
            if (dir.x > 0) _anim.SetBool(P_Right_a, true);
            else _anim.SetBool(P_Left_a, true);
        }
        else
        {
            if (dir.y > 0) _anim.SetBool(P_Up_a, true);
            else _anim.SetBool(P_Down_a, true);
        }
    }

    // ── Урон / смерть ────────────────────────────────────────
    public void TakeDamage(float dmg)
    {
        if (_isDead) return;
        _health -= dmg;
        if (_health <= 0) Die();
    }

    private void Die()
    {
        _isDead = true;
        _rb.linearVelocity = Vector2.zero;

        // Сбрасываем все параметры
        _anim.SetBool(P_Up_w, false);
        _anim.SetBool(P_Down_w, false);
        _anim.SetBool(P_Left_w, false);
        _anim.SetBool(P_Right_w, false);
        _anim.SetBool(P_Idle, false);
        _anim.SetBool(P_Up_a, false);
        _anim.SetBool(P_Down_a, false);
        _anim.SetBool(P_Left_a, false);
        _anim.SetBool(P_Right_a, false);

        _anim.SetTrigger(P_Death);

        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 2f);
    }

    // ── Гизмо для отладки ────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}