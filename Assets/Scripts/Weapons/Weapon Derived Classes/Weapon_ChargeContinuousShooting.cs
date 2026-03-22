using UnityEngine;

public class Weapon_LaserBeam : Weapon
{
    [Header("Laser")]
    public LineRenderer laserLine;
    [Tooltip("Точка откуда выходит луч (дуло)")]
    public Transform laserSpawnPoint;
    [Tooltip("Максимальная длина луча")]
    public float laserMaxLength = 20f;
    [Tooltip("Слои по которым луч попадает")]
    public LayerMask hitLayers;
    [Tooltip("Урон в секунду")]
    public float damagePerSecond = 15f;

    [Header("Charged Laser")]
    public float chargedDamageMultiplier = 3f;
    public float chargedWidth = 0.25f;
    public float chargedDuration = 2f;

    [Header("Charge FX")]
    public GameObject chargingPFX;
    public SoundHandlerLocal chargingSFX;
    public SoundHandlerLocal laserSFX;

    [Header("Hit FX")]
    public GameObject hitFX;

    // ── Приватные ──────────────────────────────────────────────────────────
    private bool isReceivingInput = false;
    private bool isCharging = false;
    private bool isChargedFiring = false;
    private float chargingTime = 0f;
    private float chargedTimer = 0f;
    private bool primaryHeld = false;

    private GameObject hitFXInstance;
    private Camera _cam;

    private const float CHARGE_DURATION = 2f;

    // ══════════════════════════════════════════════════════════════════════
    protected override void Awake()
    {
        base.Awake();
        useRateValues = new float[] { 0f };
        _cam = Camera.main;
    }

    private void OnEnable()
    {
        if (laserLine != null) laserLine.enabled = false;
    }

    private void OnDisable()
    {
        StopLaser();
    }

    // ══════════════════════════════════════════════════════════════════════
    protected override void Update()
    {
        base.Update();

        if (primaryHeld)
            FireLaser(damagePerSecond, 0.05f);

        if (isReceivingInput && !isChargedFiring)
        {
            OnChargingStart();
            chargingTime += Time.deltaTime;
            OnCharging(chargingTime);
            if (chargingTime >= CHARGE_DURATION)
                OnChargingEnd();
        }

        if (!isReceivingInput && isCharging)
            OnChargeCancel();

        if (isChargedFiring)
        {
            FireLaser(damagePerSecond * chargedDamageMultiplier, chargedWidth);
            chargedTimer -= Time.deltaTime;
            if (chargedTimer <= 0f)
                StopChargedFire();
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    public override void PrimaryAction(bool value)
    {
        base.PrimaryAction(value);
        primaryHeld = value;

        if (value)
        {
            if (laserLine != null) laserLine.enabled = true;
            laserSFX?.PlaySound();
        }
        else
        {
            StopLaser();
        }
    }

    public override void SecondaryAction(bool value)
    {
        base.SecondaryAction(value);

        if (chargingPFX == null || chargingSFX == null)
        {
            Debug.LogWarning(gameObject.name + ": missing charge prefabs!");
            return;
        }

        isReceivingInput = value;
    }

    // ══════════════════════════════════════════════════════════════════════
    private Vector2 GetAimDirection()
    {
        if (_cam == null) _cam = Camera.main;
        if (_cam == null || laserSpawnPoint == null) return Vector2.right;

        Vector2 mouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouseWorld - (Vector2)laserSpawnPoint.position;

        return dir.sqrMagnitude > 0.001f ? dir.normalized : Vector2.right;
    }

    // ══════════════════════════════════════════════════════════════════════
    private void FireLaser(float dps, float width)
    {
        if (laserLine == null || laserSpawnPoint == null) return;

        laserLine.startWidth = width;
        laserLine.endWidth = width * 0.4f;

        Vector2 origin = laserSpawnPoint.position;
        Vector2 direction = GetAimDirection();

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, laserMaxLength, hitLayers);

        Vector3 endPoint;

        if (hit.collider != null)
        {
            endPoint = hit.point;

            float dmg = dps * Time.deltaTime;
            ApplyDamage(hit.collider.gameObject, dmg);

            if (hitFX != null)
            {
                if (hitFXInstance == null)
                    hitFXInstance = Instantiate(hitFX);
                hitFXInstance.SetActive(true);
                hitFXInstance.transform.position = hit.point;
            }
        }
        else
        {
            endPoint = (Vector3)origin + (Vector3)(direction * laserMaxLength);
            if (hitFXInstance != null) hitFXInstance.SetActive(false);
        }

        laserLine.SetPosition(0, laserSpawnPoint.position);
        laserLine.SetPosition(1, endPoint);
    }

    private void ApplyDamage(GameObject target, float dmg)
    {
        // Дверь
        LaserDoor door = target.GetComponentInParent<LaserDoor>();
        if (door != null) { door.HeatUp(); return; }

        // Босс — лазер бьёт по щиту, потом по телу
        BossAI2 boss = target.GetComponentInParent<BossAI2>();
        if (boss != null)
        {
            if (boss.ShieldActive) boss.TakeShieldDamage(dmg);
            else boss.TakeDamage(dmg);
            return;
        }

        // AlienAI
        AlienAI alien = target.GetComponentInParent<AlienAI>();
        if (alien != null) { alien.TakeDamage(dmg); return; }

        // Robot
        Robot robot = target.GetComponentInParent<Robot>();
        if (robot != null) { robot.TakeDamage(dmg); return; }

        // ZombieAI
        ZombieAI zombie = target.GetComponentInParent<ZombieAI>();
        if (zombie != null) { zombie.TakeDamage(dmg); return; }
    }

    // ══════════════════════════════════════════════════════════════════════
    private void StopLaser()
    {
        primaryHeld = false;
        if (laserLine != null) laserLine.enabled = false;
        if (hitFXInstance != null) hitFXInstance.SetActive(false);
        laserSFX?.StopSound();
    }

    private void StopChargedFire()
    {
        isChargedFiring = false;
        chargedTimer = 0f;
        StopLaser();
    }

    // ══════════════════════════════════════════════════════════════════════
    private void OnChargingStart()
    {
        if (!isCharging)
        {
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
        isCharging = false;
        isChargedFiring = true;
        chargingTime = 0f;
        chargedTimer = chargedDuration;

        chargingPFX.transform.localScale = Vector2.one;
        chargingPFX.SetActive(false);
        chargingSFX.StopSound();

        CameraShake.Shake(duration: 0.2f, shakeAmount: 1f, decreaseFactor: 3f);

        if (laserLine != null) laserLine.enabled = true;
        laserSFX?.PlaySound();
    }

    private void OnChargeCancel()
    {
        isCharging = false;
        chargingTime = 0f;
        CameraShake.Shake(0f, 0f, 0f);
        chargingSFX.StopSound();
        chargingPFX.transform.localScale = Vector2.one;
        chargingPFX.SetActive(false);
    }
}