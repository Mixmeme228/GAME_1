using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool requireKey = true;
    [SerializeField] private float interactRadius = 1.5f;

    [Header("On Open")]
    [SerializeField] private GameObject objectToActivate;
    [Tooltip("Зомби которые активируются когда дверь открывается")]
    public ZombieAI[] zombiesToActivate;
    [Tooltip("Роботы которые активируются когда дверь открывается")]
    public Robot[] robotsToActivate;
    [Tooltip("Боссы которые активируются когда дверь открывается")]
    public BossAI[] bossesToActivate;

    [Header("Prompt")]
    [SerializeField] private TextMesh promptText;
    [SerializeField] private string pressEText = "Press E";
    [SerializeField] private string noKeyText = "Need a key!";
    [SerializeField] private Color pressEColor = Color.white;
    [SerializeField] private Color noKeyColor = Color.red;

    private Animator _animator;
    private bool _isOpen;
    private bool _canInteract;
    private Transform _player;
    private PlayerHealth _playerHealth;

    private static readonly int AnimOpen = Animator.StringToHash("Open");

    // ──────────────────────────────────────────────────────────────────────
    private void Awake()
    {
        TryGetComponent(out _animator);

        // Выключаем в Awake — раньше чем Start любого врага
        if (objectToActivate != null)
            objectToActivate.SetActive(false);

        foreach (var z in zombiesToActivate) if (z != null) z.gameObject.SetActive(false);
        foreach (var r in robotsToActivate) if (r != null) r.gameObject.SetActive(false);
        foreach (var b in bossesToActivate) if (b != null) b.gameObject.SetActive(false);
    }

    private void Start()
    {
        PlayerHealth ph = FindObjectOfType<PlayerHealth>();
        if (ph != null)
        {
            _playerHealth = ph;
            _player = ph.transform;
        }
        else
            Debug.LogWarning("[Door] PlayerHealth не найден на сцене!");

        HidePrompt();
    }

    private void Update()
    {
        if (_isOpen || _player == null) return;

        float dist = Vector2.Distance(transform.position, _player.position);
        bool inRange = dist <= interactRadius;

        if (inRange)
        {
            _canInteract = true;
            UpdatePrompt();
        }
        else
        {
            if (_canInteract)
            {
                _canInteract = false;
                HidePrompt();
            }
        }

        // ── Взаимодействие по E ────────────────────────────────────────────
        if (_canInteract && TadaInput.GetKeyDown(TadaInput.ThisKey.Interact))
        {
            if (requireKey && !_playerHealth.HasKey())
            {
                Debug.Log("[Door] Нужен ключ!");
                return;
            }

            OpenDoor();
        }
    }

    // ──────────────────────────────────────────────────────────────────────
    private void OpenDoor()
    {
        _isOpen = true;

        if (requireKey)
        {
            bool used = _playerHealth.UseKey();
            if (!used)
            {
                _isOpen = false;
                return;
            }
        }

        _animator.SetBool(AnimOpen, true);
        HidePrompt();

        // ── Активация объекта ──────────────────────────────────────────────
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
            Debug.Log("[Door] Объект активирован: " + objectToActivate.name);
        }

        // ── Активация врагов ───────────────────────────────────────────────
        ActivateEnemies();

        Debug.Log("[Door] Дверь открыта! Ключей осталось: " + _playerHealth.KeyCount);
    }

    // ──────────────────────────────────────────────────────────────────────
    private void ActivateEnemies()
    {
        Debug.Log($"[Door] Активация: зомби={zombiesToActivate.Length} роботы={robotsToActivate.Length} боссы={bossesToActivate.Length}");

        foreach (var z in zombiesToActivate)
        {
            if (z == null) continue;
            z.gameObject.SetActive(true);
            z.aiEnabled = true;
            StartCoroutine(DelaySetZombieState(z));
            Debug.Log($"[Door] Активирован зомби: {z.name}");
        }

        foreach (var r in robotsToActivate)
        {
            if (r == null) continue;
            r.gameObject.SetActive(true);
            r.aiEnabled = true;
            StartCoroutine(DelaySetRobotState(r));
            Debug.Log($"[Door] Активирован робот: {r.name}");
        }

        foreach (var b in bossesToActivate)
        {
            if (b == null) continue;
            b.gameObject.SetActive(true);
            b.SetAIEnabled(true);
            Debug.Log($"[Door] Активирован босс: {b.name}");
        }
    }

    // Ждём один кадр чтобы Awake/Start врага успел отработать
    private IEnumerator DelaySetZombieState(ZombieAI z)
    {
        yield return null;
        if (z != null)
            z.SetState(ZombieAI.ZombieState.Idle);
    }

    private IEnumerator DelaySetRobotState(Robot r)
    {
        yield return null;
        if (r != null)
            r.SetState(Robot.RobotState.Idle);
    }

    // ── Подсказка ─────────────────────────────────────────────────────────
    private void UpdatePrompt()
    {
        if (promptText == null) return;

        promptText.gameObject.SetActive(true);

        bool hasKey = !requireKey || (_playerHealth != null && _playerHealth.HasKey());

        if (hasKey)
        {
            promptText.text = pressEText;
            promptText.color = pressEColor;
        }
        else
        {
            promptText.text = noKeyText;
            promptText.color = noKeyColor;
        }
    }

    private void HidePrompt()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    // ── Гизмо ─────────────────────────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, interactRadius);
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.9f);
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}