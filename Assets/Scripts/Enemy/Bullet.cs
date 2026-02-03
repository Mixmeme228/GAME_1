using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

//скрипт должен описывать поведение оружия босса первого уровня
public class Bullet : MonoBehaviour
{
    public bool isGrenada;
    public bool isBullet;
    public float targetPosX;
    public float targetPosY;
    public Transform area;
    public Rigidbody2D rb;
    public float speed_bullet = 15f;
    public Vector2 direction_bullet;
    public Vector2 PointPos;
    public float CenterX; //координата x местопложения босса, выпускающего гранату
    public float CenterY; //координата y местопложения босса, выпускающего гранату

    private void IdentifyTargetOrDirection()
    {
        if (isGrenada == true) //если это граната
        {
            //определяем в какой области находится 
            //определяем местоположение данной областм
            //выбираем конечную координату гранаты в её границах
            if (Boss1.Instance.blastAttack_1 == true)
            {
                area = GameObject.FindGameObjectWithTag("Blast1").transform; 
                targetPosX = Random.Range(area.position.x - 2.5f, area.position.x + 2.5f);
                targetPosY = Random.Range(area.position.y - 2.5f, area.position.y + 2.5f);
            }
            if (Boss1.Instance.blastAttack_2 == true)
            {
                area = GameObject.FindGameObjectWithTag("Blast2").transform;
                targetPosX = Random.Range(area.position.x - 2.5f, area.position.x + 2.5f);
                targetPosY = Random.Range(area.position.y - 2.5f, area.position.y + 2.5f);
            }
            if (Boss1.Instance.blastAttack_3 == true)
            {
                area = GameObject.FindGameObjectWithTag("Blast3").transform;
                targetPosX = Random.Range(area.position.x - 2.5f, area.position.x + 2.5f);
                targetPosY = Random.Range(area.position.y - 2.5f, area.position.y + 2.5f);
            }
            if (Boss1.Instance.blastAttack_4 == true)
            {
                area = GameObject.FindGameObjectWithTag("Blast4").transform;
                targetPosX = Random.Range(area.position.x - 2.5f, area.position.x + 2.5f);
                targetPosY = Random.Range(area.position.y - 2.5f, area.position.y + 2.5f);
            }
        }
        if (isBullet == true)
        {
            //в зависимости от угла поворта босса и положения пули определяем направление её движения
            if (Mathf.Abs(CenterX - PointPos.x) <= Mathf.Abs(CenterY - PointPos.y))
            {
                if (CenterY - PointPos.y < 0f)
                {
                    direction_bullet = Vector2.right;
                }
                if (CenterY - PointPos.y > 0f)
                {
                    direction_bullet = -Vector2.right;
                }
            }
            else
            {
                if (CenterY - PointPos.y < 0f)
                {
                    direction_bullet = Vector2.up;
                }
                if (CenterY - PointPos.y > 0f)
                {
                    direction_bullet = -Vector2.up;
                }
            }
        }
    }
    //перед началом работы определяем вид оружия: пуля или граната
    void Awake()
    {
        if (gameObject.tag == "BulletBoss")
        {
            isBullet = true;
            isGrenada = false;
        }
        if (gameObject.tag == "GrenadaBoss")
        {
            isBullet = false;
            isGrenada = true;
        }
        //PointPos = GameObject.FindGameObjectWithTag("PointAttackBoss").transform; находим объект по тегу
    }
    //инициализируем компонент физики
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    //в этом методе мы описываем движение пули или гранаты
    void FixedUpdate()
    {
        PointPos = PointMoveBoss.Instance.ReturnPosition(); //определяем положение точки выстрела
        IdentifyTargetOrDirection();
        if (isGrenada == true)
        {
            rb.MovePosition(new Vector2(targetPosX, targetPosY));
            //если граната, то мы отправляем её двигаться в определённую координату
        }
        if (isBullet == true)
        {
            rb.velocity = direction_bullet.normalized * speed_bullet;
            //если пуля, то она двигается по выбранному направлению с определённой скоростью
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBullet == true)
        {
            //если наш объект является пулей, то наноси урон нашему герою только в том случае, если пуля попала в него
            if (collision.gameObject.tag == "Player_1") //считываем тег объекта
            {
                Player hero = collision.gameObject.GetComponent<Player>(); //полачаем доступ к компоненту-скрипту игрока
                if (hero != null)
                {
                    hero.TakeDamage_hero(Boss1.Instance.attackDamage); //наносим урон
                }
            }
            Destroy(gameObject, 1f); //уничтожаем пулю
        }
        if (isGrenada == true)
        {
            //само нанесение урона прописано в скрипте BlastAttack
            Destroy(gameObject, 5f); //уничтожаем гранату
        }
    }
}
