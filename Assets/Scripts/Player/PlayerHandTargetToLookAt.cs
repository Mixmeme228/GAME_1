using UnityEngine;


public class PlayerHandTargetToLookAt : MonoBehaviour
{
    
    
    

    

    [SerializeField] private float lerpSpeed = 2f;
    [SerializeField] private Vector2 sensitivity = new Vector2(0.1f, 1f);
    [SerializeField] private Vector2 initialOffset = new Vector2(0.25f, 0.18f);
    [SerializeField] private Vector2 minMaxOffsetX = new Vector2 (-1f, 0.3f);
    [SerializeField] private Vector2 minMaxOffsetY = new Vector2(0.18f, 1f);

    private Vector2 aimDirection;

    private void Update()
    {
        float clampX = 0.0f;
        float clampY = 0.0f;

        if (TadaInput.IsMouseActive)
            aimDirection = CrosshairMouse.AimDirection;
        else
            aimDirection = CrosshairJoystick.AimDirection;

        if (aimDirection.y > 0)
            clampX = Mathf.Clamp(initialOffset.x - aimDirection.y * sensitivity.x, minMaxOffsetX.x, minMaxOffsetX.y);
        else if (aimDirection.y < 0)
            clampX = Mathf.Clamp(initialOffset.x + aimDirection.y * sensitivity.x, minMaxOffsetX.x, minMaxOffsetX.y);

        clampY = Mathf.Clamp(initialOffset.y - aimDirection.y * sensitivity.y, minMaxOffsetY.x, minMaxOffsetY.y);

        transform.localPosition = Vector2.Lerp(transform.localPosition, new Vector2 (clampX, clampY), Time.deltaTime * lerpSpeed);
    }
}
