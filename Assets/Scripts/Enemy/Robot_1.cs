using Pathfinding;
using System;
using System.Collections;
using UnityEngine;
public class Robot_1 : Enemy_AI
{
    private float moveSpeed = 2f; // Скорость движения врага
    private float attackCooldown = 1f; // Время между атаками
    private float damageCooldown = 1f;
    public float attackDamage = 10f; // Урон от атаки врага
    private float agroRange = 5f; // Дистанция, при которой враг начинает атаковать игрока
    private float chaseDistance = 15f; // Дальность преследования
    //private float shootingRange = 10f; // Максимальная дистанция для стрельбы
    public float stopDistance = 0.1f;
    private float lastAttackTime; // Время последней атаки
    private float lastDamageTime;
    public Vector2 directionToPlayer;
    public Vector2 shootingDirection;
    public float distanceToPlayer;
    public float distanceToStart;

    public bool isAttacking = false;
    public bool isReturning = false;
    public bool isMoving = false;
    public float Health;
    private bool isDie;
    private bool isTakeDamage = false;
    private bool isDam = false;
    private bool Left_1;
    private bool Right_1;
    private bool Left;
    private bool Right;
    private bool Up;
    private bool Down;
    private bool Idle;
    private bool IsColliderFind;

    public Transform shootpoint;
    private Transform player; // Ссылка на игрока
    private Animator animator; //Animator для визуализации
    private Vector3 startingPosition; // Начальная позиция врага
    public LayerMask ignoreLayer_1;

