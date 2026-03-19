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
        Destroy(gameObject, LifeTime);
    }

    // ──────────────────────────────────────────────
    private void Travel()
    {
        movement = Time.deltaTime * Speed;
        transform.Translate(travelDirection.normalized * movement, Space.Self);
    }

    // ──────────────────────────────────────────────
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (impactCount > 0) return;

        BossAI boss = collision.GetComponent<BossAI>();
        if (boss != null)
        {
            impactCount++;
            boss.TakeDamage(_Damage);
            if (hitPFX != null)
                Instantiate(hitPFX, collision.ClosestPoint(transform.position), Quaternion.identity);
            Destroy(gameObject);
            return;
        }

        bool shouldHit = collision.CompareTag("Wall")
                      || collision.CompareTag("Enemy")
                      || collision.CompareTag("Player");
        if (!shouldHit) return;

        impactCount++;

        // ── Урон врагам ───────────────────────────────────────────────────
        if (collision.CompareTag("Enemy"))
        {
            // Робот
            Robot robot = collision.GetComponent<Robot>();
            if (robot != null)
                robot.TakeDamage(_Damage);

            // Зомби
            ZombieAI zombie = collision.GetComponent<ZombieAI>();
            if (zombie != null)
                zombie.TakeDamage(_Damage);
        }

        // ── Эффект попадания ──────────────────────────────────────────────
        if (hitPFX != null)
            Instantiate(hitPFX, collision.ClosestPoint(transform.position), Quaternion.identity);

        Destroy(gameObject);
    }
}