using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Повесь на любой объект. Вызывай LoadScene() из кнопки или другого скрипта.
/// </summary>
public class SceneTransition : MonoBehaviour
{
    [Header("Scene")]
    [Tooltip("Индекс сцены. -1 = следующая по порядку")]
    public int targetSceneIndex = -1;
    [Tooltip("Задержка перед загрузкой")]
    public float transitionDelay = 0f;

    // ══════════════════════════════════════════════════════════════════════
    /// <summary>Вызови из кнопки (OnClick) или из любого скрипта.</summary>
    public void LoadScene()
    {
        StartCoroutine(LoadRoutine());
    }

    /// <summary>Загрузить конкретную сцену по индексу.</summary>
    public void LoadSceneByIndex(int index)
    {
        targetSceneIndex = index;
        StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()
    {
        if (transitionDelay > 0f)
            yield return new WaitForSeconds(transitionDelay);

        int index = targetSceneIndex >= 0
            ? targetSceneIndex
            : SceneManager.GetActiveScene().buildIndex + 1;

        if (index < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(index);
        else
            Debug.LogWarning($"[SceneTransition] Сцена {index} не найдена в Build Settings!");
    }
}