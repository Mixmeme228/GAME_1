using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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
    [Tooltip("Объект который удалится при смерти игрока (перетащи сюда нужный)")]
    public GameObject objectToDestroyOnDeath;
    [Tooltip("Секунд неуязвимости после получения урона")]
    public float invincibleTime = 0.5f;
    [Tooltip("Задержка перед перезапуском сцены")]
    public float restartDelay = 1.5f;

    [Header("Keys")]
    [Tooltip("UI-текст для отображения кол-ва ключей (необязательно)")]
    public Text keyCountText;

    public float CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }
    public float HealthPercent => CurrentHealth / maxHealth;

    // ── Ключи ──────────────────────────────────────────────────────────────
    public int KeyCount { get; private set; }

    private float invincibleTimer = 0f;
    private SpriteRenderer sr;
    private SoundHandlerLocal sfx;

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
        RefreshKeyUI();
    }

    private void Update()
    {
        if (invincibleTimer > 0f)
            invincibleTimer -= Time.deltaTime;
    }

    // ── Здоровье ───────────────────────────────────────────────────────────
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

    public void Heal(float amount)
    {
        if (IsDead) return;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        RefreshHealthBar();
    }

    public void Die()
    {
        if (IsDead) return;
        IsDead = true;
        CurrentHealth = 0f;
        RefreshHealthBar();

        if (deathFX != null)
            Instantiate(deathFX, transform.position, Quaternion.identity);

        // Удаляем нужный объект при смерти
        if (objectToDestroyOnDeath != null)
            Destroy(objectToDestroyOnDeath);

        Debug.Log("[PlayerHealth] Игрок погиб — перезапуск сцены...");
        StartCoroutine(RestartScene());
    }

    // ── Неуязвимость ───────────────────────────────────────────────────────
    public void SetInvincible(float duration)
    {
        // duration = 0 — принудительный сброс неуязвимости
        if (duration <= 0f)
            invincibleTimer = 0f;
        else
            invincibleTimer = Mathf.Max(invincibleTimer, duration);
    }

    // ── Ключи ──────────────────────────────────────────────────────────────
    public void AddKey()
    {
        KeyCount++;
        Debug.Log($"[PlayerHealth] Подобран ключ! Всего ключей: {KeyCount}");
        RefreshKeyUI();
    }

    public bool UseKey()
    {
        if (KeyCount <= 0) return false;
        KeyCount--;
        RefreshKeyUI();
        return true;
    }

    public bool HasKey() => KeyCount > 0;

    // ── Вспомогательное ───────────────────────────────────────────────────
    private IEnumerator RestartScene()
    {
        yield return new WaitForSeconds(restartDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void RefreshHealthBar()
    {
        if (healthBarSlider != null)
            healthBarSlider.value = CurrentHealth / maxHealth;
    }

    private void RefreshKeyUI()
    {
        if (keyCountText != null)
            keyCountText.text = $"Keys: {KeyCount}";
    }

    private IEnumerator HurtFlash()
    {
        if (sr) sr.color = new Color(1f, 0.3f, 0.3f);
        yield return new WaitForSeconds(0.1f);
        if (sr) sr.color = Color.white;
    }
}