using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunVisual : MonoBehaviour
{
    private Animator animGun;
    private SpriteRenderer spriteRendererGun;
    private const string is_up = "UpG";
    private const string is_down = "DownG";
    private const string is_left = "LeftG";
    private const string is_right = "RightG";
    private const string attack = "Attack";
    private void Awake()
    {
        animGun = GetComponent<Animator>();
        spriteRendererGun = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        animGun.SetBool(is_up, Player.Instance.IsShootingUp());
        animGun.SetBool(is_down, Player.Instance.IsShootingDown());
        animGun.SetBool(is_left, Player.Instance.IsShootingLeft());
        animGun.SetBool(is_right, Player.Instance.IsShootingRight());
        animGun.SetBool(attack, Gun.Instance.IsAttacking());
    }
}
