using UnityEngine;

/// <summary>
/// Повесь на триггер-зону за стеной/укрытием.
/// Когда игрок (Player_1) входит — босс переключается на ракеты и призыв.
/// Когда выходит — возвращается к обычным атакам.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class BossCoverZone : MonoBehaviour
{
    [Tooltip("Ссылка на босса")]
    public BossAI boss;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player_1")) return;
        boss?.OnPlayerEnterCover();
        Debug.Log("[CoverZone] Игрок в укрытии — босс: только ракеты и роботы");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player_1")) return;
        boss?.OnPlayerExitCover();
        Debug.Log("[CoverZone] Игрок вышел из укрытия — босс: все атаки");
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
        Collider2D col = GetComponent<Collider2D>();
        if (col is BoxCollider2D box)
        {
            Gizmos.DrawCube(transform.position + (Vector3)box.offset,
                            new Vector3(box.size.x, box.size.y, 0.1f));
        }
    }
#endif
}
