using System.Collections;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    public TrailRenderer[] dashTrails;
    [SerializeField] private float dashForce = 5f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashInvincibleTime = 0.3f;
    public SoundHandlerGlobal dashSFXHandler;

    private PlayerPhysics _PlayerPhysics;
    private PlayerMaterials _PlayerMaterials;
    private AfterImageHandler _AfterImageHandler;
    private PlayerHealth _PlayerHealth;
    private bool canDash;
    private const float DASH_DURATION = 0.2f;

    private void Awake()
    {
        _AfterImageHandler = FindObjectOfType<AfterImageHandler>();
        _PlayerHealth = FindObjectOfType<PlayerHealth>();
        TryGetComponent(out _PlayerPhysics);
        TryGetComponent(out _PlayerMaterials);
        canDash = true;
        SetActiveTrails(false);
    }

    public void Dash()
    {
        if (canDash)
            StartCoroutine(CO_Dash());
    }

    private IEnumerator CO_Dash()
    {
        canDash = false;
        SetActiveTrails(true);
        _AfterImageHandler.SetActiveAfterImages();
        _PlayerPhysics.CanMove = false;

        // ── Неуязвимость на время дэша ────────────────────────────────────
        if (_PlayerHealth != null)
            _PlayerHealth.SetInvincible(dashInvincibleTime);

        ActualDash();

        yield return new WaitForSeconds(DASH_DURATION);
        SetActiveTrails(false);
        _PlayerPhysics.CanMove = true;

        // ── Ждём КД ───────────────────────────────────────────────────────
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void ActualDash()
    {
        if (dashSFXHandler != null)
            dashSFXHandler.PlaySound();
        _PlayerMaterials.SetActiveHighlightBody(DASH_DURATION, intensity: 1.25f);
        _PlayerPhysics.SetVelocity(Vector2.zero);
        _PlayerPhysics.AddForce(TadaInput.MoveAxisRawInput.normalized, dashForce, ForceMode2D.Impulse);
    }

    private void SetActiveTrails(bool value)
    {
        for (int i = 0; i < dashTrails.Length; i++)
        {
            if (dashTrails != null)
                dashTrails[i].emitting = value;
        }
    }
}