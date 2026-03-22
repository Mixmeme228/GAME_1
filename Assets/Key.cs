using UnityEngine;

public class Key : MonoBehaviour
{
    [Header("Highlight")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = new Color(1f, 0.85f, 0.2f);
    [SerializeField] private float highlightScale = 1.15f;
    [SerializeField] private float highlightRadius = 2f;

    [Header("Pickup")]
    [SerializeField] private float pickupRadius = 1.5f;

    [Header("Prompt")]
    [SerializeField] private TextMesh promptText;
    [SerializeField] private string pickupText = "Press E";
    [SerializeField] private Color pickupColor = Color.white;

    [Header("FX")]
    [SerializeField] private GameObject pickupFX;

    private SpriteRenderer _sr;
    private Vector3 _originalScale;
    private bool _collected;
    private bool _isHighlighted;
    private bool _canPickup;
    private Transform _player;
    private PlayerHealth _playerHealth;

    private void Awake()
    {
        TryGetComponent(out _sr);
        _originalScale = transform.localScale;
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
            Debug.LogWarning("[Key] PlayerHealth не найден на сцене!");

        HidePrompt();
    }

    private void Update()
    {
        if (_collected || _player == null) return;

        float dist = Vector2.Distance(transform.position, _player.position);

        // ── Подсветка ──────────────────────────────────────────────────────
        if (dist <= highlightRadius && !_isHighlighted)
            SetHighlight(true);
        else if (dist > highlightRadius && _isHighlighted)
            SetHighlight(false);

        // ── Зона подбора + подсказка ───────────────────────────────────────
        bool inRange = dist <= pickupRadius;
        if (inRange != _canPickup)
        {
            _canPickup = inRange;
            if (_canPickup)
                ShowPrompt();
            else
                HidePrompt();
        }

        // ── Подбор по E через TadaInput ────────────────────────────────────
        if (_canPickup && TadaInput.GetKeyDown(TadaInput.ThisKey.Interact))
            Pickup();
    }

    private void SetHighlight(bool on)
    {
        _isHighlighted = on;
        if (_sr) _sr.color = on ? highlightColor : normalColor;
        transform.localScale = on ? _originalScale * highlightScale : _originalScale;
    }

    // ── Подсказка ─────────────────────────────────────────────────────────
    private void ShowPrompt()
    {
        if (promptText == null) return;
        promptText.gameObject.SetActive(true);
        promptText.text = pickupText;
        promptText.color = pickupColor;
    }

    private void HidePrompt()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    // ──────────────────────────────────────────────────────────────────────
    private void Pickup()
    {
        _collected = true;
        _playerHealth.AddKey();

        if (pickupFX != null)
            Instantiate(pickupFX, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    // ── Гизмо ─────────────────────────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.85f, 0.2f, 0.3f);
        Gizmos.DrawSphere(transform.position, highlightRadius);
        Gizmos.color = new Color(1f, 0.85f, 0.2f, 0.8f);
        Gizmos.DrawWireSphere(transform.position, highlightRadius);

        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, pickupRadius);
        Gizmos.color = new Color(0f, 1f, 0f, 0.8f);
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}