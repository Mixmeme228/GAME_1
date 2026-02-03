using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenada_hero : MonoBehaviour
{
    public static Grenada_hero Instance { get; private set; }
    private float grenada_speed = 2.0f;
    private Rigidbody2D RB;
    private Vector2 dir_move;
    public GameObject _damage;
    public Vector3 _damagePos;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        RB = GetComponent <Rigidbody2D>();
    }
    private void Update()
    {
        if (Player.Instance.NowIsShooting_2())
        {
            Attack();
        }
    }
    public void Attack()
    {
        if (Player.Instance.IsDropingGrenadaUp())
        {
            dir_move = Vector2.up;
        }
        if (Player.Instance.IsDropingGrenadaDown())
        {
            dir_move = - Vector2.up;
        }
        if (Player.Instance.IsDropingGrenadaRight())
        {
            dir_move = Vector2.right;
        }
        if (Player.Instance.IsDropingGrenadaLeft())
        {
            dir_move = - Vector2.right;
        }
        RB.velocity = dir_move * grenada_speed;
        //уменьшить или увеличить сопротивление воздуха
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            _damagePos = transform.position;
            Destroy(gameObject);
            Instantiate(_damage, _damagePos, Quaternion.identity);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                _damagePos = transform.position;
                Destroy(gameObject);
                Instantiate(_damage, _damagePos, Quaternion.identity);
            }
        }
    }
}
