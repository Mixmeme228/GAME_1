using Pathfinding;
using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class Zombie_1 : Enemy_AI
{
    public float moveSpeed = 2f; // Скорость движения врага
    public float attackRange = 2.5f; // Дальность атаки
    public float attackCooldown = 1f; // Время между атаками
    public float chaseDistance = 5f; // Расстояние преследования
    public float stopDistance = 0.1f;
    public float distanceToPlayer;
    public float distanceToStart;
    public Vector2 distance;
    public float Health_;

    private Transform player; // Ссылка на игрока
    private Vector3 startingPosition; // Начальная позиция врага
    private Animator anim;

    public bool IsAttacking = false;
    public bool IsWalking = false;
    public bool IsAttackUp;
    public bool IsAttackDown;
    public bool IsAttackLeft;
    public bool IsAttackRight;
    public bool IsWalkUp;
    public bool IsWalkDown;
    public bool IsWalkLeft;
    public bool IsWalkRight;
    public bool IsDeathUp;
    public bool IsDeathDown;
    public bool IsDeathLeft;
    public bool IsDeathRight;
    public bool IsStop;
    private bool isDie_;
    private bool IsColliderFind_;
    private bool Up;
    private bool Down;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb_2 = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player_1").transform;
        startingPosition = transform.position;
        anim = GetComponent<Animator>();
        Health_ = GetComponent<Enemy_1>().health_enemy;
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }
    void FixedUpdate()
    {
        distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Проверяем, находится ли игрок в пределах расстояния преследования
        if (distanceToPlayer < chaseDistance)
        {
            MoveTowardsPlayer();
            Animation(player.position);

            // Если игрок близок к врагу, атакуем
            if (distanceToPlayer < attackRange)
            {
                AttackPlayer();
                Animation(player.position);
            }
        }
        else
        {
            // Если игрок далеко, останавливаемся на месте
            rb_2.velocity = Vector2.zero;
            StopAnimation();
        }
        Health_ = GetComponent<Enemy_1>().health_enemy;
        Get_Health();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            IsColliderFind_ = true;
        }
        else
        {
            IsColliderFind_ = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            IsColliderFind_ = true;
        }
        else
        {
            IsColliderFind_ = false;
        }
    }
    void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Move();
            IsWalking = true;
            IsAttacking = false;

            distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Двигаемся только если игрок не слишком близко
            if (distanceToPlayer > stopDistance)
            {
                rb_2.velocity = direction * moveSpeed;
            }
            else
            {
                rb_2.velocity = Vector2.zero; // Останавливаемся, если слишком близко
            }
        }
        else
        {
            rb_2.velocity = Vector2.zero; // Останавливаемся, если игрока нет
        }
    }
    void AttackPlayer()
    {
        Move();
        if (Player.Instance.IsAttacking_() == true)
        {
            rb_2.velocity = Vector3.zero;
            IsAttacking = true;
            IsWalking = false;
        }
        else
        {
            rb_2.velocity = direction * moveSpeed;
            IsAttacking = false;
            IsWalking = true;
        }
    }
    void StopAnimation()
    {
        IsStop = true;
        Up = false;
        Down = false;
        IsAttackUp = false;
        IsAttackDown = false;
        IsAttackLeft = false;
        IsAttackRight = false;
        IsWalkUp = false;
        IsWalkDown = false;
        IsWalkLeft = false;
        IsWalkRight = false;
        anim.SetBool("Up_w", IsWalkUp);
        anim.SetBool("Down_w", IsWalkDown);
        anim.SetBool("Right_w", IsWalkRight);
        anim.SetBool("Left_w", IsWalkLeft);
        anim.SetBool("Idle_zom", IsStop);
        anim.SetBool("Up_a", IsAttackUp);
        anim.SetBool("Down_a", IsAttackDown);
        anim.SetBool("Left_a", IsAttackLeft);
        anim.SetBool("Right_a", IsAttackRight);
        anim.SetBool("Death_2", isDie_);//добавить в аниматор триггер
    }
    //функция смены анимаций
    void Animation(Vector3 direction_point)
    {
        distance = direction_point - transform.position;
        distanceToStart = Vector2.Distance(transform.position, startingPosition);
        float distanceToPoint = Vector2.Distance(transform.position, direction_point);
        Up = false;
        Down = false;
        IsAttackUp = false;
        IsAttackDown = false;
        IsAttackLeft = false;
        IsAttackRight = false;
        IsWalkUp = false;
        IsWalkDown = false;
        IsWalkLeft = false;
        IsWalkRight = false;
        IsStop = false;
        if ((distance.y < 0) && (Mathf.Abs(distance.y) > 0.9f))
        {
            Down = true;
            if (IsAttacking == true)
            {
                IsAttackDown = true;
                IsWalkDown = false;
            }
            if (IsWalking == true)
            {
                IsWalkDown = true;
                IsAttackDown = false;
            }
        }
        if ((distance.y > 0) && (Mathf.Abs(distance.y) > 0.9f))
        {
            Up = true;
            if (IsAttacking == true)
            {
                IsAttackUp = true;
                IsWalkUp = false;
            }
            if (IsWalking == true)
            {
                IsWalkUp = true;
                IsAttackUp = false;
            }
        }
        if ((Up == false) && (Down == false))
        {
            if (distance.x < 0)
            {
                if (IsAttacking == true)
                {
                    IsAttackLeft = true;
                    IsWalkLeft = false;
                }
                if (IsWalking == true)
                {
                    IsWalkLeft = true;
                    IsAttackLeft = false;
                }
            }
            if (distance.x > 0)
            {
                if (IsAttacking == true)
                {
                    IsAttackRight = true;
                    IsWalkRight = false;
                }
                if (IsWalking == true)
                {
                    IsWalkRight = true;
                    IsAttackRight = false;
                }
            }
        }
        anim.SetBool("Up_w", IsWalkUp);
        anim.SetBool("Down_w", IsWalkDown);
        anim.SetBool("Right_w", IsWalkRight);
        anim.SetBool("Left_w", IsWalkLeft);
        anim.SetBool("Idle_zom", IsStop);
        anim.SetBool("Up_a", IsAttackUp);
        anim.SetBool("Down_a", IsAttackDown);
        anim.SetBool("Left_a", IsAttackLeft);
        anim.SetBool("Right_a", IsAttackRight);
        anim.SetBool("Death_2", isDie_);//добавить в аниматор триггер
    }
    void Get_Health()
    {
        if (Health_ <= 0)
        {
            isDie_ = true;
            anim.SetBool("Death_2", isDie_);//добавить в аниматор триггер
        }
        else
        {
            isDie_ = false;
        }
    }
}