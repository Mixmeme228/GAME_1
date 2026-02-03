using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    private Rigidbody2D rb;
    private Vector2 inputVector;

    private float lastAttackTime_1;
    private float attackCooldown_1 = 1f;

    [SerializeField] private float speed_player = 3f;
    public float health_hero = 100f;
    private Vector2 speed_to_axis;
    private bool togetherY;
    private bool togetherX;
    private bool isRunningUp;
    private bool isRunningDown;
    private bool isRunningLeftRight;
    private bool rev;
    private bool isAttacking_ = false;
    private bool isAttackingUp;
    private bool isAttackingDown;
    private bool isAttackingLeft;
    private bool isAttackingRight;
    private bool isUpDown;
    private bool isStandingUp;
    private bool isStandingDown;
    private bool isStandingRight;
    private bool isStandingLeft;
    public bool isShooting_1 = false;
    public bool isShooting_2 = false;
    private bool isShootingUp;
    private bool isShootingDown;
    private bool isShootingLeft;
    private bool isShootingRight;
    private bool isDropingGrenadaUp;
    private bool isDropingGrenadaDown;
    private bool isDropingGrenadaLeft;
    private bool isDropingGrenadaRight;

    public GameObject _gun;
    public GameObject _grenada;

    public event EventHandler TakeHP;
    public event EventHandler TakeCount;
    public event EventHandler TakeMed;
    public byte count_Key = 0;
    public byte count_Gre = 0;

    public bool IsAttacking_()
    {
        return isAttacking_;
    }
    public bool IsRunningUp()
    {
        return isRunningUp;
    }
    public bool IsRunningDown()
    {
        return isRunningDown;
    }
    public bool IsRunningLeftRight()
    {
        return isRunningLeftRight;
    }
    public bool Rev()
    {
        return rev;
    }
    public bool IsAttackingUp()
    {
        return isAttackingUp;
    }
    public bool IsAttackingDown()
    {
        return isAttackingDown;
    }
    public bool IsAttackingRight()
    {
        return isAttackingRight;
    }
    public bool IsAttackingLeft()
    {
        return isAttackingLeft;
    }
    public bool IsDropingGrenadaUp()
    {
        return isDropingGrenadaUp;
    }
    public bool IsDropingGrenadaDown()
    {
        return isDropingGrenadaDown;
    }
    public bool IsDropingGrenadaRight()
    {
        return isDropingGrenadaRight;
    }
    public bool IsDropingGrenadaLeft()
    {
        return isDropingGrenadaLeft;
    }
    public bool IsStandingUp()
    {
        return isStandingUp;
    }
    public bool IsStandingDown()
    {
        return isStandingDown;
    }
    public bool IsStandingRight()
    {
        return isStandingRight;
    }
    public bool IsStandingLeft()
    {
        return isStandingLeft;
    }
    public bool IsShootingUp()
    {
        return isShootingUp;
    }
    public bool IsShootingDown()
    {
        return isShootingDown;
    }
    public bool IsShootingRight()
    {
        return isShootingRight;
    }
    public bool IsShootingLeft()
    {
        return isShootingLeft;
    }
    public bool NowIsShooting_1()
    {
        return isShooting_1;
    }
    public bool NowIsShooting_2()
    {
        return isShooting_2;
    }
    public bool ReturnToIdle()
    {
        return !isShooting_1;
    }
    private void HandleMovement()
    {
        inputVector = inputVector.normalized;
        speed_to_axis = inputVector * (speed_player * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + speed_to_axis);
    }
    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        _gun.SetActive(false);
        if (_grenada != null)
        {
            _grenada.SetActive(false);
        }
    }
    private void FixedUpdate()
    {
        HandleMovement();
        moving_mode();
    }
    private void Update()
    {
        inputVector = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            inputVector.y = 1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            inputVector.y = -1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            inputVector.x = -1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            inputVector.x = 1f;
        }
        Animation();
    }
    public void moving_mode()
    {
        if (Input.GetKey(KeyCode.Alpha2))
        {
            isShooting_1 = true;
            _gun.SetActive(true);
            isShooting_2 = false;
            if (_grenada != null)
            {
                _grenada.SetActive(false);
            }
        }
        if (Input.GetKey(KeyCode.Alpha1))
        {
            isShooting_1 = false;
            _gun.SetActive(false);
            isShooting_2 = false;
            if (_grenada != null)
            {
                _grenada.SetActive(false);
            }
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            if (_grenada != null)
            {
                isShooting_2 = true;
                _grenada.SetActive(true);
            }
            isShooting_1 = false;
            _gun.SetActive(false);
        }
    }
    private void Animation()
    {
        isUpDown = false;

        isRunningUp = false;
        isRunningDown = false;
        isRunningLeftRight = false;
        rev = false;
        togetherY = false;
        togetherX = false;
        
        isAttackingUp = false;
        isAttackingDown = false;
        isAttackingLeft = false;
        isAttackingRight = false;

        isStandingUp = false;
        isStandingDown = false;
        isStandingRight = false;
        isStandingLeft = false;

        isShootingUp = false;
        isShootingDown = false;
        isShootingRight = false;
        isShootingLeft = false;

        isDropingGrenadaUp = false;
        isDropingGrenadaDown = false;
        isDropingGrenadaLeft = false;
        isDropingGrenadaRight = false;

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
        {
            togetherY = true;
        }
        if (Input.GetKey(KeyCode.W) && togetherY == false)
        {
            if (isShooting_1 == false && isShooting_2 == false)
            {
                isRunningUp = true;
            }
            else if (isShooting_1 == true)
            {
                isAttackingUp = true;
                isShootingUp = true;
            }
            else if (isShooting_2 == true)
            {
                isDropingGrenadaUp = true;
            }
            isUpDown = true;
        }
        if (Input.GetKeyUp(KeyCode.W) && isShooting_1)
        {
            isStandingUp = true;
            isShootingUp = true;
        }     
        if (Input.GetKey(KeyCode.S) && togetherY == false)
        {
            if (isShooting_1 == false && isShooting_2 == false)
            {
                isRunningDown = true;
            }
            else if (isShooting_1 == true)
            {
                isAttackingDown = true;
                isShootingDown = true;
            }
            else if (isShooting_2 == true)
            {
                isDropingGrenadaDown = true;
            }
            isUpDown = true;
        }
        if (Input.GetKeyUp(KeyCode.S) && isShooting_1)
        {
            isStandingDown = true;
            isShootingDown = true;
        }

        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            togetherX = true;
        }
        if (Input.GetKey(KeyCode.A) && togetherX == false)
        {
            if (isUpDown == false)
            {
                if (isShooting_1 == false && isShooting_2 == false)
                {
                    isRunningLeftRight = true;
                    rev = true;
                }
                else if (isShooting_1 == true)
                { 
                    isAttackingLeft = true;
                    isShootingLeft = true;
                }
                else if (isShooting_2 == true)
                {
                    isDropingGrenadaLeft = true;
                }
            } 
        }
        if (Input.GetKeyUp(KeyCode.A) && isShooting_1)
        {
            isStandingLeft = true;
            isShootingLeft = true;
        }

        if (Input.GetKey(KeyCode.D) && togetherX == false)
        {
            if (isUpDown == false)
            {
                if (isShooting_1 == false && isShooting_2 == false)
                {
                    isRunningLeftRight = true;
                    rev = false;
                }
                else if (isShooting_1 == true)
                { 
                    isAttackingRight = true;
                    isShootingRight = true;
                }
                else if (isShooting_2 == true)
                {
                    isDropingGrenadaRight = true;
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.D) && isShooting_1)
        {
            isStandingRight = true;
            isShootingRight = true;
        }
    }
    public void TakeDamage_hero(float damage)
    {
        health_hero -= damage; // Уменьшаем здоровье
        Debug.Log("Player takes damage: " + damage + ". Current health: " + health_hero);

        if (health_hero <= 0)
        {
            Die(); // Вызываем метод смерти, если здоровье ниже или равно нулю
        }
    }
    public void TakeHP_hero()
    {
        TakeHP?.Invoke(this, EventArgs.Empty);
    }
    public void TakeCountKey()
    {
        TakeCount?.Invoke(this, EventArgs.Empty);
    }
    public void TakeMedicine()
    {
        TakeMed?.Invoke(this, EventArgs.Empty);
    }
    private void Die()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(5);
        //добавить эффектов при помощи postprocessing
        //Debug.Log("Player has died!");
        // Логика смерти игрока (например, перезагрузка сцены, анимации и т.д.)
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            isAttacking_ = true;
            if (Time.time >= lastAttackTime_1 + attackCooldown_1)
            {
                if ((collision.gameObject.tag == "Enemy") && (collision.gameObject.GetComponent<Zombie>() != null))
                {
                    Enemy_1 en = collision.gameObject.GetComponent<Enemy_1>();
                    Zombie zom = collision.gameObject.GetComponent<Zombie>();
                    if (en != null)
                    {
                        Debug.Log("Attack! Damage: " + zom.attackDamage);
                        TakeDamage_hero(zom.attackDamage);
                        TakeHP_hero();
                    }
                }
                lastAttackTime_1 = Time.time;
            }
        }
        if (collision.gameObject.tag == "Key")
        {
            count_Key = +1;
            Destroy(collision.gameObject, 0.5f);
            TakeCountKey();
        }
        if (collision.gameObject.tag == "Medicine")
        {
            health_hero = +5f;
            Destroy(collision.gameObject, 0.5f);
            TakeMedicine();
        }
        if (collision.gameObject.tag == "GrenadaPlayer")
        {
            count_Gre += 1;
            if (count_Gre < 1)
            {
                collision.transform.SetParent(gameObject.transform);
                collision.gameObject.transform.position = new Vector3(1, 1, 0);
                _grenada = collision.gameObject;
                _grenada.SetActive(false);
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            isAttacking_ = true;
            if (Time.time >= lastAttackTime_1 + attackCooldown_1)
            {
                if ((collision.gameObject.tag == "Enemy") && (collision.gameObject.GetComponent<Zombie>() != null))
                {
                    Enemy_1 en = collision.gameObject.GetComponent<Enemy_1>();
                    Zombie zom = collision.gameObject.GetComponent<Zombie>();
                    if (en != null)
                    {
                        Debug.Log("Attack! Damage: " + zom.attackDamage);
                        TakeDamage_hero(zom.attackDamage);
                        TakeHP_hero();
                    }
                }
                lastAttackTime_1 = Time.time;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
            isAttacking_ = false;
    }
}
