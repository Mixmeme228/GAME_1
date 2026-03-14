using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HP-бар игрока для Canvas.
/// Повесь на пустой GameObject внутри Canvas.
/// Сегменты создаются автоматически как дочерние Image.
/// </summary>
public class PlayerHealthBar : MonoBehaviour
{
    public PlayerHealth player;

    [Header("Segments")]
    public Sprite segmentSprite;
    public int totalSegments = 10;
    public float segmentWidth = 20f;
    public float segmentHeight = 8f;
    public float segmentSpacing = 3f;

    [Header("Цвета")]
    public Color colorFull = Color.green;
    public Color colorEmpty = new Color(0.05f, 0.2f, 0.05f, 0.6f);

    private Image[] segments;
    private int lastShown = -1;

    // ═══════════════════════════════════════════════
    void Start()
    {
        BuildSegments();
        Invoke(nameof(DelayedRefresh), 0.1f);
    }

    void DelayedRefresh()
    {
        lastShown = -1;
        if (player != null) Refresh(player.HealthPercent);
    }

    void Update()
    {
        if (player == null) return;
        Refresh(player.HealthPercent);
    }

    // ═══════════════════════════════════════════════
    void BuildSegments()
    {
        segments = new Image[totalSegments];
        float step = segmentWidth + segmentSpacing;
        float startX = -step * (totalSegments - 1) / 2f;

        for (int i = 0; i < totalSegments; i++)
        {
            GameObject seg = new GameObject($"seg_{i}");
            seg.transform.SetParent(transform, false);

            RectTransform rt = seg.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(segmentWidth, segmentHeight);
            rt.anchoredPosition = new Vector2(startX + i * step, 0f);

            Image img = seg.AddComponent<Image>();
            img.sprite = segmentSprite;
            img.color = colorEmpty;
            img.type = Image.Type.Simple;
            img.preserveAspect = false;

            segments[i] = img;
        }
    }

    void Refresh(float percent)
    {
        bool isDead = player.IsDead;
        int toShow = isDead ? 0 : Mathf.RoundToInt(percent * totalSegments);

        if (toShow == lastShown) return;
        lastShown = toShow;

        for (int i = 0; i < segments.Length; i++)
            segments[i].color = (i < toShow) ? colorFull : colorEmpty;
    }
}