using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointShootRobot : MonoBehaviour
{
    private Vector3 _startPos = new Vector3(-0.45f, 0.05f, 0f);
    void Update()
    {
        if (Player.Instance.IsAttackingDown())
        {
            transform.localPosition = _startPos;
        }
        if (Player.Instance.IsAttackingUp())
        {
            transform.localPosition = new Vector3(0.5f, 0.05f, 0f);
        }
        if (Player.Instance.IsAttackingLeft())
        {
            transform.localPosition = _startPos;
        }
        if (Player.Instance.IsAttackingRight())
        {
            transform.localPosition = new Vector3(0.5f, 0.05f, 0f);
        }
    }
}
