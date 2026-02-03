using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    public static ActiveWeapon Instance { get; private set; }
    [SerializeField] private GameObject act_weapon;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        //
    }
    void Update()
    {
        //
    }
    public GameObject GetActiveWeapon()
    {
        return act_weapon;
    }

}
