using UnityEngine;


public class PlayerShoulderSecondary : MonoBehaviour
{
    
    

    [SerializeField] private float rate = 60f;
    [SerializeField] private float minOffsetAngle = 0f;
    [SerializeField] private float maxOffsetAngle = 50f;
    public Transform shoulderMain;

    [Tooltip("Check to let this behaviour be run by the local Update() method and " +
        "Uncheck if you want to call it from any other class by using UpdateLookAt().")]
    [SerializeField] private bool isUpdateCalledLocally = false;

    private float clampAngle;

    private void Update()
    {
        if (!isUpdateCalledLocally)
            return;
        UpdateRotation();
    }

    public void UpdateRotation()
    {
        // I got this hardcoded values after testing them in the Inspector.
        if (PlayerBodyPartsHandler.isRightDirection)
        {
            minOffsetAngle = 0f;
            maxOffsetAngle = 50f;
        }
        else
        {
            minOffsetAngle = -50f;
            maxOffsetAngle = -50f;
        }

        if (TadaInput.IsMouseActive)
            clampAngle = Mathf.Clamp(CrosshairMouse.AimDirection.y * rate, minOffsetAngle, maxOffsetAngle);
        else
            clampAngle = Mathf.Clamp(CrosshairJoystick.AimDirection.y * rate, minOffsetAngle, maxOffsetAngle);

        transform.rotation = shoulderMain.rotation;
        transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + clampAngle);
    }
}
