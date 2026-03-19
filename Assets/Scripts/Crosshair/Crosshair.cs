using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [Tooltip("The gameobject that will be used to visually represent the crosshair.")]
    public GameObject crosshair;

    protected virtual void Awake()
    {
        if (crosshair == null)
        {
            Debug.LogError(gameObject.name + ": Missing crosshair!");
            Debug.Break();
        }
    }

    public virtual void UpdateCrosshair() { }

    public bool IsActive
    {
        get { return _IsActive; }
        set { _IsActive = value; crosshair.SetActive(value); }
    }
    private bool _IsActive;
}
