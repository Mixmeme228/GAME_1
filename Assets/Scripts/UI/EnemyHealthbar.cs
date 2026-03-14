using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    public Robot robot;

    [Header("Segments")]
    public Sprite segmentSprite;
    public int totalSegments = 10;
    public float segmentWidth = 0.15f;
    public float segmentSpacing = 0.05f;

    private SpriteRenderer[] segments;
    private int lastShown = -1;

    void Start()
    {
        BuildSegments();
        Invoke(nameof(DelayedRefresh), 0.1f);
    }

    void DelayedRefresh()
    {
        lastShown = -1; // сбросить кэш
        if (robot != null)
            Refresh(robot.HealthPercent);
    }

    void Update()
    {
        if (robot == null) return;

       
        Refresh(robot.HealthPercent);
    }

    void BuildSegments()
    {
        segments = new SpriteRenderer[totalSegments];

        float step = segmentWidth + segmentSpacing;
        float startX = -step * (totalSegments - 1) / 2f;

        for (int i = 0; i < totalSegments; i++)
        {
            GameObject seg = new GameObject($"seg_{i}");
            seg.transform.SetParent(transform);
            seg.transform.localPosition = new Vector3(startX + i * step, 0f, 0f);
            seg.transform.localScale = Vector3.one;

            SpriteRenderer sr = seg.AddComponent<SpriteRenderer>();
            sr.sprite = segmentSprite;
            sr.sortingOrder = 21;
            segments[i] = sr;
        }
    }

    void Refresh(float percent)
    {
        int toShow = robot.IsDead ? 0 : Mathf.RoundToInt(percent * totalSegments);
        if (toShow == lastShown) return;
        lastShown = toShow;

        for (int i = 0; i < segments.Length; i++)
            segments[i].gameObject.SetActive(i < toShow);
    }
}