using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//скрипт для кнопки, которую нужно уничтожить в бое с боссом
public class ButtonBoss : MonoBehaviour
{
    public int health_button = 140;
    public float LastAttackTime;
    public float AttackCoolDown = 1f;
    public GameObject pref_mobe;//префаб моба
    public static event EventHandler isDestroy;
    private void Update()
    {
        if (pref_mobe != null)
        {
            if (health_button <= 140 && health_button >= 120) //если здоровье больше 120 и меньше 140
            {
                for (int i = 0; i < 4; i++)
                {
                    Instantiate(pref_mobe, transform.position, Quaternion.identity);
                    //создаём четырёх мобов
                }
            }
            if (health_button < 120 && health_button >= 80) //если здоровье больше 80 и меньше 120
            {
                for (int i = 0; i < 3; i++)
                {
                    Instantiate(pref_mobe, transform.position, Quaternion.identity);
                    //создаём трёх мобов
                }
            }
            if (health_button < 80 && health_button >= 40) //если здоровье больше 40 и меньше 80
            {
                for (int i = 0; i < 2; i++)
                {
                    Instantiate(pref_mobe, transform.position, Quaternion.identity);
                    //создаём двух мобов
                }
            }
            if (health_button < 40 && health_button > 0) //если здоровье больше 0 и меньше 40
            {
                Instantiate(pref_mobe, transform.position, Quaternion.identity);
                //создаём одного моба
            }
            if (health_button <= 0) //если здоровье меньше нуля
            {
                isDestroy?.Invoke(this, EventArgs.Empty); 
                //запускаем событие уничтожение кнопки (сама пока не могу вспомнить для чего создала это событие)
                Destroy(gameObject, 1f); 
                //уничтожаем объект кнопки
            }
        }
    }

    public void TakeDamageButton()
    {
        health_button -= 10;
    }

    //в методах ниже у кнопки отнимается здоровье при атаке игрока
    //надо обсудить (переделывать ли так, чтобы можно было атаковать также и при помощи ружья, так как сейчас будет считываться непосредственно соприкосновение с кнопкой)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.tag == "GunPlayer")
            {
                if (Time.time >= LastAttackTime + AttackCoolDown)
                {
                    health_button -= 10;
                    LastAttackTime = Time.time;
                }
            }
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.tag == "GunPlayer")
            {
                if (Time.time >= LastAttackTime + AttackCoolDown)
                {
                    health_button -= 10;
                    LastAttackTime = Time.time;
                }
            }
        }
    }
}
