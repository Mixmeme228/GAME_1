using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendDataHero: MonoBehaviour
{
    public DataHero hero;
    private void Awake()
    {
        
        hero = GameObject.FindGameObjectWithTag("Player_1").GetComponent<DataHero>();
    }
    private void Update()
    {
        hero = GameObject.FindGameObjectWithTag("Player_1").GetComponent<DataHero>();
    }
}
