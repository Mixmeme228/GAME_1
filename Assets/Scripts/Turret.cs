using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private Animator anim_t;
    
    public bool IsAppear;
    public bool IsWait;
    public bool IsDisAppear;

    private Vector2 attack_dir;
    public float attackSpeed = 20f;
    public GameObject prefBullet;
    public Boss3 boos3;

    private void Awake()
    {
        //anim_t = GetComponent<Animator>();
    }
    private void Start()
    {
        if (gameObject.tag == "TurretUp")
        {
            attack_dir = Vector2.up;
        }
        if (gameObject.tag == "TurretDown")
        {
            attack_dir = Vector2.down;
        }
        if (gameObject.tag == "TurretLeft")
        {
            attack_dir = Vector2.left;
        }
        if (gameObject.tag == "TurretRight")
        {
            attack_dir = Vector2.right;
        }
        Invoke("ActivationAttack", 2f);
    }
    private void ActivationAttack()
    {
        GameObject obj = Instantiate(prefBullet, transform.position, Quaternion.identity);
        obj.GetComponent<Rigidbody2D>().velocity = attack_dir * attackSpeed;
        //obj.GetComponent<Tranform>().Translate(attack_dir * attackSpeed);
        Destroy(obj, 3f);
        Destroy(gameObject, 3f);
    }
}
