using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Sword : MonoBehaviour
{
    public event EventHandler OnSwordSwing; 
    private PolygonCollider2D _polygonCollider2D;
    private void Awake()
    {
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
    }
    private void Start()
    {
        AttackColliderTurnOff();
    }
    public void AttackColliderTurnOff()
    {
        _polygonCollider2D.enabled = false;
    }
    private void AttackColliderTurnOn()
    {
        _polygonCollider2D.enabled = true;

    }
    private void AttackColliderTurnOffOn()
    {
        AttackColliderTurnOff();
        AttackColliderTurnOn();
    }
    public void Attack()
    {
        AttackColliderTurnOffOn();
        OnSwordSwing?.Invoke(this, EventArgs.Empty);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //нанесение урона врагу
    }
}
