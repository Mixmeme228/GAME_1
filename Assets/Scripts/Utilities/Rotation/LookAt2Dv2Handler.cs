using UnityEngine;


public class LookAt2Dv2Handler : MonoBehaviour
{
    public LookAt2Dv2[] lookAts;

    private void Awake()
    {
        CheckArray();
    }

    public void FlipAxis(bool value)
    {
        for (int i = 0; i < lookAts.Length; i++)
        {
            if (lookAts[i] != null)
                lookAts[i].isFlipAxis = value;
        }
    }

    public void SwitchToTarget(LookAt2Dv2.LookAtTarget target)
    {
        for (int i = 0; i < lookAts.Length; i++)
        {
            if (lookAts[i] != null)
                lookAts[i].SwitchToTarget(target);
        }
    }

    private void CheckArray ()
    {
        for (int i = 0; i < lookAts.Length; i++)
        {
            if (lookAts[i] == null)
            {
                Debug.Log(gameObject.name + ": LookAts to Update missing! Check array!");
                return;
            }
        }
    }
}
