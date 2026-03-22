using UnityEngine;

/// <summary>
/// Вешай на объект-пикап в сцене (иконка оружия на полу).
/// В инспекторе перетащи сюда Weapon-компонент с игрока (выключенный).
/// При подборе — объект пикапа удаляется, оружие на игроке включается.
/// </summary>
public class WeaponPickup : MonoBehaviour
{
    [Tooltip("Weapon-компонент на игроке который нужно разблокировать (перетащи сюда)")]
    public Weapon weaponToUnlock;

    [Tooltip("Тег игрока")]
    public string playerTag = "Player_1";

    [Tooltip("Эффект при подборе (необязательно)")]
    public GameObject pickupFX;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(playerTag)) return;

        WeaponHandler handler = col.GetComponent<WeaponHandler>();
        if (handler == null)
            handler = col.GetComponentInParent<WeaponHandler>();
        if (handler == null)
            handler = col.GetComponentInChildren<WeaponHandler>();

        if (handler == null)
        {
            Debug.LogWarning("[WeaponPickup] WeaponHandler не найден на игроке!");
            return;
        }

        if (weaponToUnlock == null)
        {
            Debug.LogWarning("[WeaponPickup] Не назначено weaponToUnlock!");
            return;
        }

        if (pickupFX != null)
            Instantiate(pickupFX, transform.position, Quaternion.identity);

        handler.UnlockWeapon(weaponToUnlock);

        Destroy(gameObject); // удаляем пикап со сцены
    }
}