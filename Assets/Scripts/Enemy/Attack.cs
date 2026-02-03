using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public class EnemyAI : MonoBehaviour
    {
        public Transform player; // Ссылка на объект игрока
        public float shootingRange = 10f; // Максимальная дистанция для стрельбы
        public float shootingDelay = 2f; // Время задержки между выстрелами
        public float fieldOfViewAngle = 45f; // Угол зрительного поля
        public GameObject bulletPrefab; // Префаб пули
        public Transform firePoint; // Точка, откуда будут вылетать пули

        private void Start()
        {
            StartCoroutine(ShootAtPlayer());
        }

        private IEnumerator ShootAtPlayer()
        {
            while (true)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);

                // Проверяем находимся ли мы в радиусе стрельбы и есть ли прямая видимость на игрока
                if (distanceToPlayer <= shootingRange && CanSeePlayer())
                {
                    Shoot();
                    yield return new WaitForSeconds(shootingDelay); // Ждем перед следующим выстрелом
                }
                else
                {
                    yield return null; // Ожидание следующего кадра
                }
            }
        }

        private void Shoot()
        {
            // Создаем пулю
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.AddForce(firePoint.up * 20f, ForceMode2D.Impulse); // Задаем скорость пули
            Debug.Log("Shoot at player!");
        }

        private bool CanSeePlayer()
        {
            // Проверяем угол между врагом и игроком
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            float angle = Vector2.Angle(transform.up, directionToPlayer);

            if (angle < fieldOfViewAngle / 2f)
            {
                // Проверяем на наличие препятствий между врагом и игроком
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, shootingRange);
                if (hit.collider != null && hit.collider.transform == player)
                {
                    return true; // Игрок на виду
                }
            }

            return false; // Игрок вне поля зрения или есть препятствия
        }

        private void Update()
        {
            // Логика поворота врага к игроку (по желанию)
            FacePlayer();
        }

        private void FacePlayer()
        {
            Vector2 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
    public class Bullet : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Проверка на попадание в игрока
            if (other.CompareTag("Player"))
            {
                // Логика обработки урона игроку
                // Например, вызвать метод повреждения у игрока
                Debug.Log("Player hit!");
                Destroy(gameObject); // Уничтожаем пулю
            }
            else
            {
                Destroy(gameObject, 2f); // Уничтожаем пулю через 2 секунды, если не попала в игрока
            }
        }
    }
}
