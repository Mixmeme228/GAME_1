using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMoveBoss : MonoBehaviour
{
    public static PointMoveBoss Instance { get; private set; }

    public float radius = 5f;
    public float speed = 2f;
    private float t = 0f; // Параметр времени
    public float StartCenterX = 43.1f;
    public float StartCenterY = 43.1f;
    //параметры центра вращения поменять нужно
    private float lastAttackTime_boss;
    private float attackCooldown_boss = 1f;
    public GameObject bullet;
    public GameObject grenada;
    public Vector2 PointPos;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //в зависимости от того или иного вида атаки запускаем событие
        Boss1.Instance.Attack_1 += CreateBullet; //создаём пули
        Boss1.Instance.Attack_2 += CreateGrenada; //создаём гранаты
    }
    private void CreateBullet(object sender, System.EventArgs e)
    {
        lastAttackTime_boss = Time.time;
        Instantiate(bullet, transform.position, Quaternion.identity);
        //выпускаем пули с определённым интервалом
    }
    private void CreateGrenada(object sender, System.EventArgs e)
    {
        //в зависимости от того, в какую область стреляет босс, точка выстрела будет менять положение
        if (Boss1.Instance.blastAttack_1 == true)
        {
            PointPos = new Vector2();//установить
        }
        if (Boss1.Instance.blastAttack_2 == true)
        {
            PointPos = new Vector2();//установить
        }
        if (Boss1.Instance.blastAttack_3 == true)
        {
            PointPos = new Vector2();//установить
        }
        if (Boss1.Instance.blastAttack_4 == true)
        {
            PointPos = new Vector2();//установить
        }
        transform.position = PointPos;
        Instantiate(grenada, transform.position, Quaternion.identity);
        lastAttackTime_boss = Time.time;
        //и из этой точки с новыми координатами выпускаем гранаты с определённым интервалом
    }
    private Vector2 GetPositionOnCircle(float centerX, float centerY)
    {
        // Параметрическое уравнение окружности: x = centerX + radius * cos(t), y = centerY + radius * sin(t)
        float x = centerX + radius * Mathf.Cos(t);
        float y = centerY + radius * Mathf.Sin(t);

        t += speed * Time.deltaTime; // Изменяем параметр времени
        return new Vector2(x, y);
    }

    void Update()
    {
        if (Boss1.Instance.isShooting == true) //если босс стреляет пулями
        {
            Vector2 newPos = GetPositionOnCircle(StartCenterX, StartCenterY);
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
            //то точка выстрела продолжает также как и босс двигаться по окружности
        }
    }

    public Vector2 ReturnPosition()
    {
        return transform.position;
    }
}
