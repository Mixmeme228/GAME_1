using UnityEngine;

/// <summary>
/// Обычный снаряд босса — летит прямо в направлении игрока.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class BossBullet : MonoBehaviour
{
    [Header("Параметры")]
    public float speed = 12f;
    public float damage = 20f;
    public float lifetime = 4f;

    [Header("VFX (опционально)")]
    public GameObject hitEffectPrefab;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        gameObject.tag = "BossProjectile";
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector2 dir)
    {
        rb.linearVelocity = dir.normalized * speed;

        // Поворачиваем спрайт по направлению
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BossProjectile") || other.CompareTag("Enemy")) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(damage);
        }

        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
