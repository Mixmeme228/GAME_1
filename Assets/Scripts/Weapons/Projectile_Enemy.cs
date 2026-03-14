using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float Speed { get => _Speed; set => _Speed = value; }
    public float LifeTime { get => _LifeTime; set => _LifeTime = value; }

    [SerializeField] private float _Speed = 8f;
    [SerializeField] private float _LifeTime = 2f;

    [Header("FXs")]
    [SerializeField] private GameObject hitPFX = null;

    [Header("Damage")]
    [SerializeField] private float damage = 5f;

    // Теги которые останавливают пулю
    private static readonly string[] stopTags = { "Wall", "Player_1", "Obstacle" };

    private BoxCollider2D _Collider;
    private SpriteRenderer _Renderer;
    private SoundHandlerLocal _Sfx;
    private Vector2 travelDirection;
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
        if (_Renderer == null || _Collider == null)
            Debug.LogWarning(gameObject.name + ": Missing BoxCollider2D or SpriteRenderer!");
    }

    private void Update()
    {
        if (hasLaunched) Travel();
    }

    // ──────────────────────────────────────────────
    /// <summary>Вызывается из Robot.Shoot() — передаёт направление выстрела.</summary>
    public void Fire(Vector2 direction)
    {
        travelDirection = direction.normalized;
        hasLaunched = true;
        RotateToDirection(travelDirection);
        _Sfx?.PlaySound(0);
        transform.parent = null;
        Destroy(gameObject, LifeTime);
    }

    public void SetActive(bool value)
    {
        if (_Renderer != null) _Renderer.enabled = value;
        if (_Collider != null) _Collider.enabled = value;
    }

    // ──────────────────────────────────────────────
    private void Travel()
    {
        transform.Translate(travelDirection * (Time.deltaTime * _Speed), Space.World);
    }

    private void RotateToDirection(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    // ──────────────────────────────────────────────
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (impactCount > 0) return;

        bool shouldStop = false;
        foreach (var tag in stopTags)
            if (collision.CompareTag(tag)) { shouldStop = true; break; }

        if (!shouldStop) return;

        impactCount++;

        // Наносим урон игроку через PlayerHealth
        if (collision.CompareTag("Player_1"))
        {
            PlayerHealth ph = collision.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(damage);
        }

        // Эффект попадания
        if (hitPFX != null)
            Instantiate(hitPFX, collision.ClosestPoint(transform.position), Quaternion.identity);

        Destroy(gameObject);
    }
}