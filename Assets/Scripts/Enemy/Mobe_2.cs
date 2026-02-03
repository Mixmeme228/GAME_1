using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Mobe_2 : Enemy_AI
{
    private Transform player_pos; // Ссылка на игрока
    private Vector3 startingPosition; // Начальная позиция врага
    private Animator animator;
    private float moveSpeed = 2f;
    public float health_mobe;
    public bool isDie_m;
    public bool IsAttacking_m = false;
    public bool IsWalking_m = false;
    public bool IsAttackUp_m;
    public bool IsAttackDown_m;
    public bool IsAttackLeft_m;
    public bool IsAttackRight_m;
    public bool IsWalkUp_m;
    public bool IsWalkDown_m;
    public bool IsWalkLeft_m;
    public bool IsWalkRight_m;
    public bool IsDeathUp_m;
    public bool IsDeathDown_m;
    public bool IsDeathLeft_m;
    public bool IsDeathRight_m;
    public bool IsStop_m;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb_2 = GetComponent<Rigidbody2D>();
        player_pos = GameObject.FindGameObjectWithTag("Player_1").transform;
        InvokeRepeating("UpdatePath", 0f, 0.5f);
        health_mobe = GetComponent<Enemy_1>().health_enemy;
    }

    void Update()
    {
        Move();
        rb_2.velocity = direction * moveSpeed;
        IsAttacking_m = false;
        IsWalking_m = true;
        if (Player.Instance.IsAttacking_() == true)
        {
            rb_2.velocity = Vector3.zero;
            IsAttacking_m = true;
            IsWalking_m = false;
        }
        Get_Health();
    }
    void Get_Health()
    {
        if (health_mobe <= 0)
        {
            isDie_m = true;
        }
        else
        {
            isDie_m = false;
        }
    }
    //прописать код для смены анимации
}
