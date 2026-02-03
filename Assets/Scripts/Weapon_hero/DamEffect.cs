using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamEffect : MonoBehaviour
{
    private Animator anim;
    private const string _go = "Go";
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        anim.SetBool(_go, Gun.Instance.IsCollider());
    }
}

