using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Robot : MonoBehaviour
{
    // ─── Движение ──────────────────────────────────────────────────────────
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float stopDistance = 0.5f;

    // ─── Обнаружение ───────────────────────────────────────────────────────
    [Header("Detection")]
    public float activationRadius = 5f;
    public float chaseDistance = 15f;
    public float attackRadius = 2.5f;

    // ─── Атака ─────────────────────────────────────────────────────────────
    [Header("Attack")]
    public float attackDamage = 5f;
    public float attackCooldown = 2f;
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float bulletSpeed = 8f;

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

    // ─── AI вкл/выкл ───────────────────────────────────────────────────────
    [Header("AI")]
    [Tooltip("Выключи чтобы заморозить робота")]
    public bool aiEnabled = true;

    // ─── Прямая видимость ──────────────────────────────────────────────────
    [Header("Line of Sight")]
    [Tooltip("Слои препятствий, которые блокируют выстрел")]
    public LayerMask obstacleLayer;

    [Tooltip("Радиус поиска позиции при отсутствии LoS")]
    public float repositionRadius = 3f;

    [Tooltip("Сколько попыток найти точку с LoS")]
    public int repositionAttempts = 8;

    // ─── Состояния ─────────────────────────────────────────────────────────
    public enum RobotState { Idle, Activating, Chasing, Attacking, Repositioning, Returning, Dead }
    public RobotState currentState = RobotState.Idle;

    // ─── Приватные ─────────────────────────────────────────────────────────
    private NavMeshAgent agent;
    private Animator anim;
    private Transform player;
    private SoundHandlerEnemy sfx;

    private Vector3 startingPosition;
    private Vector3 repositionTarget;
    private float lastAttackTime;

    // ─── Animator hashes ───────────────────────────────────────────────────
    private static readonly int hIdle = Animator.StringToHash("Idle");
    private static readonly int hActivate = Animator.StringToHash("Activate");
    private static readonly int hMoveUp = Animator.StringToHash("Move_up");
    private static readonly int hMoveDown = Animator.StringToHash("Move_do");
    private static readonly int hMoveLeft = Animator.StringToHash("Move_lef");
    private static readonly int hMoveRight = Animator.StringToHash("Move_rig");
    private static readonly int hAtUp = Animator.StringToHash("At_up");
    private static readonly int hAtDown = Animator.StringToHash("At_down");
    private static readonly int hAtLeft = Animator.StringToHash("At_left");
    private static readonly int hAtRight = Animator.StringToHash("At_right");
    private static readonly int hIsDead = Animator.StringToHash("isDead");
    private static readonly int hTakeHit = Animator.StringToHash("TakeHit");

    // ══════════════════════════════════════════════════════════════════════
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        sfx = GetComponent<SoundHandlerEnemy>();
        player = GameObject.FindGameObjectWithTag("Player_1").transform;

        startingPosition = transform.position;

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = moveSpeed;
        agent.stoppingDistance = stopDistance;

        CurrentHealth = maxHealth;
        IsDead = false;
        RefreshHealthBar();

        SetState(RobotState.Idle);
    }

    // ══════════════════════════════════════════════════════════════════════
    void Update()
    {
        if (currentState == RobotState.Dead || !aiEnabled) return;

        float dist = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case RobotState.Idle: HandleIdle(dist); break;
            case RobotState.Activating: break;
            case RobotState.Chasing: HandleChase(dist); break;
            case RobotState.Attacking: HandleAttack(dist); break;
            case RobotState.Repositioning: HandleReposition(dist); break;
            case RobotState.Returning: HandleReturn(dist); break;
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    #region HP System

    /// <summary>Нанести урон роботу. Вызывай из пуль, ловушек и т.д.</summary>
    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        RefreshHealthBar();

        if (hitFX != null)
            Instantiate(hitFX, transform.position, Quaternion.identity);

        sfx?.PlayHitSound();

        if (anim.HasState(0, hTakeHit))
            anim.SetTrigger(hTakeHit);

        if (CurrentHealth <= 0f)
            Die();
    }

    /// <summary>Убить робота немедленно.</summary>
    public void Die()
    {
        if (IsDead) return;
        IsDead = true;
        CurrentHealth = 0f;
        RefreshHealthBar();

        if (deathFX != null)
            Instantiate(deathFX, transform.position, Quaternion.identity);

        SetState(RobotState.Dead);
        Destroy(gameObject, 1f);
    }

    /// <summary>Вернуть HP (лечение).</summary>
    public void Heal(float amount)
    {
        if (IsDead) return;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        RefreshHealthBar();
    }

    void RefreshHealthBar()
    {
        if (healthBarSlider == null) return;
        healthBarSlider.value = CurrentHealth / maxHealth;
    }

    public float HealthPercent => CurrentHealth / maxHealth;

    #endregion

    // ══════════════════════════════════════════════════════════════════════
    #region State Handlers

    void HandleIdle(float dist)
    {
        if (dist <= activationRadius)
            SetState(RobotState.Activating);
    }

    void HandleChase(float dist)
    {
        if (dist > chaseDistance) { SetState(RobotState.Returning); return; }
        if (dist <= attackRadius) { SetState(RobotState.Attacking); return; }

        agent.isStopped = false;
        agent.SetDestination(player.position);
        UpdateMoveAnim(Get4Dir(player.position));
    }

    void HandleAttack(float dist)
    {
        agent.isStopped = true;

        if (dist > attackRadius * 1.5f) { SetState(RobotState.Chasing); return; }

        UpdateAttackAnim(Get4Dir(player.position));

        if (Time.time < lastAttackTime + attackCooldown) return;

        if (HasLineOfSight())
        {
            Shoot();
            lastAttackTime = Time.time;
        }
        else
        {
            if (TryFindRepositionPoint(out Vector3 point))
            {
                repositionTarget = point;
                SetState(RobotState.Repositioning);
            }
            else
            {
                SetState(RobotState.Chasing);
            }
        }
    }

    void HandleReposition(float dist)
    {
        float distToTarget = Vector2.Distance(transform.position, repositionTarget);

        if (distToTarget < 0.3f) { SetState(RobotState.Attacking); return; }
        if (HasLineOfSight() && dist <= attackRadius) { SetState(RobotState.Attacking); return; }
        if (dist > chaseDistance) { SetState(RobotState.Returning); return; }

        agent.isStopped = false;
        agent.SetDestination(repositionTarget);
        UpdateMoveAnim(Get4Dir(repositionTarget));
    }

    void HandleReturn(float dist)
    {
        if (dist <= chaseDistance) { SetState(RobotState.Chasing); return; }

        float distToStart = Vector2.Distance(transform.position, startingPosition);
        if (distToStart > 0.15f)
        {
            agent.isStopped = false;
            agent.SetDestination(startingPosition);
            UpdateMoveAnim(Get4Dir(startingPosition));
        }
        else
        {
            agent.isStopped = true;
            SetState(RobotState.Idle);
        }
    }

    #endregion

    // ══════════════════════════════════════════════════════════════════════
    #region Line of Sight

    bool HasLineOfSight()
    {
        Vector2 origin = shootPoint != null ? (Vector2)shootPoint.position : (Vector2)transform.position;

        if (IsShootPointBlocked(origin)) return false;

        Vector2 target = player.position;
        Vector2 direction = (target - origin).normalized;
        float distance = Vector2.Distance(origin, target);

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, obstacleLayer);
        return hit.collider == null;
    }

    bool IsShootPointBlocked(Vector2 point)
    {
        Collider2D overlap = Physics2D.OverlapPoint(point, obstacleLayer);
        return overlap != null;
    }

    bool TryFindRepositionPoint(out Vector3 result)
    {
        for (int i = 0; i < repositionAttempts; i++)
        {
            float angle = i * (360f / repositionAttempts) * Mathf.Deg2Rad;
            Vector3 candidate = player.position + new Vector3(
                Mathf.Cos(angle) * repositionRadius,
                Mathf.Sin(angle) * repositionRadius, 0f);

            if (!NavMesh.SamplePosition(candidate, out NavMeshHit navHit, 1f, NavMesh.AllAreas))
                continue;

            Vector3 navPoint = navHit.position;
            Vector2 dir = ((Vector2)player.position - (Vector2)navPoint).normalized;
            float dist = Vector2.Distance(navPoint, player.position);
            RaycastHit2D los = Physics2D.Raycast(navPoint, dir, dist, obstacleLayer);

            if (los.collider == null) { result = navPoint; return true; }
        }
        result = Vector3.zero;
        return false;
    }

    #endregion

    // ══════════════════════════════════════════════════════════════════════
    #region State Machine

    public void SetState(RobotState newState)
    {
        currentState = newState;
        ResetAnimFlags();

        switch (newState)
        {
            case RobotState.Idle:
                agent.isStopped = true;
                anim.SetBool(hIdle, true);
                break;

            case RobotState.Activating:
                agent.isStopped = true;
                anim.SetBool(hActivate, true);
                Invoke(nameof(OnActivationComplete), 1f);
                break;

            case RobotState.Chasing:
            case RobotState.Returning:
            case RobotState.Repositioning:
                agent.isStopped = false;
                break;

            case RobotState.Attacking:
                agent.isStopped = true;
                break;

            case RobotState.Dead:
                agent.isStopped = true;
                anim.SetBool(hIsDead, true);
                enabled = false;
                break;
        }
    }

    public void OnActivationComplete()
    {
        if (currentState == RobotState.Activating)
            SetState(RobotState.Chasing);
    }

    #endregion

    // ══════════════════════════════════════════════════════════════════════
    #region Combat

    void Shoot()
    {
        if (bulletPrefab == null || shootPoint == null) return;

        Vector2 exactDir = ((Vector2)(player.position - shootPoint.position)).normalized;
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);

        EnemyProjectile proj = bullet.GetComponent<EnemyProjectile>();
        if (proj != null)
        {
            proj.Speed = bulletSpeed;
            proj.Fire(exactDir);
        }
        else
        {
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = exactDir * bulletSpeed;
            float angle = Mathf.Atan2(exactDir.y, exactDir.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        sfx?.PlayAttackSound();
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

    void UpdateMoveAnim(Vector2 dir)
    {
        ResetAnimFlags();
        if (dir == Vector2.up) anim.SetBool(hMoveUp, true);
        else if (dir == Vector2.down) anim.SetBool(hMoveDown, true);
        else if (dir == Vector2.left) anim.SetBool(hMoveLeft, true);
        else if (dir == Vector2.right) anim.SetBool(hMoveRight, true);
    }

    void UpdateAttackAnim(Vector2 dir)
    {
        ResetAnimFlags();
        if (dir == Vector2.up) anim.SetBool(hAtUp, true);
        else if (dir == Vector2.down) anim.SetBool(hAtDown, true);
        else if (dir == Vector2.left) anim.SetBool(hAtLeft, true);
        else if (dir == Vector2.right) anim.SetBool(hAtRight, true);
    }

    void ResetAnimFlags()
    {
        anim.SetBool(hIdle, false);
        anim.SetBool(hActivate, false);
        anim.SetBool(hMoveUp, false);
        anim.SetBool(hMoveDown, false);
        anim.SetBool(hMoveLeft, false);
        anim.SetBool(hMoveRight, false);
        anim.SetBool(hAtUp, false);
        anim.SetBool(hAtDown, false);
        anim.SetBool(hAtLeft, false);
        anim.SetBool(hAtRight, false);
    }

    #endregion

    // ══════════════════════════════════════════════════════════════════════
    #region Debug Gizmos
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        if (player == null) return;

        Vector2 origin = shootPoint != null ? (Vector2)shootPoint.position : (Vector2)transform.position;

        bool spBlocked = IsShootPointBlocked(origin);
        Gizmos.color = spBlocked ? Color.red : Color.green;
        Gizmos.DrawWireSphere(origin, 0.1f);

        bool los = HasLineOfSight();
        Gizmos.color = los ? Color.cyan : Color.red;
        Gizmos.DrawLine(origin, player.position);

        if (currentState == RobotState.Repositioning)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(repositionTarget, 0.2f);
            Gizmos.DrawLine(transform.position, repositionTarget);
        }

        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 1.2f,
            $"HP: {CurrentHealth:0}/{maxHealth:0}  [{currentState}]"
        );
    }
#endif
    #endregion
}