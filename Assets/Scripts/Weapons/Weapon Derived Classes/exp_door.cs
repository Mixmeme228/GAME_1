using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Повесь на объект двери.
/// При длительном попадании лазера — дверь нагревается и уничтожается.
/// </summary>
public class LaserDoor : MonoBehaviour
{
    [Header("Overheat")]
    [Tooltip("Секунд непрерывного попадания лазера до уничтожения")]
    public float maxHeat = 3f;
    [Tooltip("Скорость остывания в секунду когда лазер не бьёт")]
    public float cooldownRate = 1f;
    [Tooltip("Задержка остывания после прекращения попадания")]
    public float cooldownDelay = 0.5f;

    [Header("Visual")]
    [Tooltip("SpriteRenderer двери — будет менять цвет при нагреве")]
    public SpriteRenderer doorSprite;
    [Tooltip("Цвет нормальной двери")]
    public Color normalColor = Color.white;
    [Tooltip("Цвет раскалённой двери")]
    public Color heatedColor = Color.red;
    [Tooltip("Слайдер нагрева (необязательно)")]
    public Slider heatBar;

    [Header("Destruction")]
    [Tooltip("Эффект при уничтожении (необязательно)")]
    public GameObject destroyFX;
    [Tooltip("Задержка перед уничтожением объекта")]
    public float destroyDelay = 0.2f;

    // ── Приватные ──────────────────────────────────────────────────────────
    private float _currentHeat = 0f;
    private float _cooldownTimer = 0f;
    private bool _isHit = false;
    private bool _isDestroyed = false;

    // ══════════════════════════════════════════════════════════════════════
    private void Awake()
    {
        if (doorSprite == null)
            doorSprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_isDestroyed) return;

        if (_isHit)
        {
            // Нагрев
            _cooldownTimer = cooldownDelay;
            _currentHeat += Time.deltaTime;

            if (_currentHeat >= maxHeat)
            {
                DestroyDoor();
                return;
            }
        }
        else
        {
            // Остывание с задержкой
            if (_cooldownTimer > 0f)
                _cooldownTimer -= Time.deltaTime;
            else
                _currentHeat = Mathf.Max(0f, _currentHeat - cooldownRate * Time.deltaTime);
        }

        // Сбрасываем флаг — он выставляется каждый кадр из Weapon_LaserBeam
        _isHit = false;

        RefreshVisuals();
    }

    // ══════════════════════════════════════════════════════════════════════
    /// <summary>
    /// Вызывай из Weapon_LaserBeam в методе ApplyDamage каждый кадр пока луч бьёт по двери.
    /// </summary>
    public void HeatUp()
    {
        if (_isDestroyed) return;
        _isHit = true;
    }

    // ══════════════════════════════════════════════════════════════════════
    private void DestroyDoor()
    {
        _isDestroyed = true;

        if (doorSprite != null)
            doorSprite.color = heatedColor;

        if (heatBar != null)
            heatBar.value = 1f;

        if (destroyFX != null)
            Instantiate(destroyFX, transform.position, Quaternion.identity);

        // Отключаем коллайдер сразу — чтобы игрок мог пройти
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Debug.Log($"[LaserDoor] {gameObject.name} уничтожена лазером!");

        Destroy(gameObject, destroyDelay);
    }

    // ══════════════════════════════════════════════════════════════════════
    private void RefreshVisuals()
    {
        float t = _currentHeat / maxHeat;

        if (doorSprite != null)
            doorSprite.color = Color.Lerp(normalColor, heatedColor, t);

        if (heatBar != null)
            heatBar.value = t;
    }

    public float HeatPercent => _currentHeat / maxHeat;
}