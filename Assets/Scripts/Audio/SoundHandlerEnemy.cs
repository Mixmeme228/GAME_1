using UnityEngine;

public class SoundHandlerEnemy : SoundEmitter
{
    [TextArea(3, 6)]
    public string notes = "Handles all sounds for Robot enemy. Assign clips to each state slot. " +
                          "Sounds trigger automatically based on Robot state changes.";

    [Header("Enemy Sounds")]
    public Sound activateSound;   // Когда робот активируется
    public Sound chaseSound;      // Во время преследования (лупится)
    public Sound attackSound;     // При выстреле
    public Sound hitSound;        // Когда получает урон
    public Sound deathSound;      // Когда умирает
    public Sound idleSound;       // В режиме ожидания (опционально, лупится)

    [Header("Settings")]
    [SerializeField] private bool loopChaseSound = true;
    [SerializeField] private bool loopIdleSound = false;

    private Robot robot;
    private Robot.RobotState lastState;

    // ──────────────────────────────────────────────
    private void Start()
    {
        robot = GetComponent<Robot>();

        if (robot == null)
        {
            Debug.LogError(gameObject.name + ": SoundHandlerEnemy requires a Robot component!");
            return;
        }

        lastState = robot.currentState;
        CheckIfReady();

        // Стартовое состояние
        if (idleSound != null && idleSound.clip != null)
            PlayLoopSound(idleSound);
    }

    // ──────────────────────────────────────────────
    private void Update()
    {
        if (robot == null) return;

        Robot.RobotState current = robot.currentState;
        if (current == lastState) return; // Состояние не изменилось

        OnStateChanged(lastState, current);
        lastState = current;
    }

    // ──────────────────────────────────────────────
    private void OnStateChanged(Robot.RobotState from, Robot.RobotState to)
    {
        StopLoop(); // Останавливаем любой луп перед сменой состояния

        switch (to)
        {
            case Robot.RobotState.Idle:
                if (loopIdleSound && idleSound != null && idleSound.clip != null)
                    PlayLoopSound(idleSound);
                break;

            case Robot.RobotState.Activating:
                PlayOneShot(activateSound);
                break;

            case Robot.RobotState.Chasing:
            case Robot.RobotState.Returning:
                if (loopChaseSound && chaseSound != null && chaseSound.clip != null)
                    PlayLoopSound(chaseSound);
                break;

            case Robot.RobotState.Attacking:
                // Звук атаки вызывается напрямую через PlayAttackSound()
                break;

            case Robot.RobotState.Dead:
                PlayOneShot(deathSound);
                break;
        }
    }

    // ──────────────────────────────────────────────
    // Вызывай этот метод из Robot.Shoot()
    public void PlayAttackSound()
    {
        PlayOneShot(attackSound);
    }

    // Вызывай этот метод из Enemy_1.TakeDamage()
    public void PlayHitSound()
    {
        PlayOneShot(hitSound);
    }

    // ──────────────────────────────────────────────
    #region Internal Playback

    private void PlayOneShot(Sound sound)
    {
        if (sound == null || sound.clip == null) return;

        _Source.loop = false;
        _Source.volume = sound.Volume;
        _Source.pitch = Random.Range(sound.MinPitch, sound.MaxPitch);
        _Source.PlayOneShot(sound.clip);
    }

    private void PlayLoopSound(Sound sound)
    {
        if (sound == null || sound.clip == null) return;

        _Source.loop = true;
        _Source.volume = sound.Volume;
        _Source.pitch = Random.Range(sound.MinPitch, sound.MaxPitch);
        _Source.clip = sound.clip;
        _Source.Play();
    }

    private void StopLoop()
    {
        if (_Source.loop)
        {
            _Source.loop = false;
            _Source.Stop();
        }
    }

    public void StopSound() => _Source.Stop();

    #endregion

    // ──────────────────────────────────────────────
    private void CheckIfReady()
    {
        // Хотя бы один звук должен быть назначен
        bool hasAny = (activateSound != null && activateSound.clip != null) ||
                      (chaseSound != null && chaseSound.clip != null) ||
                      (attackSound != null && attackSound.clip != null) ||
                      (hitSound != null && hitSound.clip != null) ||
                      (deathSound != null && deathSound.clip != null) ||
                      (idleSound != null && idleSound.clip != null);

        if (!hasAny)
            Debug.LogWarning(gameObject.name + ": SoundHandlerEnemy — no sounds assigned!");
    }
}