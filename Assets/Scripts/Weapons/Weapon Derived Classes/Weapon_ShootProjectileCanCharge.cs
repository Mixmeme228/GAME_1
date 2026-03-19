using UnityEngine;

public class Weapon_ShootProjectileCanCharge : Weapon
{
    public GameObject basicProjectilePrefab;
    public GameObject chargedProjectilePrefab;
    public Transform projectileSpawnPoint;
    public GameObject chargingPFX;
    public SoundHandlerLocal chargingSFX;

    private WeaponAnim_ShootProjectileCanCharge anim;
    private Projectile primaryProjectile;
    private Projectile secondaryProjectile;
    private bool isReceivingInput = false;
    private bool isCharging = false;
    private float chargingTime;

    private const float CHARGE_DURATION = 2f;

    protected override void Awake()
    {
        base.Awake();
        useRateValues = new float[] { 1.125f, 0.05f };
        TryGetComponent(out anim);
    }

    protected override void Update()
    {
        base.Update();
        if (isReceivingInput)
        {
            OnChargingStart();

            chargingTime += Time.deltaTime;

            OnCharging(chargingTime);

            if (chargingTime >= CHARGE_DURATION)
                OnChargingEnd();
        }

        if (!isReceivingInput && isCharging)
            OnChargeCancel();
    }

    private void OnEnable()
    {
        SpawnProjectiles();
    }

    protected override void OnCanUse()
    {
        base.OnCanUse();
        SpawnProjectiles();
        if (anim != null)
            anim.PlayAnimation(WeaponAnim_ShootProjectileCanCharge.Animation.Idle);
    }

    private void SpawnProjectiles()
    {
        if (basicProjectilePrefab == null || chargedProjectilePrefab == null || projectileSpawnPoint == null)
        {
            Debug.LogError(gameObject.name + " missing prefabs or spawnPoint!");
            return;
        }

        if (primaryProjectile == null)
        {
            primaryProjectile = Instantiate(basicProjectilePrefab, projectileSpawnPoint.position,
            projectileSpawnPoint.rotation, projectileSpawnPoint).GetComponent<Projectile>();

            primaryProjectile.SetActive(false);
        }

        if (secondaryProjectile == null)
        {
            secondaryProjectile = Instantiate(chargedProjectilePrefab, projectileSpawnPoint.position,
            projectileSpawnPoint.rotation, projectileSpawnPoint).GetComponent<Projectile>();

            secondaryProjectile.SetActive(false);
        }
    }

    public override void PrimaryAction(bool value)
    {
        base.PrimaryAction(value);

        if (primaryProjectile != null && canUse)
        {
            if (anim != null)
                anim.PlayAnimation(WeaponAnim_ShootProjectileCanCharge.Animation.BasicShot);

            CameraShake.Shake(duration: 0.075f, shakeAmount: 0.1f, decreaseFactor: 3f);

            primaryProjectile.SetActive(true);

            primaryProjectile.Fire();

            primaryProjectile = null;

            canUse = false;
        }
    }

    public override void SecondaryAction(bool value)
    {
        base.SecondaryAction(value);

        if (!canUse)
        {
            isReceivingInput = false;
            return;
        }

        if ((secondaryProjectile == null || chargingPFX == null || chargingSFX == null))
        {
            Debug.LogWarning(gameObject.name + ": missing prefabs!");
            return;
        }

        isReceivingInput = value;
    }

    private void OnChargingStart()
    {
        if (!isCharging)
        {
            if (anim != null)
                anim.PlayAnimation(WeaponAnim_ShootProjectileCanCharge.Animation.Charging);

            isCharging = true;

            CameraShake.Shake(duration: CHARGE_DURATION, shakeAmount: 0.065f, decreaseFactor: 1f);

            chargingPFX.SetActive(true);

            chargingSFX.PlaySound();
        }
    }

    private void OnCharging(float t)
    {
        chargingPFX.transform.localScale = Vector2.one * t;
    }

    private void OnChargingEnd()
    {
        if (anim != null)
            anim.PlayAnimation(WeaponAnim_ShootProjectileCanCharge.Animation.ChargedShot);

        isCharging = false;

        canUse = false;

        chargingTime = 0.0f;

        CameraShake.Shake(duration: 0.2f, shakeAmount: 1f, decreaseFactor: 3f);

        secondaryProjectile.SetActive(true);

        secondaryProjectile.Fire();

        secondaryProjectile = null;

        chargingPFX.transform.localScale = Vector2.one;

        chargingPFX.SetActive(false);
    }

    private void OnChargeCancel()
    {
        if (anim != null)
            anim.PlayAnimation(WeaponAnim_ShootProjectileCanCharge.Animation.Idle);

        isCharging = false;

        chargingTime = 0.0f;

        CameraShake.Shake(0f, 0f, 0f);

        chargingSFX.StopSound();

        chargingPFX.transform.localScale = Vector2.one;

        chargingPFX.SetActive(false);
    }
}
