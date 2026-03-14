//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using UnityEngine;
//using UnityEngine.Rendering;

////������ ������ ��������� ��������� ������ ����� ������� ������
//public class Bullet : MonoBehaviour
//{
//    public bool isGrenada;
//    public bool isBullet;
//    public float targetPosX;
//    public float targetPosY;
//    public Transform area;
//    public Rigidbody2D rb;
//    public float speed_bullet = 15f;
//    public Vector2 direction_bullet;
//    public Vector2 PointPos;
//    public float CenterX; //���������� x ������������� �����, ������������ �������
//    public float CenterY; //���������� y ������������� �����, ������������ �������

//    private void IdentifyTargetOrDirection()
//    {
//        if (isGrenada == true) //���� ��� �������
//        {
//            //���������� � ����� ������� ��������� 
//            //���������� �������������� ������ �������
//            //�������� �������� ���������� ������� � � ��������
//            if (Boss1.Instance.blastAttack_1 == true)
//            {
//                area = GameObject.FindGameObjectWithTag("Blast1").transform; 
//                targetPosX = Random.Range(area.position.x - 2.5f, area.position.x + 2.5f);
//                targetPosY = Random.Range(area.position.y - 2.5f, area.position.y + 2.5f);
//            }
//            if (Boss1.Instance.blastAttack_2 == true)
//            {
//                area = GameObject.FindGameObjectWithTag("Blast2").transform;
//                targetPosX = Random.Range(area.position.x - 2.5f, area.position.x + 2.5f);
//                targetPosY = Random.Range(area.position.y - 2.5f, area.position.y + 2.5f);
//            }
//            if (Boss1.Instance.blastAttack_3 == true)
//            {
//                area = GameObject.FindGameObjectWithTag("Blast3").transform;
//                targetPosX = Random.Range(area.position.x - 2.5f, area.position.x + 2.5f);
//                targetPosY = Random.Range(area.position.y - 2.5f, area.position.y + 2.5f);
//            }
//            if (Boss1.Instance.blastAttack_4 == true)
//            {
//                area = GameObject.FindGameObjectWithTag("Blast4").transform;
//                targetPosX = Random.Range(area.position.x - 2.5f, area.position.x + 2.5f);
//                targetPosY = Random.Range(area.position.y - 2.5f, area.position.y + 2.5f);
//            }
//        }
//        if (isBullet == true)
//        {
//            //� ����������� �� ���� ������� ����� � ��������� ���� ���������� ����������� � ��������
//            if (Mathf.Abs(CenterX - PointPos.x) <= Mathf.Abs(CenterY - PointPos.y))
//            {
//                if (CenterY - PointPos.y < 0f)
//                {
//                    direction_bullet = Vector2.right;
//                }
//                if (CenterY - PointPos.y > 0f)
//                {
//                    direction_bullet = -Vector2.right;
//                }
//            }
//            else
//            {
//                if (CenterY - PointPos.y < 0f)
//                {
//                    direction_bullet = Vector2.up;
//                }
//                if (CenterY - PointPos.y > 0f)
//                {
//                    direction_bullet = -Vector2.up;
//                }
//            }
//        }
//    }
//    //����� ������� ������ ���������� ��� ������: ���� ��� �������
//    void Awake()
//    {
//        if (gameObject.tag == "BulletBoss")
//        {
//            isBullet = true;
//            isGrenada = false;
//        }
//        if (gameObject.tag == "GrenadaBoss")
//        {
//            isBullet = false;
//            isGrenada = true;
//        }
//        //PointPos = GameObject.FindGameObjectWithTag("PointAttackBoss").transform; ������� ������ �� ����
//    }
//    //�������������� ��������� ������
//    void Start()
//    {
//        rb = GetComponent<Rigidbody2D>();
//    }
//    //� ���� ������ �� ��������� �������� ���� ��� �������
//    void FixedUpdate()
//    {
//        PointPos = PointMoveBoss.Instance.ReturnPosition(); //���������� ��������� ����� ��������
//        IdentifyTargetOrDirection();
//        if (isGrenada == true)
//        {
//            rb.MovePosition(new Vector2(targetPosX, targetPosY));
//            //���� �������, �� �� ���������� � ��������� � ����������� ����������
//        }
//        if (isBullet == true)
//        {
//            rb.linearVelocity = direction_bullet.normalized * speed_bullet;
//            //���� ����, �� ��� ��������� �� ���������� ����������� � ����������� ���������
//        }
//    }
//    private void OnCollisionEnter2D(Collision2D collision)
//    {
//        if (isBullet == true)
//        {
//            //���� ��� ������ �������� �����, �� ������ ���� ������ ����� ������ � ��� ������, ���� ���� ������ � ����
//            if (collision.gameObject.tag == "Player_1") //��������� ��� �������
//            {
//                Player hero = collision.gameObject.GetComponent<Player>(); //�������� ������ � ����������-������� ������
//                if (hero != null)
//                {
//                    hero.TakeDamage_hero(Boss1.Instance.attackDamage); //������� ����
//                }
//            }
//            Destroy(gameObject, 1f); //���������� ����
//        }
//        if (isGrenada == true)
//        {
//            //���� ��������� ����� ��������� � ������� BlastAttack
//            Destroy(gameObject, 5f); //���������� �������
//        }
//    }
//}
