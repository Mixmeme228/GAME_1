using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public enum ActionType { Primary, Secondary }
    public enum WeaponSwitchMode { Next, Previous, ByIndex }
    public Weapon[] weapons;
    private Weapon currentWeapon;
    private int currentWeaponIndex;
    private bool isUsingPrimaryAction;
    private bool isUsingSecondaryAction;
    private void Start()
    {
        SwitchWeapon(WeaponSwitchMode.ByIndex);
        SwitchUseRate(Weapon.SwitchUseRateType.ByIndex);
    }
    private void DisableAllWeapons()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
                weapons[i].gameObject.SetActive(false);
        }
    }
    public void SwitchWeapon(WeaponSwitchMode mode, int index = 0)
    {
        CheckWeaponsAvailability();
        DisableAllWeapons();
        switch (mode)
        {
            case WeaponSwitchMode.Next:
                currentWeaponIndex = ArraysHandler.GetNextIndex(currentWeaponIndex, weapons.Length);
                break;

            case WeaponSwitchMode.Previous:
                currentWeaponIndex = ArraysHandler.GetPreviousIndex(currentWeaponIndex, weapons.Length);
                break;

            case WeaponSwitchMode.ByIndex:
                if (index >= 0 && index <= weapons.Length - 1)
                    currentWeaponIndex = index;
                break;

            default:
                break;
        }
        currentWeapon = weapons[currentWeaponIndex];
        currentWeapon.gameObject.SetActive(true);
    }
    public void SwitchUseRate(Weapon.SwitchUseRateType type, int index = 0, bool applyToAllWeapons = true)
    {
        CheckWeaponsAvailability();
        if (applyToAllWeapons)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] != null)
                    weapons[i].SwitchUseRate(type, index);
            }
            return;
        }

        if (currentWeapon != null)
            currentWeapon.SwitchUseRate(type, index);
    }
    public void UseWeapon(ActionType type, bool value = true)
    {
        CheckWeaponsAvailability();

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

            default:
                break;
        }
    }
    private void CheckWeaponsAvailability()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null)
            {
                Debug.LogWarning(gameObject.name + " WeaponHandler: weapons missing, check array!");
                return;
            }
        }
    }
}
