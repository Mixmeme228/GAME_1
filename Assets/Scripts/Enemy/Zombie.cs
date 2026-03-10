using Pathfinding;
using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class Zombie : Enemy_AI
{
    public float moveSpeed = 2f; // �������� �������� �����
    public float attackDamage = 10f; //���� �� �����
    public float attackRange = 2.5f; // ��������� �����
    public float attackCooldown = 1f; // ����� ����� �������
    public float chaseDistance = 5f; // ���������� �������������
    public float stopDistance = 0.1f;
    public float distanceToPlayer;
    public float distanceToStart;
    public Vector2 distance;
    public float Health_;

    private Transform player; // ������ �� ������
    private Vector3 startingPosition; // ��������� ������� �����
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

        // ���������, ��������� �� ����� � �������� ���������� �������������
        if (distanceToPlayer < chaseDistance)
        {
            MoveTowardsPlayer();
            Animation(player.position);

            // ���� ����� ������ � �����, �������
            if (distanceToPlayer < attackRange)
            {
                AttackPlayer();
                Animation(player.position);
            }
        }
        else
        {
            // ���� ����� ������, ������������ �� ��������� �������
            ReturnToStartingPosition();
            Animation(startingPosition);
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

            // ��������� ������ ���� ����� �� ������� ������
            if (distanceToPlayer > stopDistance)
            {
                rb_2.linearVelocity = direction * moveSpeed;
            }
            else
            {
                rb_2.linearVelocity = Vector2.zero; // ���������������, ���� ������� ������
            }
        }
        else
        {
            rb_2.linearVelocity = Vector2.zero; // ���������������, ���� ������ ���
        }
    }
    void AttackPlayer()
    {
        Move();
        if (Player.Instance.IsAttacking_() == true)
        {
            rb_2.linearVelocity = Vector3.zero;
            IsAttacking = true;
            IsWalking = false;
        }
        else
        {
            rb_2.linearVelocity = direction * moveSpeed;
            IsAttacking = false;
            IsWalking = true;
        }
    }
    void ReturnToStartingPosition()
    {
        // ������������ �� ��������� ���������
        distanceToStart = Vector2.Distance(transform.position, startingPosition);
        Vector2 directionToStart = (startingPosition - transform.position).normalized;

        if (distanceToStart > 0.1f) // ��� �������������� ��������
        {
            rb_2.linearVelocity = directionToStart * moveSpeed;
            IsWalking = true;
            IsAttacking = false;
        }
        else
        {
            rb_2.linearVelocity = Vector2.zero; // ���������������, ��� ������ �������� ��������� �������
            IsWalking = false;
            IsAttacking = false;
        }
    }
    //������� ����� ��������
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
        if (distanceToStart < 0.1f)
        {
            IsAttackUp = IsAttackDown = IsAttackLeft = IsAttackRight = false;
            IsWalkUp = IsWalkDown = IsWalkLeft = IsWalkRight = false;
            IsDeathUp = IsDeathDown = IsDeathLeft = IsDeathRight = false;
            IsAttacking = IsWalking = false;
            IsStop = true;
        }
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
        anim.SetBool("Death_2", isDie_);//�������� � �������� �������
    }
    void Get_Health()
    {
        if (Health_ <= 0)
        {
            isDie_ = true;
        }
        else
        {
            isDie_ = false;
        }
    }
}