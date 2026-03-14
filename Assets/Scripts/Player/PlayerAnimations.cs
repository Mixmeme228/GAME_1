using UnityEngine;


public class PlayerAnimations : AnimationHandler
{
    
   

    
   
    public enum AnimName { Idle, WalkForward, WalkBackwards }

    public void SetAnimationSpeed(AnimName name, float value)
    {
        switch (name)
        {
            case AnimName.Idle:
                SetAnimationSpeed("Idle", value);
                break;

            case AnimName.WalkForward:
                // value / 2.5f because player moveSpeed / 2.5f is what feels better for this animation
                SetAnimationSpeed("WalkForward", value / 2.5f);
                break;

            case AnimName.WalkBackwards:
                // value / 2.5f because player moveSpeed / 2.5f is what feels better for this animation
                SetAnimationSpeed("WalkBackwards", value / 2.5f);
                break;

            default:
                break;
        }
    }

    public void PlayMoveAnimationsByMoveInputAndLookDirection(Vector3 moveInput)
    {
       
        if ((moveInput.x > 0 || moveInput.y > 0) && ((CrosshairMouse.AimDirection.x > 0 && TadaInput.IsMouseActive) || 
            (CrosshairJoystick.AimDirection.x > 0 && !TadaInput.IsMouseActive)))
            PlayAnimation("WalkForward");
        else if ((moveInput.x > 0 || moveInput.y > 0) && ((CrosshairMouse.AimDirection.x < 0 && TadaInput.IsMouseActive) ||
            (CrosshairJoystick.AimDirection.x < 0 && !TadaInput.IsMouseActive)))
            PlayAnimation("WalkBackwards");
        else if ((moveInput.x < 0 || moveInput.y < 0) && ((CrosshairMouse.AimDirection.x < 0 && TadaInput.IsMouseActive) ||
            (CrosshairJoystick.AimDirection.x < 0 && !TadaInput.IsMouseActive)))
            PlayAnimation("WalkForward");
        else if ((moveInput.x < 0 || moveInput.y < 0) && ((CrosshairMouse.AimDirection.x > 0 && TadaInput.IsMouseActive) ||
            (CrosshairJoystick.AimDirection.x > 0 && !TadaInput.IsMouseActive)))
            PlayAnimation("WalkBackwards");
        else
            PlayAnimation("Idle");
    }
}
