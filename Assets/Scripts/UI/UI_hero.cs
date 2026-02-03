using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_hero : MonoBehaviour
{
    public Image bar;
    public Text count_keys;
    public SendDataHero sd;

    void Start()
    {
        sd = GameObject.FindGameObjectWithTag("Send").GetComponent<SendDataHero>();
    }

    void Update()
    {
        bar.fillAmount = sd.hero.HP_hero;
        count_keys.text = (sd.hero.Count_key).ToString();
    }
}
