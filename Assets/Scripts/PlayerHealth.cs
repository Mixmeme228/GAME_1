using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// HP игрока — такая же система как у Robot и ZombieAI.
/// Повесь на корневой GameObject игрока с тегом Player_1.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;

    [Tooltip("Слайдер HP-бара (необязательно)")]
    public Slider healthBarSlider;

    [Tooltip("Эффект при получении урона (необязательно)")]
    public GameObject hitFX;

    [Tooltip("Эффект при смерти (необязательно)")]
    public GameObject deathFX;

    [Tooltip("Секунд неуязвимости после получения урона")]
    public float invincibleTime = 0.5f;

    [Tooltip("Задержка перед перезапуском сцены")]
    public float restartDelay = 1.5f;

    public float CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }
    public float HealthPercent => CurrentHealth / maxHealth;

    // ─── Приватные ───────────────────────────────────────────────────
    private float invincibleTimer = 0f;
    private SpriteRenderer sr;
    private SoundHandlerLocal sfx;

    // ═══════════════════════════════════════════════════════════════════
    private void Awake()
    {
        TryGetComponent(out sr);
        TryGetComponent(out sfx);
    }

    private void Start()
    {
        CurrentHealth = maxHealth;
        IsDead = false;
        RefreshHealthBar();
    }

    private void Update()
    {
        if (invincibleTimer > 0f)
            invincibleTimer -= Time.deltaTime;
    }

    // ═══════════════════════════════════════════════════════════════════
    //  УРОН
    // ═══════════════════════════════════════════════════════════════════
    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        if (invincibleTimer > 0f) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        invincibleTimer = invincibleTime;

        RefreshHealthBar();

        if (hitFX != null)
            Instantiate(hitFX, transform.position, Quaternion.identity);

        sfx?.PlaySound(0);

        StartCoroutine(HurtFlash());

        if (CurrentHealth <= 0f)
            Die();
    }

    // ═══════════════════════════════════════════════════════════════════
    //  ЛЕЧЕНИЕ
    // ═══════════════════════════════════════════════════════════════════
    public void Heal(float amount)
    {
        if (IsDead) return;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        RefreshHealthBar();
    }

    // ═══════════════════════════════════════════════════════════════════
    //  СМЕРТЬ → ПЕРЕЗАПУСК СЦЕНЫ
    // ═══════════════════════════════════════════════════════════════════
    public void Die()
    {
        if (IsDead) return;
        IsDead = true;
        CurrentHealth = 0f;
        RefreshHealthBar();

        if (deathFX != null)
            Instantiate(deathFX, transform.position, Quaternion.identity);

        Debug.Log("[PlayerHealth] Игрок погиб — перезапуск сцены...");
        StartCoroutine(RestartScene());
    }

    private IEnumerator RestartScene()
    {
        yield return new WaitForSeconds(restartDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ═══════════════════════════════════════════════════════════════════
    private void RefreshHealthBar()
    {
        if (healthBarSlider != null)
            healthBarSlider.value = CurrentHealth / maxHealth;
    }

    private IEnumerator HurtFlash()
    {
        if (sr) sr.color = new Color(1f, 0.3f, 0.3f);
        yield return new WaitForSeconds(0.1f);
        if (sr) sr.color = Color.white;
    }
}