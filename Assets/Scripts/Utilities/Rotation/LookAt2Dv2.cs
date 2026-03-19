using UnityEngine;

public class LookAt2Dv2 : MonoBehaviour
{
    public enum LookAtTarget { TargetTransform, MouseWorldPosition }
    [SerializeField] private LookAtTarget lookAtTarget = LookAtTarget.TargetTransform;

    public Transform targetTransform;

    private enum Axis { X, Y }
    [SerializeField] private Axis axis = Axis.Y;

    [SerializeField] private float turnRate = 10f;

    [SerializeField] private float offsetLookAtAngle = 0f;

    [SerializeField] private float maxAngle = 360f;

    [SerializeField] private bool isUpdateCalledLocally = false;

    public bool isSmoothRotationEnable = false;

    public bool isFlipAxis = false;

    [Header("Debug")]
    [SerializeField] private Color debugColor = Color.green;
    [SerializeField] private bool debug = false;

    private Vector3 targetPosition;
    private Vector3 direction;
    private Vector3 upwardAxis; 

    private void Update()
    {
        if (!isUpdateCalledLocally)
            return;
        UpdateLookAt();
    }

    public void UpdateLookAt()
    {
        Vector3 myPosition = transform.position;

        if (lookAtTarget == LookAtTarget.MouseWorldPosition)
            targetPosition = TadaInput.MouseWorldPos;
        else if ((lookAtTarget == LookAtTarget.TargetTransform))
        {
            if (targetTransform == null)
            {
                Debug.LogError(gameObject.name + " target missing!");
                return;
            }
            targetPosition = targetTransform.position;
        }

        targetPosition.z = myPosition.z;

        direction = (targetPosition - myPosition).normalized;

        switch (axis)
        {
            case Axis.X:

                if (!isFlipAxis)
                {
                    upwardAxis = Quaternion.Euler(0, 0, 90 + offsetLookAtAngle) * direction;
                }
                else
                {
                    upwardAxis = Quaternion.Euler(0, 0, -90 + offsetLookAtAngle) * direction;
                }
                break;

            case Axis.Y:

                if (!isFlipAxis)
                    upwardAxis = direction;
                else
                    upwardAxis = -direction;
                break;

            default:
                break;
        }

        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: upwardAxis);

        if (debug)
            Debug.DrawLine(transform.position, targetPosition, debugColor);

        if (!isSmoothRotationEnable)
        {
            if (Quaternion.Angle(Quaternion.identity, targetRotation) < maxAngle)
                transform.rotation = targetRotation;
            return;
        }

        Quaternion rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnRate * Time.deltaTime);

        if (Quaternion.Angle(Quaternion.identity, rotation) < maxAngle)
            transform.rotation = rotation;
    }

    public void SwitchToTarget(LookAtTarget target)
    {
        lookAtTarget = target;
    }

    public void SetOffsetAngle(float value)
    {
        offsetLookAtAngle = value;
    }
}
