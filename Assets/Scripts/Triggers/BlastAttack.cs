using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlastAttack : MonoBehaviour
{
    public GameObject explosion;
    private bool PlayerHere = false;
    private void GetInformationAboutPlayerInBlastAttack()
    {
        PlayerHere = true; //меняем флаг
        Boss1.Instance.isShooting = false;
        Boss1.Instance.blastAttack = true;
        //меняем флаги у босса
        if (gameObject.tag == "Blast1") //анализируем в какое из укрытий зашёл герой
        {
            Boss1.Instance.blastAttack_1 = true; //меняем соответствующий флаг у босса
            Boss1.Instance.blastAttack_2 = Boss1.Instance.blastAttack_3 = Boss1.Instance.blastAttack_4 = false;
            //меняем флаги для других областей
        }
        if (gameObject.tag == "Blast2") //анализируем в какое из укрытий зашёл герой
        {
            Boss1.Instance.blastAttack_2 = true; //меняем соответствующий флаг у босса
            Boss1.Instance.blastAttack_1 = Boss1.Instance.blastAttack_3 = Boss1.Instance.blastAttack_4 = false;
            //меняем флаги для других областей
        }
        if (gameObject.tag == "Blast3") //анализируем в какое из укрытий зашёл герой
        {
            Boss1.Instance.blastAttack_3 = true; //меняем соответствующий флаг у босса
            Boss1.Instance.blastAttack_1 = Boss1.Instance.blastAttack_2 = Boss1.Instance.blastAttack_4 = false;
            //меняем флаги этой и для других областей
        }
        if (gameObject.tag == "Blast4") //анализируем в какое из укрытий зашёл герой
        {
            Boss1.Instance.blastAttack_4 = true; //меняем соответствующий флаг у босса
            Boss1.Instance.blastAttack_1 = Boss1.Instance.blastAttack_2 = Boss1.Instance.blastAttack_3 = false;
            //меняем флаги этой и для других областей
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //считываем вхождение героя в области интенсивных атак
        if (collision.gameObject.tag == "Player_1")
        {
            GetInformationAboutPlayerInBlastAttack();
            PlayerHere = true; //меняем флаг
            Boss1.Instance.isShooting = false; 
            Boss1.Instance.blastAttack = true;
            //меняем флаги у босса
            if (gameObject.tag == "Blast1") //анализируем в какое из укрытий зашёл герой
            {
                Boss1.Instance.blastAttack_1 = true; //меняем соответствующий флаг у босса
                Boss1.Instance.blastAttack_2 = Boss1.Instance.blastAttack_3 = Boss1.Instance.blastAttack_4 = false;
                //меняем флаги этой и для других областей
            }
            if (gameObject.tag == "Blast2") //анализируем в какое из укрытий зашёл герой
            {
                Boss1.Instance.blastAttack_2 = true; //меняем соответствующий флаг у босса
                Boss1.Instance.blastAttack_1 = Boss1.Instance.blastAttack_3 = Boss1.Instance.blastAttack_4 = false;
                //меняем флаги этой и для других областей
            }
            if (gameObject.tag == "Blast3") //анализируем в какое из укрытий зашёл герой
            {
                Boss1.Instance.blastAttack_3 = true; //меняем соответствующий флаг у босса
                Boss1.Instance.blastAttack_1 = Boss1.Instance.blastAttack_2 = Boss1.Instance.blastAttack_4 = false;
                //меняем флаги этой и для других областей
            }
            if (gameObject.tag == "Blast4") //анализируем в какое из укрытий зашёл герой
            {
                Boss1.Instance.blastAttack_4 = true; //меняем соответствующий флаг у босса
                Boss1.Instance.blastAttack_1 = Boss1.Instance.blastAttack_2 = Boss1.Instance.blastAttack_3 = false;
                //меняем флаги этой и для других областей
            }
        }
        if (collision.gameObject.tag == "GrenadaBoss")
        {
            //если попала пуля или граната босса в область для укрытия и там находится герой
            //то игроку наносится урон
            Instantiate(explosion, collision.transform.position, Quaternion.identity); //создаём объект для урона
            if (PlayerHere == true)
            {
                //Player.Instance.TakeDamage_hero(Boss1.Instance.attackDamage);
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player_1")
        {
            //считываем информацию о нахождении игрока в укрытии
            GetInformationAboutPlayerInBlastAttack();
            PlayerHere = true;
            //считываем информацию о нахождении игрока в укрытии
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //если герой уходит из укрытия, обноаляем флаги у босса и этого скрипта
        if (collision.gameObject.tag == "Player_1")
        {
            PlayerHere = false;
            Boss1.Instance.blastAttack_1 = Boss1.Instance.blastAttack_2 = false;
            Boss1.Instance.blastAttack_3 = Boss1.Instance.blastAttack_4 = false;
        }
    }
}
