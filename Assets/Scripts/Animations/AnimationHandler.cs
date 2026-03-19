using UnityEngine;
public class AnimationHandler : MonoBehaviour
{
    [SerializeField] private bool getAnimInChild = false;
    protected Animation anim;
    protected virtual void Awake()
    {
        if (!getAnimInChild)
            TryGetComponent(out anim);
        else
            anim = GetComponentInChildren<Animation>();
    }
    protected virtual void PlayAnimation(string name)
    {
        if (anim.GetClip(name) == null)
            return;
        anim.Play(name);
    }
    protected virtual void SetAnimationSpeed(string name, float speed)
    {
        if (anim.GetClip(name) == null)
            return;
        anim[name].speed = speed;
    }
}