    public GameObject pref_2;
    public GameObject bul_robot;
    public float bul_speed_2;
    public static bool IsRobot = true;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb_2 = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Получаем компонент Animator
        player = GameObject.FindGameObjectWithTag("Player_1").transform; // Находим игрока по тегу
        startingPosition = transform.position; // Запоминаем начальную позицию врага
        Health = GetComponent<Enemy_1>().health_enemy;
        InvokeRepeating("UpdatePath", 0f, 0.5f);
        //GetDamage();
    }
    void FixedUpdate()
    {
        distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < agroRange) { isAttacking = true; }
        if (distanceToPlayer > agroRange) { isAttacking = false; }

        if (isAttacking == true)
        {
            AttackPlayer();
        }
        else
        {
            if (distanceToPlayer < chaseDistance)
            {
                MoveTowardsPlayer();
            }
            else
            {
                rb_2.velocity = Vector2.zero;
                AnimationStay();
            }
        }
        Health = GetComponent<Enemy_1>().health_enemy;
        Get_Health();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            IsColliderFind = true;
        }
        else
        {
            IsColliderFind = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            IsColliderFind = true;
        }
        else
        {
            IsColliderFind = false;
        }
    }
    void AttackPlayer()
    {
        Left = false;
        Right = false;
        Up = false;
        Down = false;
        Left_1 = false;
        Right_1 = false;
        Idle = false;

        float playerX = player.position.x;
        float playerY = player.position.y;
        float enemyX = transform.position.x;
        float enemyY = transform.position.y;
        float yDiff = Mathf.Abs(playerY - enemyY);
        float xDiff = Mathf.Abs(playerX - enemyX);

        if ((playerY > enemyY) && (yDiff > xDiff))
        {
            Up = true;
            shootingDirection = Vector2.up;
            if (playerX > enemyX)
            {
                rb_2.velocity = new Vector2(moveSpeed, 0);
            }
            else
            {
                rb_2.velocity = new Vector2(-moveSpeed, 0);
            }
        }
        if ((playerY < enemyY) && (yDiff > xDiff))
        {
            Down = true;
            shootingDirection = -Vector2.up;
            if (playerX > enemyX)
            {
                rb_2.velocity = new Vector2(moveSpeed, 0);
            }
            else
            {
                rb_2.velocity = new Vector2(-moveSpeed, 0);
            }
        }
        if ((playerX > enemyX) && (xDiff > yDiff))
        {
            Right = true;
            shootingDirection = Vector2.right;
            if (playerY > enemyY)
            {
                rb_2.velocity = new Vector2(0, moveSpeed);
            }
            else
            {
                rb_2.velocity = new Vector2(0, -moveSpeed);
            }
        }
        if ((playerX < enemyX) && (xDiff > yDiff))
        {
            Left = true;
            shootingDirection = -Vector2.right;
            if (playerY > enemyY)
            {
                rb_2.velocity = new Vector2(0, moveSpeed);
            }
            else
            {
                rb_2.velocity = new Vector2(0, -moveSpeed);
            }
        }
        animator.SetBool("Left_1", Left_1);
        animator.SetBool("Right_1", Right_1);
        animator.SetBool("At_right", Right);
        animator.SetBool("At_left", Left);
        animator.SetBool("At_up", Up);
        animator.SetBool("At_down", Down);
        animator.SetBool("Idle", Idle);
        animator.SetBool("Death_1", isDie); //добавить в аниматор триггер

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            RaycastHit2D hit = Physics2D.Raycast(shootpoint.position, shootingDirection, Mathf.Infinity, ~ignoreLayer_1);
            if (hit.collider != null)
            {
                if (hit.collider.tag == "Player_1")
                {
                    Player player_1 = hit.collider.GetComponent<Player>();
                    Debug.Log("Attack! Damage: " + attackDamage);
                    Debug.Log("Attack! Damage: " + hit.collider.tag);
                    player_1.TakeDamage_hero(attackDamage);
                    player_1.TakeHP_hero();
                    bul_robot = Instantiate(pref_2, shootpoint.position, Quaternion.identity);
                    if (bul_robot != null)
                    {
                        bul_robot.GetComponent<Rigidbody2D>().velocity = shootingDirection * bul_speed_2;
                    }
                }
            }
            lastAttackTime = Time.time;
        }
    }
    void MoveTowardsPlayer()
    {
        Move();
        distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > stopDistance)
        {
            rb_2.velocity = direction * moveSpeed;
        }
        else
        {
            rb_2.velocity = Vector2.zero;
        }
        AnimationMove(player.position);
    }
    void AnimationMove(Vector3 direction_point)
    {
        Left = false;
        Right = false;
        Up = false;
        Down = false;
        Left_1 = false;
        Right_1 = false;
        Idle = false;
        Vector2 distance = direction_point - transform.position;
        if (distance.x < 0)
        {
            Left_1 = true;
        }
        else
        {
            Right_1 = true;
        }
        animator.SetBool("Left_1", Left_1);
        animator.SetBool("Right_1", Right_1);
        animator.SetBool("At_right", Right);
        animator.SetBool("At_left", Left);
        animator.SetBool("At_up", Up);
        animator.SetBool("At_down", Down);
        animator.SetBool("Idle", Idle);
        animator.SetBool("Death_1", isDie); //добавить в аниматор триггер
    }
    void AnimationStay()
    {
        Idle = true;
        Left = false;
        Right = false;
        Up = false;
        Down = false;
        Left_1 = false;
        Right_1 = false;
        animator.SetBool("Left_1", Left_1);
        animator.SetBool("Right_1", Right_1);
        animator.SetBool("At_right", Right);
        animator.SetBool("At_left", Left);
        animator.SetBool("At_up", Up);
        animator.SetBool("At_down", Down);
        animator.SetBool("Idle", Idle);
        animator.SetBool("Death_1", isDie); //добавить в аниматор триггер

    }
    //если заработает, тоже самое во все скприпты врагов
    /*
    void GetDamage()
    {
        Enemy_1.Get_Damage_enemy += DamageToEnemy;
    }
    void DamageToEnemy()
    {
        isTakeDamage = true;
        if (Time.time >= lastDamageTime + damageCooldown)
        {
            isTakeDamage = false;
            lastDamageTime = Time.time;
        }
    }
    */
    void Get_Health()
    {
        if (Health <= 0)
        {
            isDie = true;
            animator.SetBool("Death_1", isDie); //добавить в аниматор триггер
        }
        else
        {
            isDie = false;
        }
    }
}

