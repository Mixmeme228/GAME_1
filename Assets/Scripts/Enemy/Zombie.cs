using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class ZombieAI : MonoBehaviour
{
    // ─── Движение ──────────────────────────────────────────────────────────
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float stopDistance = 0.4f;

    // ─── Обнаружение ───────────────────────────────────────────────────────
    [Header("Detection")]
    public float activationRadius = 5f;
    public float chaseDistance = 15f;
    public float attackRadius = 0.8f;

    // ─── Атака ─────────────────────────────────────────────────────────────
    [Header("Attack")]
    public int attackDamage = 1;
    public float attackCooldown = 1.2f;

    // ─── HP ────────────────────────────────────────────────────────────────
    [Header("Health")]
    public float maxHealth = 100f;
    [Tooltip("Слайдер HP-бара (необязательно)")]
    public Slider healthBarSlider;
    [Tooltip("Эффект при получении урона (необязательно)")]
    public GameObject hitFX;
    [Tooltip("Эффект при смерти (необязательно)")]
    public GameObject deathFX;

    public float CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    [Header("AI")]
    [Tooltip("Выключи чтобы заморозить зомби")]
    public bool aiEnabled = true;

    // ─── Патруль ───────────────────────────────────────────────────────────
    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 1.5f;

    // ─── Состояния ─────────────────────────────────────────────────────────
    public enum ZombieState { Idle, Activating, Patrolling, Chasing, Attacking, Returning, Dead }
    public ZombieState currentState = ZombieState.Idle;

    // ─── Приватные ─────────────────────────────────────────────────────────
    private NavMeshAgent _agent;
    private Animator _anim;
    private Transform _player;
    private SoundHandlerEnemy _sfx;

    private static readonly int H_TakeHit = Animator.StringToHash("TakeHit");

    private Vector3 _startPosition;
    private float _lastAttackTime;
    private int _patrolIndex = 0;
    private float _patrolTimer = 0f;
    private bool _waitingAtPoint = false;

    // ─── Animator hashes ───────────────────────────────────────────────────
    private static readonly int H_Idle = Animator.StringToHash("Idle_zom");
    private static readonly int H_Up_w = Animator.StringToHash("Up_w");
    private static readonly int H_Down_w = Animator.StringToHash("Down_w");
    private static readonly int H_Left_w = Animator.StringToHash("Left_w");
    private static readonly int H_Right_w = Animator.StringToHash("Right_w");
    private static readonly int H_Up_a = Animator.StringToHash("Up_a");
    private static readonly int H_Down_a = Animator.StringToHash("Down_a");
    private static readonly int H_Left_a = Animator.StringToHash("Left_a");
    private static readonly int H_Right_a = Animator.StringToHash("Right_a");
    private static readonly int H_Death = Animator.StringToHash("Death_2");

    // ══════════════════════════════════════════════════════════════════════
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        _sfx = GetComponent<SoundHandlerEnemy>();

        var p = GameObject.FindGameObjectWithTag("Player_1");
        if (p) _player = p.transform;

        _startPosition = transform.position;

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.speed = moveSpeed;
        _agent.stoppingDistance = stopDistance;

        CurrentHealth = maxHealth;
        IsDead = false;

        SetState(ZombieState.Idle);
    }

    // ══════════════════════════════════════════════════════════════════════
    void Update()
    {
        if (currentState == ZombieState.Dead || _player == null || !aiEnabled) return;

        float dist = Vector2.Distance(transform.position, _player.position);

        switch (currentState)
        {
            case ZombieState.Idle: HandleIdle(dist); break;
            case ZombieState.Activating: break;
            case ZombieState.Patrolling: HandlePatrol(dist); break;
            case ZombieState.Chasing: HandleChase(dist); break;
            case ZombieState.Attacking: HandleAttack(dist); break;
            case ZombieState.Returning: HandleReturn(dist); break;
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    #region State Handlers

    void HandleIdle(float dist)
    {
        if (dist <= activationRadius)
            SetState(ZombieState.Activating);
        else if (patrolPoints != null && patrolPoints.Length > 0)
            SetState(ZombieState.Patrolling);
    }

    void HandlePatrol(float dist)
    {
        if (dist <= activationRadius) { SetState(ZombieState.Chasing); return; }
        if (patrolPoints == null || patrolPoints.Length == 0) { SetState(ZombieState.Idle); return; }

        if (_waitingAtPoint)
        {
            _agent.isStopped = true;
            SetMoveAnim(Vector2.zero);
            _patrolTimer -= Time.deltaTime;
            if (_patrolTimer <= 0f) { _waitingAtPoint = false; _patrolIndex = (_patrolIndex + 1) % patrolPoints.Length; }
            return;
        }

        Transform target = patrolPoints[_patrolIndex];
        if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            _waitingAtPoint = true;
            _patrolTimer = patrolWaitTime;
        }
        else
        {
            _agent.isStopped = false;
            _agent.SetDestination(target.position);
            SetMoveAnim(Get4Dir(target.position));
        }
    }

    void HandleChase(float dist)
    {
        if (dist > chaseDistance) { SetState(ZombieState.Returning); return; }
        if (dist <= attackRadius) { SetState(ZombieState.Attacking); return; }

        _agent.isStopped = false;
        _agent.SetDestination(_player.position);
        SetMoveAnim(Get4Dir(_player.position));
    }

    void HandleAttack(float dist)
    {
        _agent.isStopped = true;
        SetAttackAnim(Get4Dir(_player.position));

        if (dist > attackRadius * 1.5f) { SetState(ZombieState.Chasing); return; }

        if (Time.time >= _lastAttackTime + attackCooldown)
        {
            _lastAttackTime = Time.time;
            _player.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
        }
    }

    void HandleReturn(float dist)
    {
        if (dist <= activationRadius) { SetState(ZombieState.Chasing); return; }

        if (Vector2.Distance(transform.position, _startPosition) > 0.2f)
        {
            _agent.isStopped = false;
            _agent.SetDestination(_startPosition);
            SetMoveAnim(Get4Dir(_startPosition));
        }
        else
        {
            _agent.isStopped = true;
            SetState(ZombieState.Idle);
        }
    }

    #endregion

    // ══════════════════════════════════════════════════════════════════════
    #region HP System

    /// <summary>Нанести урон зомби. Вызывай из пуль, ловушек и т.д.</summary>
    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        RefreshHealthBar();

        if (hitFX != null)
            Instantiate(hitFX, transform.position, Quaternion.identity);

        _sfx?.PlayHitSound();

        if (_anim.HasState(0, H_TakeHit))
            _anim.SetTrigger(H_TakeHit);

        if (CurrentHealth <= 0f) Die();
    }

    /// <summary>Убить зомби немедленно.</summary>
    public void Die()
    {
        if (IsDead) return;
        IsDead = true;
        CurrentHealth = 0f;
        RefreshHealthBar();

        if (deathFX != null)
            Instantiate(deathFX, transform.position, Quaternion.identity);

        SetState(ZombieState.Dead);
        Destroy(gameObject, 1f);
    }

    /// <summary>Лечение.</summary>
    public void Heal(float amount)
    {
        if (IsDead) return;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        RefreshHealthBar();
    }

    void RefreshHealthBar()
    {
        if (healthBarSlider) healthBarSlider.value = CurrentHealth / maxHealth;
    }

    public float HealthPercent => CurrentHealth / maxHealth;

    #endregion

    // ══════════════════════════════════════════════════════════════════════
    #region State Machine

    public void SetState(ZombieState newState)
    {
        currentState = newState;
        ResetAnimFlags();

        switch (newState)
        {
            case ZombieState.Idle:
                _agent.isStopped = true;
                _anim.SetBool(H_Idle, true);
                break;

            case ZombieState.Activating:
                _agent.isStopped = true;
                Invoke(nameof(OnActivationComplete), 0.5f);
                break;

            case ZombieState.Dead:
                _agent.isStopped = true;
                _anim.SetTrigger(H_Death);
                GetComponent<Collider2D>().enabled = false;
                enabled = false;
                break;
        }
    }

    void OnActivationComplete()
    {
        if (currentState == ZombieState.Activating)
            SetState(ZombieState.Chasing);
    }

    #endregion

    // ══════════════════════════════════════════════════════════════════════
    #region Animation Helpers

    Vector2 Get4Dir(Vector3 target)
    {
        Vector2 d = target - transform.position;
        return Mathf.Abs(d.x) >= Mathf.Abs(d.y)
            ? (d.x > 0 ? Vector2.right : Vector2.left)
            : (d.y > 0 ? Vector2.up : Vector2.down);
    }

    void SetMoveAnim(Vector2 dir)
    {
        ResetAnimFlags();
        if (dir == Vector2.zero) _anim.SetBool(H_Idle, true);
        else if (dir == Vector2.up) _anim.SetBool(H_Up_w, true);
        else if (dir == Vector2.down) _anim.SetBool(H_Down_w, true);
        else if (dir == Vector2.left) _anim.SetBool(H_Left_w, true);
        else if (dir == Vector2.right) _anim.SetBool(H_Right_w, true);
    }

    void SetAttackAnim(Vector2 dir)
    {
        ResetAnimFlags();
        if (dir == Vector2.up) _anim.SetBool(H_Up_a, true);
        else if (dir == Vector2.down) _anim.SetBool(H_Down_a, true);
        else if (dir == Vector2.left) _anim.SetBool(H_Left_a, true);
        else if (dir == Vector2.right) _anim.SetBool(H_Right_a, true);
    }

    void ResetAnimFlags()
    {
        _anim.SetBool(H_Idle, false);
        _anim.SetBool(H_Up_w, false);
        _anim.SetBool(H_Down_w, false);
        _anim.SetBool(H_Left_w, false);
        _anim.SetBool(H_Right_w, false);
        _anim.SetBool(H_Up_a, false);
        _anim.SetBool(H_Down_a, false);
        _anim.SetBool(H_Left_a, false);
        _anim.SetBool(H_Right_a, false);
    }

    #endregion

    // ══════════════════════════════════════════════════════════════════════
    #region Gizmos
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 1.2f,
            $"HP: {CurrentHealth:0}/{maxHealth:0}  [{currentState}]");
    }
#endif
    #endregion
}