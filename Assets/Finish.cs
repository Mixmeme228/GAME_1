using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Повесь на любой объект на сцене.
/// Как только BossAI2 умирает — переход на следующий уровень.
/// </summary>
public class LevelTransitionTrigger : MonoBehaviour
{
    [Header("Transition")]
    [Tooltip("Индекс сцены. -1 = следующая по порядку")]
    public int targetSceneIndex = -1;
    [Tooltip("Задержка перед загрузкой")]
    public float transitionDelay = 2f;

    [Header("Visual")]
    public GameObject openIndicator;

    private BossAI _boss1;
    private BossAI2 _boss2;
    private bool _transitioning = false;

    // ══════════════════════════════════════════════════════════════════════
    private void Start()
    {
        _boss1 = FindObjectOfType<BossAI>();
        _boss2 = FindObjectOfType<BossAI2>();

        if (_boss1 == null && _boss2 == null)
            Debug.LogWarning("[LevelTransition] Ни BossAI ни BossAI2 не найдены на сцене!");

        if (openIndicator != null) openIndicator.SetActive(false);
    }

    private void Update()
    {
        if (_transitioning) return;

        bool boss1Dead = _boss1 == null || _boss1.IsDead;
        bool boss2Dead = _boss2 == null || _boss2.IsDead;

        // Переход если хотя бы один босс был на сцене и он мёртв
        bool hasBoss = _boss1 != null || _boss2 != null;
        if (hasBoss && boss1Dead && boss2Dead)
            StartTransition();
    }

    // ══════════════════════════════════════════════════════════════════════
    private void StartTransition()
    {
        if (_transitioning) return;
        _transitioning = true;

        if (openIndicator != null) openIndicator.SetActive(true);
        Debug.Log("[LevelTransition] Босс убит — переход!");
        StartCoroutine(TransitionRoutine());
    }

    private IEnumerator TransitionRoutine()
    {
        yield return new WaitForSeconds(transitionDelay);

        int next = targetSceneIndex >= 0
            ? targetSceneIndex
            : SceneManager.GetActiveScene().buildIndex + 1;

        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            Debug.LogWarning("[LevelTransition] Следующей сцены нет в Build Settings!");
    }
}