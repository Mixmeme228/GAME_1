using UnityEngine;

/// <summary>
/// Считает врагов в триггере — когда все убиты, открывает дверь,
/// меняет спрайт кнопки и активирует AI других врагов.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DoorEnemyTrigger : MonoBehaviour
{
    [Header("Дверь")]
    [SerializeField] private Animator _doorAnimator;

    [Header("Кнопка-индикатор")]
    [SerializeField] private SpriteRenderer _indicatorSprite;
    public Sprite spriteEnemiesAlive;
    public Sprite spriteEnemiesDead;

    [Header("Настройки")]
    [SerializeField] private string enemyTag = "Enemy";

    [Header("Активация врагов при открытии двери")]
    [Tooltip("Зомби которые активируются когда дверь открывается")]
    public ZombieAI[] zombiesToActivate;
    [Tooltip("Роботы которые активируются когда дверь открывается")]
    public Robot[] robotsToActivate;
    [Tooltip("Боссы которые активируются когда дверь открывается")]
    public BossAI[] bossesToActivate;

    // ─── Приватные ────────────────────────────────────────────────────────
    private int _enemyCount = 0;
    private bool _doorOpened = false;

    // =========================================================================
    void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;

        if (_indicatorSprite != null)
            _indicatorSprite.sprite = spriteEnemiesAlive;

        // Замораживаем всех врагов до активации
        foreach (var z in zombiesToActivate) if (z != null) z.aiEnabled = false;
        foreach (var r in robotsToActivate) if (r != null) r.aiEnabled = false;
        foreach (var b in bossesToActivate) if (b != null) b.SetAIEnabled(false);
    }

    // =========================================================================
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(enemyTag)) return;
        _enemyCount++;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(enemyTag)) return;
        _enemyCount = Mathf.Max(0, _enemyCount - 1);
        TryOpenDoor();
    }

    // =========================================================================
    private void TryOpenDoor()
    {
        if (_doorOpened || _enemyCount > 0) return;
        _doorOpened = true;

        _doorAnimator?.SetBool("Open", true);

        if (_indicatorSprite != null)
            _indicatorSprite.sprite = spriteEnemiesDead;

        ActivateEnemies();
        Debug.Log("[Door] Все враги убиты — дверь открыта, новые враги активированы!");
    }

    private void ActivateEnemies()
    {
        foreach (var z in zombiesToActivate)
        {
            if (z == null) continue;
            z.aiEnabled = true;
            z.SetState(ZombieAI.ZombieState.Idle);
            Debug.Log($"[Door] Активирован зомби: {z.name}");
        }

        foreach (var r in robotsToActivate)
        {
            if (r == null) continue;
            r.aiEnabled = true;
            r.SetState(Robot.RobotState.Idle);
            Debug.Log($"[Door] Активирован робот: {r.name}");
        }

        foreach (var b in bossesToActivate)
        {
            if (b == null) continue;
            b.SetAIEnabled(true);
            Debug.Log($"[Door] Активирован босс: {b.name}");
        }
    }

    // =========================================================================
    public void ResetDoor()
    {
        _doorOpened = false;
        _enemyCount = 0;

        _doorAnimator?.SetBool("Open", false);

        if (_indicatorSprite != null)
            _indicatorSprite.sprite = spriteEnemiesAlive;

        foreach (var z in zombiesToActivate) if (z != null) z.aiEnabled = false;
        foreach (var r in robotsToActivate) if (r != null) r.aiEnabled = false;
        foreach (var b in bossesToActivate) if (b != null) b.SetAIEnabled(false);
    }
}