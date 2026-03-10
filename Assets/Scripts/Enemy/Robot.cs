using Pathfinding;
using System;
using System.Collections;
using UnityEngine;
public class Robot : Enemy_AI
{
    //������� ���������
    public float moveSpeed = 2f;
    public float attackDamage = 5f;
    private float detectionRadius = 5f;
    private float chaseDistance = 15f;
    public float stopDistance = 0.1f;

    //����� ��� ������
    private bool isAligned = false;
    public bool isAttacking = false;
    public bool isReturning = false;
    public bool isMoving = false;
    public float Health;
    public bool isDead;

    //����� ��� ��������
    bool At_Left = false;
    bool At_Right = false;
    bool At_Up = false;
    bool At_Down = false;
    bool Up = false;
    bool Down = false;
    bool Left = false;
    bool Right = false;
    bool Idle = false;

    //��������� ����
    private float lastAttackTime;
    public float attackCooldown = 2f;
    public GameObject bulletPrefab;
    public Transform shootpoint;
    public float bulletSpeed = 8f;

    //������������
    public float alignmentSpeed = 2f;
    public float alignmentThreshold = 0.1f;

    //������������ ������  
    private Transform player;
    public Vector2 directionToPlayer;
    public Vector2 shootingDirection;
    public float distanceToPlayer;
    public float distanceToStart;

    //Animator ��� ������������
    private Animator animator;

    // ��������� ������� �����
    private Vector3 startingPosition;

