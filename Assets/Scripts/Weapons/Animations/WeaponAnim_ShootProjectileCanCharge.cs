using UnityEngine;
public class WeaponAnim_ShootProjectileCanCharge : AnimationHandler
{
   

    public enum Animation { Idle, BasicShot, Charging, ChargedShot }

    public void PlayAnimation(Animation name)
    {
        switch (name)
        {
            case Animation.Idle:
                PlayAnimation("Idle");
                break;

            case Animation.BasicShot:
                PlayAnimation("BasicShot");
                break;

            case Animation.Charging:
                PlayAnimation("Charging");
                break;

            case Animation.ChargedShot:
                PlayAnimation("ChargedShot");
                break;

            default:
                break;
        }
    }
}
