using UnityEngine;


public class LookAt2Dv2Updater : MonoBehaviour
{
    
   
    

    

    public LookAt2Dv2[] lookAtsToUpdate;

    public void UpdateLookAtClasses()
    {
        for (int i = 0; i < lookAtsToUpdate.Length; i++)
        {
            if (lookAtsToUpdate[i] == null)
            {
                Debug.Log(gameObject.name + ": LookAts to Update missing! Check array!");
                return;
            }
        }

        for (int i = 0; i < lookAtsToUpdate.Length; i++)
        {
            lookAtsToUpdate[i].UpdateLookAt();
        }
    }
}
