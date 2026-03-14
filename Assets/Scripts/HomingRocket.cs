using System.Collections;
using UnityEngine;

/// <summary>
/// Самонаводящаяся ракета.
/// — Можно уклониться: скорость и поворот не мгновенные
/// — Можно сбить: есть HP, реагирует на тег PlayerBullet
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class HomingRocket : MonoBehaviour
{
    [Header("Движение")]
    public float speed = 7f;
    [Tooltip("Задержка перед стартом — игрок видит ракету и может среагировать")]
    public float launchDelay = 0.6f;
    [Tooltip("Градусов в секунду — ниже = легче уклониться")]
    public float turnSpeed = 120f;
    [Tooltip("Секунд летит прямо перед включением наведения")]
    public float activationDelay = 0.5f;
    [Tooltip("Как часто ракета обновляет позицию игрока (сек) — больше = сильнее запаздывание")]
    public float lagInterval = 0.4f;

    [Header("Можно сбить")]
    [Tooltip("Сколько попаданий выдержит ракета")]
    public int rocketHP = 2;
    public GameObject destroyVFX;

    [Header("Урон и взрыв")]
    public float damage = 45f;
    public float explosionRadius = 1.2f;
    public GameObject explosionPrefab;

    [Header("Время жизни")]
    public float lifetime = 8f;

    // ─── Приватные ───────────────────────────────────────────────────
    private Transform target;
    private Rigidbody2D rb;
    private bool isHoming = false;
    private bool isDead = false;
    private Vector2 laggedTargetPos;   // позиция игрока с запаздыванием
    private float lagTimer = 0f;

    // ─────────────────────────────────────────────────────────────────
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;           // поворот только через код
        gameObject.tag = "BossProjectile";
    }

    // SetTarget вызывается из BossAI ДО того как объект активируется,
    // поэтому используем OnEnable вместо Start для старта движения.
    public void SetTarget(Transform t)
    {
        target = t;
    }

    /// <summary>Ищет игрока по тегу Player_1 если target не задан.</summary>
    private void FindPlayer()
    {
        if (target != null) return;
        GameObject p = GameObject.FindGameObjectWithTag("Player_1");
        if (p != null) target = p.transform;
        else Debug.LogWarning("[HomingRocket] Игрок с тегом Player_1 не найден!");
    }

    private void OnEnable()
    {
        isDead = false;
        isHoming = false;
        // Ждём явного вызова Launch() из BossAI после SetTarget
    }

    /// <summary>
    /// Вызывать из BossAI ПОСЛЕ SetTarget — запускает ракету с задержкой.
    /// </summary>
    public void Launch()
    {
        FindPlayer();
        PointToTarget();        // сразу смотрит на игрока — предупреждение
        StartCoroutine(DelayedLaunch());
        Invoke(nameof(SelfDestruct), lifetime + launchDelay);
    }

    private IEnumerator DelayedLaunch()
    {
        // Ракета висит на месте — игрок видит куда полетит
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(launchDelay);

        // Обновляем направление на случай если игрок успел убежать
        PointToTarget();
        rb.linearVelocity = transform.up * speed;
        StartCoroutine(ActivateHoming());
    }

    // ─────────────────────────────────────────────────────────────────
    private void PointToTarget()
    {
        if (target == null) return;
        Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private IEnumerator ActivateHoming()
    {
        yield return new WaitForSeconds(activationDelay);
        // Инициализируем запомненную позицию текущей позицией игрока
        if (target != null) laggedTargetPos = target.position;
        lagTimer = 0f;
        isHoming = true;
    }

    // ─────────────────────────────────────────────────────────────────
    private void FixedUpdate()
    {
        if (isDead) return;

        // Если цель потерялась — пробуем найти снова
        if (target == null) FindPlayer();

        if (isHoming && target != null)
        {
            // Запоминаем позицию игрока с задержкой — траектория «опаздывает»
            lagTimer += Time.fixedDeltaTime;
            if (lagTimer >= lagInterval)
            {
                lagTimer = 0f;
                laggedTargetPos = target.position;
            }

            Vector2 dir = ((Vector2)laggedTargetPos - (Vector2)transform.position).normalized;
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            float newAngle = Mathf.MoveTowardsAngle(
                                      transform.eulerAngles.z,
                                      targetAngle,
                                      turnSpeed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
        }

        // Всегда летим в сторону своего transform.up
        rb.linearVelocity = transform.up * speed;
    }

    // ─────────────────────────────────────────────────────────────────
    //  СТОЛКНОВЕНИЯ
    // ─────────────────────────────────────────────────────────────────
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        // Игнорируем себя и союзников
        if (other.CompareTag("BossProjectile") || other.CompareTag("Enemy")) return;

        // Пуля игрока попала в ракету — снимаем HP
        if (other.CompareTag("Player_Projectile"))
        {
            Destroy(other.gameObject);
            rocketHP--;
            if (rocketHP <= 0)
                DestroyRocket();
            return;
        }

        // Попали в игрока или препятствие — взрыв
        Explode();
    }

    // Уничтожена выстрелом — со взрывом, но без урона игроку
    private void DestroyRocket()
    {
        if (isDead) return;
        isDead = true;
        CancelInvoke();
        StopAllCoroutines();
        rb.linearVelocity = Vector2.zero;

        // Взрыв (тот же префаб что и при обычном попадании)
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Дополнительный VFX сбития если задан
        if (destroyVFX != null)
            Instantiate(destroyVFX, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    // Взрыв при попадании в цель или препятствие
    private void Explode()
    {
        if (isDead) return;
        isDead = true;
        CancelInvoke();
        StopAllCoroutines();
        rb.linearVelocity = Vector2.zero;

        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Урон напрямую через transform игрока — никаких тегов и слоёв
        if (target != null)
            target.GetComponent<PlayerHealth>()?.TakeDamage(damage);

        Destroy(gameObject);
    }

    private void SelfDestruct()
    {
        // Время жизни вышло — просто исчезает без урона
        if (!isDead)
        {
            isDead = true;
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}