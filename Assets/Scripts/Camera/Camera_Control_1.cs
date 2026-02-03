
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;

public class Camera_Control_1 : MonoBehaviour
{
    public Vector3 PosPlayer;
    public SendDataHero sd;
    private void Start()
    {
        sd = GameObject.FindGameObjectWithTag("Send").GetComponent<SendDataHero>();
        PosPlayer = sd.hero.pos;
    }
    void Update()
    {
        PosPlayer = sd.hero.pos;
        transform.position = new Vector3(PosPlayer.x, PosPlayer.y, -1);
    }
}
