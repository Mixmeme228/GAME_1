using UnityEngine;

public class PlayerBodyPartsHandler : MonoBehaviour
{
    [Header("Body parts")]
    public SpriteRenderer[] handRenderers;
    public SpriteRenderer[] headRenderers;
    [Space(5)]

    public GameObject hips;

    public GameObject upperBody;

    private LookAt2Dv2Updater lookAtUpdater;
    private LookAt2Dv2Handler lookAtHandler;
    private PlayerShoulderSecondary shoulderSecondary;

    public static bool isRightDirection;

    private bool isHandAndHeadOnFront;
    private bool isLookAtMouse;

    private enum Direction { Left, Right }

    private void Awake()
    {
        isRightDirection = true;
        TryGetComponent(out lookAtUpdater);
        TryGetComponent(out lookAtHandler);
        shoulderSecondary = FindObjectOfType<PlayerShoulderSecondary>();
    }

    private void Update()
    {
        CheckIfMissingClasses();

        if (PauseController.isGamePaused)
            return;

        lookAtUpdater.UpdateLookAtClasses();

        shoulderSecondary.UpdateRotation();

        UpdateLookAtTarget();
        UpdateRenderersLayerOrder();
        UpdateBodyPartsDirection();
    }

    private void UpdateLookAtTarget()
    {
        if (TadaInput.IsMouseActive && !isLookAtMouse)
        {
            isLookAtMouse = true;
            lookAtHandler.SwitchToTarget(LookAt2Dv2.LookAtTarget.MouseWorldPosition);
        }
        if (!TadaInput.IsMouseActive && isLookAtMouse)
        {
            isLookAtMouse = false;
            lookAtHandler.SwitchToTarget(LookAt2Dv2.LookAtTarget.TargetTransform);
        }           
    }

    private void UpdateRenderersLayerOrder()
    {
        if ((CrosshairMouse.AimDirection.y < 0f && TadaInput.IsMouseActive) && isHandAndHeadOnFront)
        {
            SetRenderersLayerOrder(handRenderers, 9);
            SetRenderersLayerOrder(headRenderers, 13);
            isHandAndHeadOnFront = false;
        }
        else if ((CrosshairMouse.AimDirection.y > 0f && TadaInput.IsMouseActive) && !isHandAndHeadOnFront )
        {
            SetRenderersLayerOrder(handRenderers, 12);
            SetRenderersLayerOrder(headRenderers, 5);
            isHandAndHeadOnFront = true;
        }
    }

    private void SetRenderersLayerOrder(SpriteRenderer[] spriteRenderers, int layerOrder)
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
            {
                spriteRenderers[i].sortingOrder = layerOrder;
            }
        }
    }

    private void UpdateBodyPartsDirection()
    {
        if (CrosshairMouse.AimDirection.x < -0.1f && TadaInput.IsMouseActive)
        {
            SetBodyPartsDirection(Direction.Left);
        }

        if (CrosshairMouse.AimDirection.x > 0.1f && TadaInput.IsMouseActive)
        {
            SetBodyPartsDirection(Direction.Right);
        }
    }

    private void SetBodyPartsDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                if (isRightDirection)
                {
                    lookAtHandler.FlipAxis(true);

                    if (hips != null)
                        hips.transform.localScale -= Vector3.right * 2;

                    if (upperBody != null)
                        upperBody.transform.localScale -= Vector3.right * 2;

                    isRightDirection = false;
                }
                break;

            case Direction.Right:
                if (!isRightDirection)
                {
                    lookAtHandler.FlipAxis(false);

                    if (hips != null)
                        hips.transform.localScale += Vector3.right * 2;

                    if (upperBody != null)
                        upperBody.transform.localScale += Vector3.right * 2;

                    isRightDirection = true;
                }
                break;

            default:
                break;
        }
    }

    private void CheckIfMissingClasses()
    {
        if (lookAtUpdater == null || lookAtHandler == null || shoulderSecondary == null)
        {
            Debug.LogWarning(gameObject.name + ": Missing behaviour classes!");
            return;
        }
    }
}
