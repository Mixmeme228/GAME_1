using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossAI2 : MonoBehaviour
{
    // ─── HP ────────────────────────────────────────────────────────────────
    [Header("Health")]
    public float maxHealth = 500f;
    public Slider healthBarSlider;
    public GameObject hitFX;
    public GameObject deathFX;

    // ─── Щит ───────────────────────────────────────────────────────────────
    [Header("Shield")]
    public float maxShieldHP = 200f;
    public Slider shieldBarSlider;
    public GameObject shieldVisual;
    public GameObject shieldBreakFX;
    public float shieldDamageReduction = 0.1f;
    [Tooltip("Через сколько секунд без урона щит восстанавливается")]
    public float shieldRegenDelay = 6f;
    [Tooltip("Щит восстанавливается после получения N урона по телу")]
    public float shieldRegenOnDamage = 100f;

    // ─── Лазерная атака ────────────────────────────────────────────────────
    [Header("Laser Attack")]
    public LineRenderer bossLaserLine;
    public Transform laserFirePoint;
    public float laserDamagePerSecond = 20f;
    public float laserDuration = 3f;
    public float laserCooldown = 8f;
    [Tooltip("Сколько раз подряд стреляет лазером")]
    public int laserBurstCount = 3;
    [Tooltip("Пауза между выстрелами в серии")]
    public float laserBurstDelay = 0.4f;
    public LayerMask laserHitLayers;

    // ─── Призыв ────────────────────────────────────────────────────────────
    [Header("Summon")]
    public GameObject minionPrefab;
    public Transform[] summonPoints;
    public int summonCount = 2;
    public float summonCooldown = 15f;

    // ─── Лечение ───────────────────────────────────────────────────────────
    [Header("Heal")]
    public float healAmount = 60f;
    public float healCooldown = 20f;
    [Tooltip("Порог HP (0-1) при котором босс лечится")]
    public float healThreshold = 0.4f;
    public GameObject healFX;

    // ─── Обнаружение ───────────────────────────────────────────────────────
    [Header("Detection")]
    public float activationRadius = 10f;
    public float attackRadius = 1.5f;
    public float meleeDamage = 5f;
    public float meleeCooldown = 1.5f;

    // ─── Движение ──────────────────────────────────────────────────────────
    [Header("Movement")]
    public float moveSpeed = 2.5f;

    // ─── Звук ──────────────────────────────────────────────────────────────
    [Header("Sound")]
    public SoundHandlerEnemy sfx;

    // ─── Приватные ─────────────────────────────────────────────────────────
    private Animator _anim;
    private Transform _player;
    private UnityEngine.AI.NavMeshAgent _agent;
    private SpriteRenderer _sr;

    private float _currentHealth;
    private float _currentShieldHP;
    private bool _shieldActive = true;
    private bool _isDead = false;

    private float _lastLaserTime = -99f;
    private float _lastSummonTime = -99f;
    private float _lastHealTime = -99f;
    private float _lastMeleeTime = -99f;
    private bool _isLaserFiring = false;

    private float _lastShieldHitTime = -99f;
    private float _damageAfterShieldBreak = 0f;

    private GameObject _laserHitFXInstance;
    private int _phase = 0;

    // Animator hashes — все Bool как в аниматоре
    private static readonly int H_Run = Animator.StringToHash("Run");
    private static readonly int H_Attack = Animator.StringToHash("Attack");
    private static readonly int H_Damaged = Animator.StringToHash("Damaged");
    private static readonly int H_Death = Animator.StringToHash("Death");

    // ══════════════════════════════════════════════════════════════════════
    void Start()
    {
        _anim = GetComponent<Animator>();
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        sfx = sfx ?? GetComponent<SoundHandlerEnemy>();

        if (_agent != null)
        {
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            _agent.speed = moveSpeed;
        }

        var p = GameObject.FindGameObjectWithTag("Player_1");
        if (p) _player = p.transform;

        _currentHealth = maxHealth;
        _currentShieldHP = maxShieldHP;

        RefreshHealthBar();
        RefreshShieldBar();

        // Настраиваем LineRenderer
        if (bossLaserLine != null)
        {
            bossLaserLine.positionCount = 2;
            bossLaserLine.useWorldSpace = true;
            bossLaserLine.sortingOrder = 100;
            bossLaserLine.sortingLayerName = "Default";
            bossLaserLine.startWidth = 0.15f;
            bossLaserLine.endWidth = 0.07f;
            bossLaserLine.startColor = Color.red;
            bossLaserLine.endColor = new Color(1f, 0.3f, 0f);
            if (bossLaserLine.sharedMaterial == null)
                bossLaserLine.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
            bossLaserLine.enabled = false;
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    void Update()
    {
        if (_isDead || _player == null) return;

        float dist = Vector2.Distance(transform.position, _player.position);
        if (dist > activationRadius) return;

        CheckPhase();
        TryHeal();
        TryRegenShield();
        TrySummon();
        TryLaserAttack(dist);

        if (!_isLaserFiring)
            HandleMovement(dist);
    }

    // ══════════════════════════════════════════════════════════════════════
    #region Phase

    void CheckPhase()
    {
        if (_phase == 0 && HealthPercent < 0.5f)
        {
            _phase = 1;
            laserCooldown = Mathf.Max(4f, laserCooldown * 0.6f);
            summonCooldown = Mathf.Max(8f, summonCooldown * 0.6f);
            if (_agent != null) _agent.speed = moveSpeed * 1.4f;
            Debug.Log("[Boss] Фаза 2!");
        }
    }

    #endregion

    // ══════════════════════════════════════════════════════════════════════
    #region Movement & Melee

    void HandleMovement(float dist)
    {
        if (dist <= attackRadius)
        {
            StopAgent();
            SetRunAnim(false);
            TryMeleeAttack();
        }
        else
        {
            if (_agent != null)
            {
                _agent.isStopped = false;
                _agent.SetDestination(_player.position);
            }
            SetRunAnim(true);
            SetAttackAnim(false);
            FlipTowards(_player.position);
        }
    }

    void TryMeleeAttack()
    {
        if (Time.time < _lastMeleeTime + meleeCooldown) return;
        _lastMeleeTime = Time.time;

        SetAttackAnim(true);
        StartCoroutine(ResetAttackAnim(0.5f));

        _player.GetComponent<PlayerHealth>()?.TakeDamage(meleeDamage);
        sfx?.PlayAttackSound();
    }

    IEnumerator ResetAttackAnim(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetAttackAnim(false);
    }

    #endregion

    // ══════════════════════════════════════════════════════════════════════
    #region Laser Attack

    void TryLaserAttack(float dist)
    {
        if (_isLaserFiring) return;
        if (Time.time < _lastLaserTime + laserCooldown) return;
        if (laserFirePoint == null || bossLaserLine == null) return;

        _lastLaserTime = Time.time;
        StartCoroutine(FireLaserRoutine());
    }

    IEnumerator FireLaserRoutine()
    {
        _isLaserFiring = true;

        StopAgent();
        SetRunAnim(false);
        SetAttackAnim(true);

        bossLaserLine.positionCount = 2;
        bossLaserLine.useWorldSpace = true;
        bossLaserLine.sortingOrder = 100;
        bossLaserLine.sortingLayerName = "Default";
        bossLaserLine.startWidth = 0.5f;
        bossLaserLine.endWidth = 0.15f;
        bossLaserLine.startColor = Color.red;
        bossLaserLine.endColor = new Color(1f, 0.4f, 0f, 0.8f);
        if (bossLaserLine.sharedMaterial == null)
            bossLaserLine.sharedMaterial = new Material(Shader.Find("Sprites/Default"));

        var playerObj = GameObject.FindGameObjectWithTag("Player_1");
        PlayerHealth ph = playerObj != null ? playerObj.GetComponent<PlayerHealth>() : null;

        for (int shot = 0; shot < laserBurstCount; shot++)
        {
            // Фиксируем направление на игрока в момент каждого выстрела
            Vector2 origin = laserFirePoint.position;
            Vector2 direction = ((Vector2)_player.position - origin).normalized;
            Vector3 endPoint = (Vector3)origin + (Vector3)(direction * 30f);

            FlipTowards(_player.position);

            bossLaserLine.SetPosition(0, origin);
            bossLaserLine.SetPosition(1, endPoint);
            bossLaserLine.enabled = true;

            // Луч активен laserDuration (1 сек) — наносим урон каждый кадр
            float elapsed = 0f;
            while (elapsed < laserDuration)
            {
                if (ph != null && playerObj != null)
                {
                    float dist = DistancePointToSegment(
                        playerObj.transform.position, origin, endPoint);
                    if (dist < 0.6f)
                    {
                        ph.SetInvincible(0f);
                        ph.TakeDamage(laserDamagePerSecond * Time.deltaTime);
                    }
                }
                elapsed += Time.deltaTime;
                yield return null;
            }

            bossLaserLine.enabled = false;

            // Пауза между выстрелами
            if (shot < laserBurstCount - 1)
                yield return new WaitForSeconds(laserBurstDelay);
        }

        SetAttackAnim(false);
        _isLaserFiring = false;
    }

    #endregion

    // ══════════════════════════════════════════════════════════════════════
    #region Summon

    void TrySummon()
    {
        if (Time.time < _lastSummonTime + summonCooldown) return;
        if (minionPrefab == null || summonPoints == null || summonPoints.Length == 0) return;

        _lastSummonTime = Time.time;
        StartCoroutine(SummonRoutine());
    }

    IEnumerator SummonRoutine()
    {
        StopAgent();
        SetRunAnim(false);

        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < summonCount; i++)
        {
            Transform sp = summonPoints[i % summonPoints.Length];
            Instantiate(minionPrefab, sp.position, Quaternion.identity);
        }

        Debug.Log($"[Boss] Призвано {summonCount} миньонов!");
    }

    #endregion

    // ══════════════════════════════════════════════════════════════════════
    #region Heal

    void TryHeal()
    {
        if (Time.time < _lastHealTime + healCooldown) return;
        if (HealthPercent > healThreshold) return;

        _lastHealTime = Time.time;
        _currentHealth = Mathf.Min(maxHealth, _currentHealth + healAmount);
        RefreshHealthBar();

        if (healFX != null)
            Instantiate(healFX, transform.position, Quaternion.identity);
    }

    #endregion

    // ══════════════════════════════════════════════════════════════════════
    #region HP & Shield

    public void TakeShieldDamage(float amount)
    {
        if (!_shieldActive || _isDead) return;
        _lastShieldHitTime = Time.time;
        _currentShieldHP = Mathf.Max(0f, _currentShieldHP - amount);
        RefreshShieldBar();
        if (_currentShieldHP <= 0f) BreakShield();
    }

    public void TakeDamage(float amount)
    {
        if (_isDead) return;

        float dmg = _shieldActive ? amount * shieldDamageReduction : amount;
        _currentHealth = Mathf.Max(0f, _currentHealth - dmg);
        RefreshHealthBar();

        // Считаем урон по телу после разрушения щита
        if (!_shieldActive)
        {
            _damageAfterShieldBreak += dmg;
            if (_damageAfterShieldBreak >= shieldRegenOnDamage)
                RegenerateShield();
        }

        if (hitFX != null) Instantiate(hitFX, transform.position, Quaternion.identity);
        sfx?.PlayHitSound();

        SetDamagedAnim(true);
        StartCoroutine(ResetDamagedAnim(0.2f));
        StartCoroutine(HurtFlash());

        if (_currentHealth <= 0f) Die();
    }

    IEnumerator ResetDamagedAnim(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetDamagedAnim(false);
    }

    void BreakShield()
    {
        _shieldActive = false;
        _damageAfterShieldBreak = 0f;
        _lastShieldHitTime = Time.time;
        if (shieldVisual != null) shieldVisual.SetActive(false);
        if (shieldBreakFX != null) Instantiate(shieldBreakFX, transform.position, Quaternion.identity);
        Debug.Log("[Boss] Щит разрушен!");
    }

    void TryRegenShield()
    {
        if (_shieldActive || _isDead) return;
        // Восстанавливаем если прошло shieldRegenDelay секунд без урона по щиту
        if (Time.time >= _lastShieldHitTime + shieldRegenDelay)
            RegenerateShield();
    }

    void RegenerateShield()
    {
        if (_shieldActive || _isDead) return;
        _shieldActive = true;
        _currentShieldHP = maxShieldHP;
        _damageAfterShieldBreak = 0f;
        _lastShieldHitTime = Time.time;
        RefreshShieldBar();
        if (shieldVisual != null) shieldVisual.SetActive(true);
        if (shieldBreakFX != null) Instantiate(shieldBreakFX, transform.position, Quaternion.identity);
        Debug.Log("[Boss] Щит восстановлен!");
    }

    public void Die()
    {
        if (_isDead) return;
        _isDead = true;
        _currentHealth = 0f;
        RefreshHealthBar();

        StopAllCoroutines();

        if (bossLaserLine != null) bossLaserLine.enabled = false;
        StopAgent();

        SetRunAnim(false);
        SetAttackAnim(false);
        _anim.SetBool(H_Death, true);

        if (deathFX != null) Instantiate(deathFX, transform.position, Quaternion.identity);

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, 2f);
    }

    void RefreshHealthBar() { if (healthBarSlider != null) healthBarSlider.value = _currentHealth / maxHealth; }
    void RefreshShieldBar() { if (shieldBarSlider != null) shieldBarSlider.value = _currentShieldHP / maxShieldHP; }

    public float HealthPercent => _currentHealth / maxHealth;
    public bool IsDead => _isDead;
    public bool ShieldActive => _shieldActive;

    #endregion

    // ══════════════════════════════════════════════════════════════════════
    #region Helpers

    void StopAgent()
    {
        if (_agent != null) _agent.isStopped = true;
    }

    void FlipTowards(Vector3 target)
    {
        if (_sr == null) return;
        float dx = target.x - transform.position.x;
        if (Mathf.Abs(dx) > 0.01f) _sr.flipX = dx < 0;
    }

    // ── Анимации через Bool ────────────────────────────────────────────────
    void SetRunAnim(bool value) => _anim.SetBool(H_Run, value);
    void SetAttackAnim(bool value) => _anim.SetBool(H_Attack, value);
    void SetDamagedAnim(bool value) => _anim.SetBool(H_Damaged, value);

    IEnumerator HurtFlash()
    {
        if (_sr != null) _sr.color = new Color(1f, 0.3f, 0.3f);
        yield return new WaitForSeconds(0.1f);
        if (_sr != null) _sr.color = Color.white;
    }

    /// <summary>Расстояние от точки до отрезка в 2D.</summary>
    float DistancePointToSegment(Vector2 point, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        Vector2 ap = point - a;
        float t = Mathf.Clamp01(Vector2.Dot(ap, ab) / ab.sqrMagnitude);
        Vector2 closest = a + t * ab;
        return Vector2.Distance(point, closest);
    }

    #endregion

    // ══════════════════════════════════════════════════════════════════════
    #region Gizmos
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
#endif
    #endregion
}