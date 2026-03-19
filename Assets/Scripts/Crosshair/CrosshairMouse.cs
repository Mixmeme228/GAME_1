using UnityEngine;


public class CrosshairMouse : Crosshair
{
    private Transform player;

    public static Vector3 AimDirection
    {
        get { return _AimDirection; }
        private set { _AimDirection = value; }
    }
    private static Vector3 _AimDirection;

    protected override void Awake()
    {
        base.Awake();
        player = FindObjectOfType<PlayerController>().transform;
    }

    public override void UpdateCrosshair()
    {
        base.UpdateCrosshair();
        crosshair.transform.position = TadaInput.MouseWorldPos;

        _AimDirection = (crosshair.transform.position - player.position).normalized;
        _AimDirection.z = 0f;
    }
}
