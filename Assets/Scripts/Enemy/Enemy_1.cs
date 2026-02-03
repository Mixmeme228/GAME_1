using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy_1 : MonoBehaviour
{
    public float health_enemy = 0f; // Здоровье врага
    public float HP_enemy = 1f;
    //public static Action Get_Damage_enemy;
    private void Awake()
    {
        if (GetComponent<Zombie>() != null || GetComponent<Robot>() != null || GetComponent<Mobe_2>() != null)
        {
            health_enemy = 100f;
        }
        if (GetComponent<Boss1>() != null || GetComponent<ButtonBoss>() != null)
        {
            health_enemy = 1000f;
        }
    }
    public void TakeDamage_enemy(float damage)
    {
        health_enemy -= damage;
        HP_enemy -= damage / 100f;
        Debug.Log("Enemy takes damage: " + damage + ". Current health: " + health_enemy);
        //Get_Damage_enemy?.Invoke();

        if (health_enemy <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        Debug.Log("Enemy has died!");
        // Логика смерти врага (например, перезагрузка сцены, анимации и т.д.)
        Destroy(gameObject); // Удаляем врага из игры
    }
}
