using Game.Utils_1;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Utils_1;
using UnityEngine.UIElements;
using System.Xml;

public class Boss3 : MonoBehaviour
{
    public static Boss3 Instance { get; private set; }

    public Rigidbody2D rb_3;
    public Animator anim_;
    public float health_3;
    public float sec;
    private float attackCooldown_b3 = 7f; 
    public float attackDamage_b3 = 20f; 
    private float lastAttackTime_b3;
    private float goCooldown_b3 = 3f;
    private float lastGoTime_b3;
    private Vector3 startingPosition_3;
    private Vector2 leftDownPoint;
    private Vector2 leftUpPoint;
    private Vector2 rightUpPoint;
    private float min_pos_x;
    private float max_pos_x;
    private float min_pos_y;
    private float max_pos_y;
    private Vector3 Center_Left_Down;
    private float cell_length;
    private float cell_width;
    private Vector3 NewPos;
    //private string current_active;
    //private string next_active;
    private int count_attack;
    private int max_count_attack;
    private bool isWait;

    public GameObject tur_left;
    public GameObject tur_right;
    public GameObject tur_up;
    public GameObject tur_down;
    public Dictionary<char, GameObject> turrets;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rb_3 = GetComponent<Rigidbody2D>();
        anim_ = GetComponent<Animator>(); 
        health_3 = GetComponent<Enemy_2>().boss_health;
        startingPosition_3 = transform.position;
        float length = rightUpPoint.x - leftUpPoint.x;
        float width = leftUpPoint.y - leftUpPoint.y;
        int a = 6;
        int b = 4;
        cell_length = length / a;
        cell_width = width / b;
        Center_Left_Down.x = leftDownPoint.x + cell_length / 2;
        Center_Left_Down.y = leftDownPoint.y + cell_width / 2;
        Center_Left_Down.z = 0;
        //current_active = "";
        //next_active = "";
        isWait = true;
        count_attack = 0;
        max_count_attack = 7;
        turrets = new Dictionary<char, GameObject> { { 'l', tur_left }, { 'r', tur_right }, { 'u', tur_up }, { 'd', tur_down } };
    }
    private void AppearAttack()
    {
        /*
        string list_dir = "lrud";
        next_active = Common2.GetRandomirStringOf12(list_dir.Length);
        if (next_active != current_active)
            current_active = next_active;
        else
        {
            while (next_active == current_active)
                next_active = Common2.GetRandomirStringOf12(list_dir.Length);
            current_active = next_active;
        }
        for (int j = 0; j < list_dir.Length; j++)
        {
            if (current_active[j] == '0') 
                turrets.Remove(list_dir[j]);
        }
        */
        int numberAttack;
        numberAttack = UnityEngine.Random.Range(1, 12);
        Vector3 pos_1;
        Vector3 pos_2;
        Vector3 pos_3;
        Vector3[] array_pos;
        int i;
        switch (numberAttack)
        {
            case 1:
                pos_1 = new Vector3(Center_Left_Down.x, Center_Left_Down.y + cell_width, 0);
                pos_2 = new Vector3(Center_Left_Down.x, Center_Left_Down.y + 2 * cell_width, 0);
                pos_3 = new Vector3(Center_Left_Down.x, Center_Left_Down.y + 3 * cell_width, 0);
                array_pos = new Vector3[3] { pos_1, pos_2, pos_3 };
                i = 0;
                foreach (KeyValuePair<char, GameObject> entry in turrets)
                {
                    Instantiate(entry.Value, array_pos[i], Quaternion.identity);
                    i++;
                }
                break;
            case 2:
                pos_1 = new Vector3(Center_Left_Down.x + 5 * cell_length, Center_Left_Down.y, 0);
                pos_2 = new Vector3(Center_Left_Down.x + 5 * cell_length, Center_Left_Down.y + cell_width, 0);
                pos_3 = new Vector3(Center_Left_Down.x + 5 * cell_length, Center_Left_Down.y + 3 * cell_width, 0);
                array_pos = new Vector3[3] { pos_1, pos_2, pos_3 };
                i = 0;
                foreach (KeyValuePair<char, GameObject> entry in turrets)
                {
                    Instantiate(entry.Value, array_pos[i], Quaternion.identity);
                    i++;
                }
                break;
        }
    }

    private void CheckAndMovePoint()
    {
        Vector3 NewPos;
        int attempts = 0;
        const int maxAttempts = 10;

        do
        {
            NewPos = Common2.GetRandomirPoint_1(leftDownPoint.x, rightUpPoint.x, leftDownPoint.y, leftUpPoint.y);
            attempts++;
        }
        while (attempts < maxAttempts && transform.position == NewPos);

        rb_3.MovePosition(NewPos);
    }

    /*
    void Update()
    {
        if (count_attack > max_count_attack)
        {
            while (Time.time >= lastAttackTime_b3 + attackCooldown_b3)
            {
                isWait = true;
                rb_3.velocity = Vector2.zero;
                lastAttackTime_b3 = Time.time;
                count_attack = 0;
            }
        }
        else
        {
            while (transform.position == NewPos)
            {
                NewPos = Common2.GetRandomirPoint_1(leftDownPoint.x, rightUpPoint.x, leftDownPoint.y, leftUpPoint.y);
            }
            rb_3.MovePosition(NewPos);
        }
    }
    */

    void Update()
    {
        CheckAndMovePoint();
        if (Time.time >= lastAttackTime_b3 + attackCooldown_b3)
        {
            AppearAttack();
            lastAttackTime_b3 = Time.time;
        }
    }

    /*
    else
        {
            isWait = false;
            CheckAndMovePoint();
            if (Time.time >= lastAttackTime_1_b3 + attackCooldown_1_b3)
                NewPos = Common2.GetRandomirPoint_1(leftDownPoint.x, rightUpPoint.x, leftDownPoint.y, leftUpPoint.y);
            if (transform.position != NewPos)
            {
                rb_3.MovePosition(NewPos);
            }
        }
    */
}
