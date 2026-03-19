using UnityEngine;

public class PlayerMaterials : MonoBehaviour
{
    public Material bodyMaterial;
    private bool isHighlightBodyActive;
    private float highlightBodyIntensity;
    private float HighlightBodyTime;
    private float highlightBodyDuration;

    private void Update()
    {
        if (isHighlightBodyActive)
        {
            HighlightBodyTime -= Time.deltaTime;

            bodyMaterial.SetFloat("_ColorIntensity", (HighlightBodyTime / highlightBodyDuration) + highlightBodyIntensity);

            if (HighlightBodyTime <= 0)
            {
                bodyMaterial.SetFloat("_ColorIntensity", 1f);

                isHighlightBodyActive = false;
            }
        }
    }
    public void SetActiveHighlightBody(float duration, float intensity)
    {
        isHighlightBodyActive = true;
        highlightBodyIntensity = intensity;
        HighlightBodyTime = duration;
        highlightBodyDuration = duration;
    }
}
