using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPbar_enemy : MonoBehaviour
{
    public Image bar;
    public Enemy_1 enemy_1;

    void Awake()
    {
        bar.fillAmount = 1f;
    }

    void Update()
    {
        bar.fillAmount = enemy_1.HP_enemy;
    }
}