    public static bool IsRobot = true;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb_2 = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // �������� ��������� Animator
        player = GameObject.FindGameObjectWithTag("Player_1").transform; // ������� ������ �� ����
        startingPosition = transform.position; // ���������� ��������� ������� �����
        Health = GetComponent<Enemy_1>().health_enemy;
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }
    void FixedUpdate()
    {
        Get_Health();
        ResetFlags();
        distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius) { isAttacking = true; }
        else { isAttacking = false; }

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
                ReturnToStartingPosition();
            }
        }
        Health = GetComponent<Enemy_1>().health_enemy;
    }
    /*
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
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            IsColliderFind = true;
        }
        else
        {
            IsColliderFind = false;
        }
    }
    */
    void AttackPlayer()
    {
        float playerX = player.position.x;
        float playerY = player.position.y;
        float enemyX = transform.position.x;
        float enemyY = transform.position.y;
        float yDiff = Mathf.Abs(playerY - enemyY);
        float xDiff = Mathf.Abs(playerX - enemyX);
        bool posCorrect = false;
        if (yDiff>0.1f && xDiff>0.1f)
        {
            posCorrect = true;
        }

        if ((playerY > enemyY) && (yDiff > xDiff))
        {
            At_Up = true;
            shootingDirection = Vector2.up;
            if (playerX > enemyX && posCorrect)
            {
                rb_2.linearVelocity = new Vector2(moveSpeed, 0);
            }
            else if (posCorrect)
            {
                rb_2.linearVelocity = new Vector2(-moveSpeed, 0);
            }
        }
        else if ((playerY < enemyY) && (yDiff > xDiff))
        {
            At_Down = true;
            shootingDirection = -Vector2.up;
            if (playerX > enemyX && posCorrect)
            {
                rb_2.linearVelocity = new Vector2(moveSpeed, 0);
            }
            else if (posCorrect)
            {
                rb_2.linearVelocity = new Vector2(-moveSpeed, 0);
            }
        }
        else if ((playerX > enemyX) && (xDiff > yDiff))
        {
            At_Right = true;
            shootingDirection = Vector2.right;
            if (playerY > enemyY && posCorrect) 
            {
                rb_2.linearVelocity = new Vector2(0, moveSpeed);
            }
            else if (posCorrect)
            {
                rb_2.linearVelocity = new Vector2(0, -moveSpeed);
            }
        }
        else if ((playerX < enemyX) && (xDiff > yDiff))
        {
            At_Left = true;
            shootingDirection = -Vector2.right;
            if (playerY > enemyY && posCorrect)
            {
                rb_2.linearVelocity = new Vector2(0, moveSpeed);
            }
            else if (posCorrect)
            {
                rb_2.linearVelocity = new Vector2(0, -moveSpeed);
            }
        }
        SetAnimFlags();

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            //RaycastHit2D hit = Physics2D.Raycast(shootpoint.position, shootingDirection, Mathf.Infinity, ~ignoreLayer_1);
            //if (hit.collider != null)
            //{
            //    if (hit.collider.CompareTag("Player_1"))
            //    {
            //        Player player_1 = hit.collider.GetComponent<Player>();
            //        //Debug.Log("Attack! Damage: " + attackDamage);
            //        //Debug.Log("Attack! Damage: " + hit.collider.tag);
            //        player_1.TakeDamage_hero(attackDamage);
            //        player_1.TakeHP_hero();
            //        /*
            //        bul_robot = Instantiate(pref_2, shootpoint.position, Quaternion.identity);
            //        if (bul_robot != null)
            //        {
            //            bul_robot.GetComponent<Rigidbody2D>().velocity = shootingDirection * bul_speed_2;
            //        }
            //        */
            //    }
            //    linerenderer.SetPosition(0, shootpoint.position);
            //    linerenderer.SetPosition(1, hit.point);
            //    linerenderer.enabled = true;
            //    Invoke("StopLine_1", 0.05f);
            //}
            //else
            //{
            //    linerenderer.SetPosition(0, shootpoint.position);
            //    linerenderer.SetPosition(1, shootpoint.position + new Vector3(shootingDirection.x, shootingDirection.y, 0) * 5);
            //}
            lastAttackTime = Time.time;
        }
    }
    void MoveTowardsPlayer()
    {
        Move();
        distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > stopDistance)
        {
            rb_2.linearVelocity = direction * moveSpeed;
        }
        else
        {
            rb_2.linearVelocity = Vector2.zero;
        }
        AnimationMove(player.position);
    }
    void ReturnToStartingPosition()
    {
        distanceToStart = Vector2.Distance(transform.position, startingPosition);
        Vector2 directionToStart = (startingPosition - transform.position).normalized;

        if (distanceToStart > 0.1f) // ��� �������������� ��������
        {
            rb_2.linearVelocity = directionToStart * moveSpeed;
        }
        else
        {
            rb_2.linearVelocity = Vector2.zero; 
        }
        AnimationMove(startingPosition);
    }
    void AnimationMove(Vector3 direction_point)
    {
        Vector2 distance = direction_point - transform.position;
        distanceToStart = Vector2.Distance(transform.position, startingPosition);
        if (Math.Abs(distance.x) - Math.Abs(distance.y) > 0)
        {
            if (distance.x < 0) { Left = true; }
            else { Right = true; }
        }
        else
        {
            if (distance.y < 0) { Down = true; }
            else { Up = true; }
        }

        if (distanceToStart < 0.1f)
        {
            Idle = true;
        }
        SetAnimFlags();
    }

    void ResetFlags()
    {
        At_Left = false;
        At_Right = false;
        At_Up = false;
        At_Down = false;
        Up = false;
        Down = false;
        Left = false;
        Right = false;
        Idle = false;
    }
    void SetAnimFlags()
    {
        animator.SetBool("Move_right", Right);
        animator.SetBool("Move_left", Left);
        animator.SetBool("Move_up", Up);
        animator.SetBool("Move_down", Down);
        animator.SetBool("Idle", Idle);
        animator.SetBool("At_right", At_Right);
        animator.SetBool("At_left", At_Left);
        animator.SetBool("At_up", At_Up);
        animator.SetBool("At_down", At_Down);

    }
    void Get_Health()
    {
        if (Health <= 0)
        {
            isDead = true;
        }
        else
        {
            isDead = false;
        }
        animator.SetBool("isDead", isDead);
    }
}

