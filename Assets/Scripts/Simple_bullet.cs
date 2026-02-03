using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//скрипт который описывает движение пули для героя и обычных врагов
public class Simple_bullet : MonoBehaviour
{
    public bool IsPlayer = false;
    public bool IsEnemy = false;

    private void Awake()
    {
        if (gameObject.tag == "BulPlayer") //определеляем кому принадлежит пуля
        {
            IsPlayer = true; //меняем соответствующий флаг
        }
        if (gameObject.tag == "BulEnemy") //определеляем кому принадлежит пуля
        { 
            IsEnemy = true; //меняем соответствующий флаг
        }
    }
    //при соприкосновении с вргаом или героем мы должны уничтожать пулю
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPlayer == true)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                Destroy(gameObject, 0.06f);
            }
        }
        if (IsEnemy == true)
        {
            if (collision.gameObject.tag == "Player_1")
            {
                Destroy(gameObject, 0.06f);
            }
        }
    }
    //после того, как пуля соприкоснулась с какой-либо поверхностью мы также должны уничтожать объект пули
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject, 0.06f);
    }
}
