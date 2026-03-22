using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Speed { get => _Speed; set => _Speed = value; }
    public float LifeTime { get => _LifeTime; set => _LifeTime = value; }

    [SerializeField] private float _Speed = 20f;
    [SerializeField] private float _LifeTime = 1f;

    [Header("Damage")]
    [SerializeField] private float _Damage = 10f;
    public float Damage => _Damage;

    [Header("FXs")]
    [SerializeField] private GameObject hitPFX = null;

    private BoxCollider2D _Collider;
    private SpriteRenderer _Renderer;
    private SoundHandlerLocal _Sfx;
    private Vector3 travelDirection;
    private float movement;
    private bool hasLaunched;
    private int impactCount;

    // ──────────────────────────────────────────────
    private void Awake()
    {
        TryGetComponent(out _Sfx);
        TryGetComponent(out _Renderer);
        TryGetComponent(out _Collider);
    }

    private void Start()
    {
        if (_Sfx == null || _Renderer == null || _Collider == null)
            Debug.LogWarning(gameObject.name + ": BoxCollider2D || SpriteRenderer || SoundFXHandler!");
    }

    private void Update()
    {
        if (hasLaunched) Travel();
    }

    // ──────────────────────────────────────────────
    public void SetActive(bool value)
    {
        _Renderer.enabled = _Collider.enabled = value;
    }

    public void Fire()
    {
        hasLaunched = true;
        _Sfx?.PlaySound(0);
        travelDirection = PlayerBodyPartsHandler.isRightDirection ? Vector3.right : -Vector3.right;
        transform.parent = null;
        Invoke(nameof(DestroyByLifetime), LifeTime);
    }

    public void Fire(Vector2 direction)
    {
        hasLaunched = true;
        travelDirection = direction;
        _Sfx?.PlaySound(0);
        transform.parent = null;
        Invoke(nameof(DestroyByLifetime), LifeTime);
    }

    // ──────────────────────────────────────────────
    private void Travel()
    {
        movement = Time.deltaTime * Speed;
        transform.Translate(travelDirection.normalized * movement, Space.Self);
    }

    // ──────────────────────────────────────────────
    private void DestroyByLifetime()
    {
        if (hitPFX != null)
            Instantiate(hitPFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    // ──────────────────────────────────────────────
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (impactCount > 0) return;

        // ── BossAI (старый) ───────────────────────────────────────────────
        BossAI boss = collision.GetComponentInParent<BossAI>();
        if (boss != null)
        {
            impactCount++;
            boss.TakeDamage(_Damage);
            SpawnHitFX(collision);
            CancelInvoke(nameof(DestroyByLifetime));
            Destroy(gameObject);
            return;
        }

        // ── BossAI2 — обычный снаряд бьёт по телу (не щиту) ──────────────
        BossAI2 boss2 = collision.GetComponentInParent<BossAI2>();
        if (boss2 != null)
        {
            impactCount++;
            boss2.TakeDamage(_Damage);  // щит поглощает урон по shieldDamageReduction
            SpawnHitFX(collision);
            CancelInvoke(nameof(DestroyByLifetime));
            Destroy(gameObject);
            return;
        }

        // ── Обычные теги ──────────────────────────────────────────────────
        bool shouldHit = collision.CompareTag("Wall")
                      || collision.CompareTag("Enemy")
                      || collision.CompareTag("Player");
        if (!shouldHit) return;

        impactCount++;

        if (collision.CompareTag("Enemy"))
        {
            Robot robot = collision.GetComponentInParent<Robot>();
            if (robot != null) { robot.TakeDamage(_Damage); }

            ZombieAI zombie = collision.GetComponentInParent<ZombieAI>();
            if (zombie != null) { zombie.TakeDamage(_Damage); }

            AlienAI alien = collision.GetComponentInParent<AlienAI>();
            if (alien != null) { alien.TakeDamage(_Damage); }
        }

        SpawnHitFX(collision);
        CancelInvoke(nameof(DestroyByLifetime));
        Destroy(gameObject);
    }

    private void SpawnHitFX(Collider2D collision)
    {
        if (hitPFX != null)
            Instantiate(hitPFX, collision.ClosestPoint(transform.position), Quaternion.identity);
    }
}