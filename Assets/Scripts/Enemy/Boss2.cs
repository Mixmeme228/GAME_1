using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using System;

//работает по примерно той же логике что и скрипт ButtonBoss
public class Boss2 : MonoBehaviour
{
    public static Boss2 Instance { get; private set; }

    private float moveSpeed = 2f; // Скорость движения врага
    private float attackCooldown = 3f; // Время между атаками
    public float attackDamage = 20f; // Урон от атаки врага
    private float lastAttackTime; // Время последней атаки
    public Vector2 directionToPlayer;
    public float distanceToPlayer;
    public Rigidbody2D rb_2_boos;
    public Animator animat;
    public Transform player_1;
    public GameObject prefmoster;
    //public GameObject monst;
    private Coroutine createCoroutine;
    private Coroutine moveCoroutine;
    public float Health_2;
    public Transform boss_pos;
    public System.Random rnd;

    private void Awake()
    {
        rb_2_boos = GetComponent<Rigidbody2D>();
        animat = GetComponent<Animator>();
        player_1 = GameObject.FindGameObjectWithTag("Player_1").transform; // Находим игрока по тегу
        Health_2 = GetComponent<Enemy_2>().boss_health;
        boss_pos = transform;
        rnd = new System.Random();
    }
    private void Start()
    {
        createCoroutine = StartCoroutine(NewMonsters());
        moveCoroutine = StartCoroutine(ChangePos());
    }

    private IEnumerator NewMonsters()
    {
        while (true)
        {
            CreateMonstersFromBoss();
            yield return new WaitForSeconds(12f);
        }
    }
    private IEnumerator ChangePos()
    {
        while (true)
        {
            ChangePositionBoss();
            yield return new WaitForSeconds(25f);
        }
    }
    private void ChangePositionBoss()
    {
        float rnd_X = Game.Utils_1.Common2.RandomFloatBetween(transform.position.x, transform.position.x + 12f);
        float rnd_Y = Game.Utils_1.Common2.RandomFloatBetween(transform.position.y, transform.position.y + 12f);
        float rnd_Z = 0f;
        transform.position = new Vector3(rnd_X, rnd_Y, rnd_Z);
    }
    private void CreateMonstersFromBoss()
    {
        if (Health_2 <= 150f && Health_2 > 100f)
        {
            for (int i = 0; i < 3; i++)
            {
                Instantiate(prefmoster, transform.position, Quaternion.identity);
            }
        }
        if (Health_2 <= 100f && Health_2 > 50f)
        {
            for (int i = 0; i < 2; i++)
            {
                Instantiate(prefmoster, transform.position, Quaternion.identity);
            }
        }
        if (Health_2 <= 50f && Health_2 > 1f)
        {
            Instantiate(prefmoster, transform.position, Quaternion.identity);
        }
    }
    private void Update()
    {
        if (Health_2 <= 0f)
        {
            StopCoroutine(createCoroutine);
            StopCoroutine(moveCoroutine);
            Destroy(gameObject, 1f);
        }
    }
}
