using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    public enum BossState { Idle, Combat, NormalShot, RocketSalvo, SummonMinions, Death }

    [Header("== Выключатель ИИ ==")]
    public bool aiEnabled = false;

    public void SetAIEnabled(bool enabled)
    {
        aiEnabled = enabled;
        if (!enabled)
        {
            StopAllCoroutines();
            isActing = false;
            if (rb2d != null) rb2d.linearVelocity = Vector2.zero;
            if (animator != null) { animator.SetBool(AnimCombat, false); animator.SetFloat(AnimAngle, 0f); }
            currentState = BossState.Idle;
        }
        else
        {
            ChangeState(BossState.Combat);
        }
    }

    [Header("== Общие настройки ==")]
    public BossState currentState = BossState.Idle;
    public float maxHealth = 500f;
    public float detectionRange = 15f;

    [Header("== Фазы босса ==")]
    public float phase2Threshold = 0.5f;
    private bool isPhase2 = false;

    [Header("== Обычный выстрел ==")]
    public GameObject bulletPrefab;
    public Transform gunMuzzle;
    public float bulletCooldown = 2f;
    public int bulletsPerBurst = 3;
    public float burstDelay = 0.2f;

    [Header("== Ракеты ==")]
    public GameObject rocketPrefab;
    public Transform rocketLauncher;
    public float rocketCooldown = 5f;
    public int rocketsPerSalvo = 2;
    public float rocketSalvoDelay = 0.8f;

    [Header("== Призыв миньонов ==")]
    public GameObject minionPrefab;
    public Transform[] spawnPoints;
    public float summonCooldown = 12f;
    public int minionsPerSummon = 2;
    public int maxMinionsAlive = 4;

    [Header("== Компоненты ==")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb2d;

    [Header("== HP-бар ==")]
    public UnityEngine.UI.Slider healthBarSlider;

    [Header("== VFX ==")]
    public GameObject hitFX;
    public GameObject deathFX;

    private float currentHealth;
    private Transform player;
    private float bulletTimer, rocketTimer, summonTimer;
    private bool isActing;
    private List<GameObject> activeMinions = new List<GameObject>();
    private bool playerInCover = false;

    private static readonly int AnimCombat = Animator.StringToHash("Combat");
    private static readonly int AnimShoot = Animator.StringToHash("Shoot");
    private static readonly int AnimRocket = Animator.StringToHash("Rocket");
    private static readonly int AnimSummon = Animator.StringToHash("Summon");
    private static readonly int AnimHurt = Animator.StringToHash("Hurt");
    private static readonly int AnimDead = Animator.StringToHash("Dead");
    private static readonly int AnimAngle = Animator.StringToHash("Angle");

    // ── Публичные свойства ────────────────────────────────────────────────
    public float HealthPercent => currentHealth / maxHealth;
    public bool IsDead => currentState == BossState.Death;

    // ══════════════════════════════════════════════════════════════════════
    private void Awake()
    {
        currentHealth = maxHealth;
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (rb2d == null) rb2d = GetComponent<Rigidbody2D>();
        if (rb2d != null) { rb2d.gravityScale = 0f; rb2d.constraints = RigidbodyConstraints2D.FreezeAll; }
        RefreshHealthBar();
    }

    private void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player_1");
        if (p) player = p.transform;
        bulletTimer = Random.Range(0f, bulletCooldown * 0.5f);
        rocketTimer = Random.Range(0f, rocketCooldown * 0.5f);
        summonTimer = Random.Range(0f, summonCooldown * 0.5f);
        if (!aiEnabled) { animator?.SetBool(AnimCombat, false); animator?.SetFloat(AnimAngle, 0f); }
        else ChangeState(BossState.Combat);
    }

    private void Update()
    {
        if (!aiEnabled || currentState == BossState.Death || player == null) return;
        if (!isPhase2 && currentHealth / maxHealth <= phase2Threshold) EnterPhase2();
        if (!isActing) { bulletTimer += Time.deltaTime; rocketTimer += Time.deltaTime; summonTimer += Time.deltaTime; }
        switch (currentState)
        {
            case BossState.Idle:
                if (Vector2.Distance(transform.position, player.position) <= detectionRange)
                    ChangeState(BossState.Combat);
                break;
            case BossState.Combat:
                HandleCombat();
                break;
        }
        FlipTowardsPlayer();
        UpdateAnimatorAngle();
    }

    private void HandleCombat()
    {
        if (isActing) return;
        if (summonTimer >= summonCooldown && CountAliveMinions() < maxMinionsAlive)
        { summonTimer = 0f; ChangeState(BossState.SummonMinions); }
        else if (rocketTimer >= rocketCooldown)
        { rocketTimer = 0f; ChangeState(BossState.RocketSalvo); }
        else if (bulletTimer >= bulletCooldown)
        {
            bulletTimer = 0f;
            if (playerInCover)
            {
                if (CountAliveMinions() < maxMinionsAlive) { summonTimer = 0f; ChangeState(BossState.SummonMinions); }
                else { rocketTimer = 0f; ChangeState(BossState.RocketSalvo); }
            }
            else ChangeState(BossState.NormalShot);
        }
    }

    private void ChangeState(BossState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case BossState.Idle: animator?.SetBool(AnimCombat, false); break;
            case BossState.Combat: animator?.SetBool(AnimCombat, true); break;
            case BossState.NormalShot: StartCoroutine(DoNormalShot()); break;
            case BossState.RocketSalvo: StartCoroutine(DoRocketSalvo()); break;
            case BossState.SummonMinions: StartCoroutine(DoSummonMinions()); break;
            case BossState.Death:
                animator?.SetBool(AnimCombat, false);
                animator?.SetTrigger(AnimDead);
                StartCoroutine(DoDeath());
                break;
        }
    }

    private IEnumerator DoNormalShot()
    {
        isActing = true;
        animator?.SetTrigger(AnimShoot);
        for (int i = 0; i < bulletsPerBurst; i++) { SpawnBullet(); yield return new WaitForSeconds(burstDelay); }
        yield return new WaitForSeconds(0.3f);
        isActing = false;
        ChangeState(BossState.Combat);
    }

    private void SpawnBullet()
    {
        if (bulletPrefab == null || gunMuzzle == null) return;
        Vector2 dir = (player.position - gunMuzzle.position).normalized;
        dir = Quaternion.Euler(0, 0, Random.Range(isPhase2 ? -2f : -6f, isPhase2 ? 2f : 6f)) * dir;
        Instantiate(bulletPrefab, gunMuzzle.position, Quaternion.identity).GetComponent<BossBullet>()?.SetDirection(dir);
    }

    private IEnumerator DoRocketSalvo()
    {
        isActing = true;
        animator?.SetTrigger(AnimRocket);
        int count = isPhase2 ? rocketsPerSalvo + 1 : rocketsPerSalvo;
        for (int i = 0; i < count; i++) { SpawnRocket(); yield return new WaitForSeconds(rocketSalvoDelay); }
        yield return new WaitForSeconds(0.5f);
        isActing = false;
        ChangeState(BossState.Combat);
    }

    private void SpawnRocket()
    {
        if (rocketPrefab == null || rocketLauncher == null) return;
        var hr = Instantiate(rocketPrefab, rocketLauncher.position, Quaternion.identity).GetComponent<HomingRocket>();
        if (hr != null) { hr.SetTarget(player); hr.Launch(); }
    }

    private IEnumerator DoSummonMinions()
    {
        isActing = true;
        animator?.SetTrigger(AnimSummon);
        yield return new WaitForSeconds(0.8f);
        int toSpawn = isPhase2 ? minionsPerSummon + 1 : minionsPerSummon;
        for (int i = 0; i < toSpawn; i++)
        {
            if (CountAliveMinions() >= maxMinionsAlive) break;
            Transform pt = GetRandomSpawnPoint();
            if (pt != null) { activeMinions.Add(Instantiate(minionPrefab, pt.position, Quaternion.identity)); yield return new WaitForSeconds(0.3f); }
        }
        yield return new WaitForSeconds(0.4f);
        isActing = false;
        ChangeState(BossState.Combat);
    }

    private Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return transform;
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    private int CountAliveMinions()
    {
        activeMinions.RemoveAll(m => m == null);
        return activeMinions.Count;
    }

    private void EnterPhase2()
    {
        isPhase2 = true;
        bulletCooldown *= 0.7f;
        rocketCooldown *= 0.75f;
        StartCoroutine(FlashRed(3, 0.15f));
    }

    private IEnumerator FlashRed(int times, float interval)
    {
        for (int i = 0; i < times; i++)
        {
            if (spriteRenderer) spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(interval);
            if (spriteRenderer) spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(interval);
        }
    }

    public void TakeDamage(float amount)
    {
        if (currentState == BossState.Death) return;
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        RefreshHealthBar();
        if (hitFX != null) Instantiate(hitFX, transform.position, Quaternion.identity);
        animator?.SetTrigger(AnimHurt);
        StartCoroutine(HurtFlash());
        if (currentHealth <= 0f) Die();
    }

    public void Die()
    {
        if (currentState == BossState.Death) return;
        currentHealth = 0f;
        RefreshHealthBar();
        ChangeState(BossState.Death);
    }

    private void RefreshHealthBar()
    {
        if (healthBarSlider != null) healthBarSlider.value = currentHealth / maxHealth;
    }

    private IEnumerator HurtFlash()
    {
        if (spriteRenderer) spriteRenderer.color = new Color(1f, 0.4f, 0.4f);
        yield return new WaitForSeconds(0.1f);
        if (spriteRenderer) spriteRenderer.color = Color.white;
    }

    private IEnumerator DoDeath()
    {
        isActing = true;
        if (rb2d != null) rb2d.linearVelocity = Vector2.zero;
        if (deathFX != null) Instantiate(deathFX, transform.position, Quaternion.identity);
        foreach (var m in activeMinions) if (m != null) Destroy(m, Random.Range(0f, 0.5f));
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    private void UpdateAnimatorAngle()
    {
        if (player == null || animator == null) return;
        Vector2 delta = player.position - transform.position;
        bool facingLeft = spriteRenderer != null && spriteRenderer.flipX;
        float angle = Mathf.Clamp(Mathf.Atan2(delta.y, facingLeft ? -delta.x : delta.x) * Mathf.Rad2Deg, -90f, 90f);
        animator.SetFloat(AnimAngle, Mathf.MoveTowards(animator.GetFloat(AnimAngle), angle, 270f * Time.deltaTime));
    }

    private void FlipTowardsPlayer()
    {
        if (player == null || spriteRenderer == null) return;
        bool shouldFlip = player.position.x < transform.position.x;
        if (spriteRenderer.flipX == shouldFlip) return;
        spriteRenderer.flipX = shouldFlip;
        FlipPoint(gunMuzzle);
        FlipPoint(rocketLauncher);
    }

    private void FlipPoint(Transform point)
    {
        if (point == null) return;
        Vector3 pos = point.localPosition; pos.x = -pos.x; point.localPosition = pos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Projectile proj = other.GetComponent<Projectile>();
        if (proj != null) { TakeDamage(proj.Damage); Destroy(other.gameObject); }
    }

    public void OnPlayerEnterCover() { playerInCover = true; }
    public void OnPlayerExitCover() { playerInCover = false; }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (Camera.main == null) return;
        Vector3 sp = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
        sp.y = Screen.height - sp.y;
        GUI.color = Color.cyan;
        GUI.Label(new Rect(sp.x - 80, sp.y - 20, 280, 40),
            $"HP:{currentHealth:0}/{maxHealth} | {currentState}{(isPhase2 ? " [P2]" : "")}{(!aiEnabled ? " [ВЫКЛ]" : "")}");
    }
#endif
}