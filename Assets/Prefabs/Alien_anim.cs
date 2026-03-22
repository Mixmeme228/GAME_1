using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class AlienAI : MonoBehaviour
{
    // ─── Движение ──────────────────────────────────────────────────────────
    [Header("Movement")]
    public float moveSpeed = 3f;

    // ─── Обнаружение ───────────────────────────────────────────────────────
    [Header("Detection")]
    public float activationRadius = 6f;
    public float chaseDistance = 18f;
    public float attackRadius = 1.5f;

    // ─── Атака ─────────────────────────────────────────────────────────────
    [Header("Attack")]
    public float attackDamage = 2f;          // ← было int, теперь float как у Robot
    public float attackCooldown = 1f;

    // ─── HP ────────────────────────────────────────────────────────────────
    [Header("Health")]
    public float maxHealth = 80f;
    [Tooltip("Слайдер HP-бара (необязательно)")]
    public Slider healthBarSlider;
    [Tooltip("Эффект при получении урона (необязательно)")]
    public GameObject hitFX;
    [Tooltip("Эффект при смерти (необязательно)")]
    public GameObject deathFX;

    public float CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    // ─── AI ────────────────────────────────────────────────────────────────
    [Header("AI")]
    [Tooltip("Выключи чтобы заморозить инопланетянина")]
    public bool aiEnabled = true;

    // ─── Флип спрайта ──────────────────────────────────────────────────────
    [Header("Sprite")]
    [Tooltip("SpriteRenderer инопланетянина (найдётся автоматически если не назначен)")]
    public SpriteRenderer spriteRenderer;
    [Tooltip("true — спрайт по умолчанию смотрит вправо; false — влево")]
    public bool defaultFacingRight = true;

    // ─── Патруль ───────────────────────────────────────────────────────────
    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 1.5f;

    // ─── Состояния ─────────────────────────────────────────────────────────
    public enum AlienState { Idle, Activating, Patrolling, Chasing, Attacking, Returning, Dead }
    public AlienState currentState = AlienState.Idle;

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
    private static readonly int H_Run = Animator.StringToHash("Run");
    private static readonly int H_Attack = Animator.StringToHash("Attack");
    private static readonly int H_Death = Animator.StringToHash("Death");

    // ══════════════════════════════════════════════════════════════════════
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        _sfx = GetComponent<SoundHandlerEnemy>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        var p = GameObject.FindGameObjectWithTag("Player_1");
        if (p) _player = p.transform;

        _startPosition = transform.position;

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.speed = moveSpeed;
        _agent.stoppingDistance = 0f;

        CurrentHealth = maxHealth;
        IsDead = false;
        RefreshHealthBar();          // ← добавлено как у Robot

        SetState(AlienState.Idle);
    }

    // ══════════════════════════════════════════════════════════════════════
    void Update()
    {
        if (currentState == AlienState.Dead || _player == null || !aiEnabled) return;

        float dist = Vector2.Distance(transform.position, _player.position);

        switch (currentState)
        {
            case AlienState.Idle: HandleIdle(dist); break;
            case AlienState.Activating: break;
            case AlienState.Patrolling: HandlePatrol(dist); break;
            case AlienState.Chasing: HandleChase(dist); break;
            case AlienState.Attacking: HandleAttack(dist); break;
            case AlienState.Returning: HandleReturn(dist); break;
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    #region State Handlers

    void HandleIdle(float dist)
    {
        if (dist <= activationRadius)
            SetState(AlienState.Activating);
        else if (patrolPoints != null && patrolPoints.Length > 0)
            SetState(AlienState.Patrolling);
    }

    void HandlePatrol(float dist)
    {
        if (dist <= activationRadius) { SetState(AlienState.Chasing); return; }
        if (patrolPoints == null || patrolPoints.Length == 0) { SetState(AlienState.Idle); return; }

        if (_waitingAtPoint)
        {
            _agent.isStopped = true;
            SetAnim(run: false, attack: false);
            _patrolTimer -= Time.deltaTime;
            if (_patrolTimer <= 0f)
            {
                _waitingAtPoint = false;
                _patrolIndex = (_patrolIndex + 1) % patrolPoints.Length;
            }
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
            FlipTowards(target.position);
            SetAnim(run: true, attack: false);
        }
    }

    void HandleChase(float dist)
    {
        if (dist > chaseDistance) { SetState(AlienState.Returning); return; }
        if (dist <= attackRadius) { SetState(AlienState.Attacking); return; }

        _agent.isStopped = false;
        _agent.SetDestination(_player.position);
        FlipTowards(_player.position);
        SetAnim(run: true, attack: false);
    }

    void HandleAttack(float dist)
    {
        _agent.isStopped = true;
        _agent.ResetPath();
        FlipTowards(_player.position);
        SetAnim(run: false, attack: true);

        if (dist > attackRadius * 1.5f) { SetState(AlienState.Chasing); return; }

        if (Time.time >= _lastAttackTime + attackCooldown)
        {
            _lastAttackTime = Time.time;
            _player.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
        }
    }

    void HandleReturn(float dist)
    {
        if (dist <= activationRadius) { SetState(AlienState.Chasing); return; }

        if (Vector2.Distance(transform.position, _startPosition) > 0.2f)
        {
            _agent.isStopped = false;
            _agent.SetDestination(_startPosition);
            FlipTowards(_startPosition);
            SetAnim(run: true, attack: false);
        }
        else
        {
            _agent.isStopped = true;
            SetState(AlienState.Idle);
        }
    }

    #endregion

    // ══════════════════════════════════════════════════════════════════════
    #region HP System

    /// <summary>Нанести урон инопланетянину. Вызывай из пуль, ловушек и т.д.</summary>
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

        if (CurrentHealth <= 0f)
            Die();                   // ← вызов вынесен отдельно как у Robot
    }

    /// <summary>Убить инопланетянина немедленно.</summary>
    public void Die()
    {
        if (IsDead) return;
        IsDead = true;
        CurrentHealth = 0f;
        RefreshHealthBar();          // ← добавлено как у Robot

        if (deathFX != null)
            Instantiate(deathFX, transform.position, Quaternion.identity);

        SetState(AlienState.Dead);
        Destroy(gameObject, 1.5f);
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
        if (healthBarSlider == null) return;   // ← защита null как у Robot
        healthBarSlider.value = CurrentHealth / maxHealth;
    }

    public float HealthPercent => CurrentHealth / maxHealth;

    #endregion

    // ══════════════════════════════════════════════════════════════════════
    #region State Machine

    public void SetState(AlienState newState)
    {
        currentState = newState;
        SetAnim(false, false);

        switch (newState)
        {
            case AlienState.Idle:
                _agent.isStopped = true;
                _agent.ResetPath();
                break;

            case AlienState.Activating:
                _agent.isStopped = true;
                Invoke(nameof(OnActivationComplete), 0.5f);
                break;

            case AlienState.Dead:
                _agent.isStopped = true;
                _agent.ResetPath();
                _anim.SetBool(H_Death, true);
                var col = GetComponent<Collider2D>();
                if (col) col.enabled = false;
                enabled = false;
                break;
        }
    }

    void OnActivationComplete()
    {
        if (currentState == AlienState.Activating)
            SetState(AlienState.Chasing);
    }

    #endregion

    // ══════════════════════════════════════════════════════════════════════
    #region Animation & Sprite Helpers

    void FlipTowards(Vector3 target)
    {
        if (spriteRenderer == null) return;
        float dx = target.x - transform.position.x;
        if (Mathf.Abs(dx) < 0.01f) return;
        spriteRenderer.flipX = defaultFacingRight ? dx < 0 : dx > 0;
    }

    void SetAnim(bool run, bool attack)
    {
        _anim.SetBool(H_Run, run);
        _anim.SetBool(H_Attack, attack);
    }

    #endregion

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log($"Trigger hit by: {col.gameObject.name}");
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log($"Collision hit by: {col.gameObject.name}");
    }
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

        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 1.2f,
            $"HP: {CurrentHealth:0}/{maxHealth:0}  [{currentState}]");
    }
#endif
    #endregion
}