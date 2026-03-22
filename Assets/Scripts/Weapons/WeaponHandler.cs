using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public enum ActionType { Primary, Secondary }
    public enum WeaponSwitchMode { Next, Previous, ByIndex }

    [Tooltip("Все оружия игрока (включая выключенные — они разблокируются при подборе)")]
    public Weapon[] weapons;

    private Weapon currentWeapon;
    private int currentWeaponIndex;
    private bool isUsingPrimaryAction;
    private bool isUsingSecondaryAction;

    // Только разблокированные оружия
    private List<Weapon> unlockedWeapons = new List<Weapon>();

    // ══════════════════════════════════════════════════════════════════════
    private void Start()
    {
        // В старт добавляем только те, что были активны изначально
        foreach (var w in weapons)
        {
            if (w != null && w.gameObject.activeSelf)
                unlockedWeapons.Add(w);
            else if (w != null)
                w.gameObject.SetActive(false); // убеждаемся что выключены
        }

        SwitchWeapon(WeaponSwitchMode.ByIndex);
        SwitchUseRate(Weapon.SwitchUseRateType.ByIndex);
    }

    // ══════════════════════════════════════════════════════════════════════
    #region Pickup

    /// <summary>
    /// Разблокировать оружие по ссылке на Weapon-компонент.
    /// Вызывай из WeaponPickup: handler.UnlockWeapon(weaponComponent)
    /// </summary>
    public void UnlockWeapon(Weapon weapon)
    {
        if (weapon == null) return;

        // Уже разблокировано?
        if (unlockedWeapons.Contains(weapon))
        {
            Debug.Log($"[WeaponHandler] {weapon.gameObject.name} уже разблокировано.");
            return;
        }

        // Оружие должно быть в массиве weapons (уже висит на игроке выключенным)
        bool found = false;
        foreach (var w in weapons)
        {
            if (w == weapon) { found = true; break; }
        }

        if (!found)
        {
            Debug.LogWarning($"[WeaponHandler] {weapon.gameObject.name} не найдено в массиве weapons!");
            return;
        }

        unlockedWeapons.Add(weapon);

        Debug.Log($"[WeaponHandler] Разблокировано: {weapon.gameObject.name}. Всего: {unlockedWeapons.Count}");

        // Переключаемся на только что подобранное
        SwitchWeapon(WeaponSwitchMode.ByIndex, unlockedWeapons.Count - 1);
    }

    #endregion

    // ══════════════════════════════════════════════════════════════════════
    private void DisableAllWeapons()
    {
        foreach (var w in unlockedWeapons)
            if (w != null) w.gameObject.SetActive(false);
    }

    public void SwitchWeapon(WeaponSwitchMode mode, int index = 0)
    {
        if (unlockedWeapons.Count == 0) return;

        DisableAllWeapons();

        switch (mode)
        {
            case WeaponSwitchMode.Next:
                currentWeaponIndex = (currentWeaponIndex + 1) % unlockedWeapons.Count;
                break;
            case WeaponSwitchMode.Previous:
                currentWeaponIndex = (currentWeaponIndex - 1 + unlockedWeapons.Count) % unlockedWeapons.Count;
                break;
            case WeaponSwitchMode.ByIndex:
                if (index >= 0 && index < unlockedWeapons.Count)
                    currentWeaponIndex = index;
                break;
        }

        currentWeapon = unlockedWeapons[currentWeaponIndex];
        currentWeapon.gameObject.SetActive(true);
    }

    public void SwitchUseRate(Weapon.SwitchUseRateType type, int index = 0, bool applyToAllWeapons = true)
    {
        if (applyToAllWeapons)
        {
            foreach (var w in unlockedWeapons)
                if (w != null) w.SwitchUseRate(type, index);
            return;
        }

        currentWeapon?.SwitchUseRate(type, index);
    }

    public void UseWeapon(ActionType type, bool value = true)
    {
        switch (type)
        {
            case ActionType.Primary:
                if (currentWeapon != null && !isUsingSecondaryAction)
                {
                    isUsingPrimaryAction = value;
                    currentWeapon.PrimaryAction(value);
                }
                break;
            case ActionType.Secondary:
                if (currentWeapon != null && !isUsingPrimaryAction)
                {
                    isUsingSecondaryAction = value;
                    currentWeapon.SecondaryAction(value);
                }
                break;
        }
    }
}