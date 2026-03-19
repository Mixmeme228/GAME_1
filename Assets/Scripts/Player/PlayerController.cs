using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// This class receives inputs from TadaInput and calls methods from other classes based on those inputs. It 
/// also handles movement.
/// </summary>
public class PlayerController : MonoBehaviour
{
    private PlayerPhysics _PlayerPhysics;
    private PlayerSkills _PlayerSkills;
    private PlayerAnimations _PlayerAnimations;
    private WeaponHandler _WeaponHandler;
    // Ignore never invoked message, it's 

    private void Update()
    {
        CheckIfMissingClasses();

        if (PauseController.isGamePaused)
            return;

        #region ---------------------------- SKILLS

        if (TadaInput.GetKeyDown(TadaInput.ThisKey.Dash) && _PlayerPhysics.Velocity.sqrMagnitude > 0)
            _PlayerSkills.Dash();

        #endregion

        #region ---------------------------- ANIMATIONS

        _PlayerAnimations.PlayMoveAnimationsByMoveInputAndLookDirection(TadaInput.MoveAxisRawInput);

        _PlayerAnimations.SetAnimationSpeed(PlayerAnimations.AnimName.WalkForward, _PlayerPhysics.Velocity.magnitude);
        _PlayerAnimations.SetAnimationSpeed(PlayerAnimations.AnimName.WalkBackwards, _PlayerPhysics.Velocity.magnitude);

        #endregion

        #region ---------------------------- WEAPON ACTIONS

        if (TadaInput.GetKey(TadaInput.ThisKey.PrimaryAction))
            _WeaponHandler.UseWeapon(WeaponHandler.ActionType.Primary);

        if (TadaInput.GetKeyUp(TadaInput.ThisKey.PrimaryAction))
            _WeaponHandler.UseWeapon(WeaponHandler.ActionType.Primary, false);

        if (TadaInput.GetKeyDown(TadaInput.ThisKey.SecondaryAction))
            _WeaponHandler.UseWeapon(WeaponHandler.ActionType.Secondary);

        if (TadaInput.GetKeyUp(TadaInput.ThisKey.SecondaryAction))
            _WeaponHandler.UseWeapon(WeaponHandler.ActionType.Secondary, false);

        #endregion

        #region ---------------------------- WEAPON USE RATE

        if (TadaInput.GetKeyDown(TadaInput.ThisKey.NextUseRate))
            _WeaponHandler.SwitchUseRate(Weapon.SwitchUseRateType.Next);

        if (TadaInput.GetKeyDown(TadaInput.ThisKey.PreviousUseRate))
            _WeaponHandler.SwitchUseRate(Weapon.SwitchUseRateType.Previous);

        #endregion

        #region ---------------------------- WEAPON SWITCH

        if (Input.GetKeyDown(KeyCode.Alpha1))
            _WeaponHandler.SwitchWeapon(WeaponHandler.WeaponSwitchMode.ByIndex, 0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            _WeaponHandler.SwitchWeapon(WeaponHandler.WeaponSwitchMode.ByIndex, 1);

        if (TadaInput.GetKeyDown(TadaInput.ThisKey.NextWeapon))
            _WeaponHandler.SwitchWeapon(WeaponHandler.WeaponSwitchMode.Next);

        if (TadaInput.GetKeyDown(TadaInput.ThisKey.PreviousWeapon))
            _WeaponHandler.SwitchWeapon(WeaponHandler.WeaponSwitchMode.Previous);

        #endregion
    }
    private void Awake()
    {
        Initialize();
    }
    private void Initialize()
    {
        TryGetComponent(out _PlayerAnimations);
        TryGetComponent(out _WeaponHandler);
        TryGetComponent(out _PlayerPhysics);
        TryGetComponent(out _PlayerSkills);
    }
    private void CheckIfMissingClasses()
    {
        if (_PlayerAnimations == null || _WeaponHandler == null || _PlayerPhysics == null || _PlayerSkills == null)
        {
            Debug.LogWarning(gameObject.name + ": Missing behaviour classes!");
            return;
        }
    }
}