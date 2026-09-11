using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator anim;
    private bool isActive;
    private bool canBeReactivated;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        canBeReactivated = GameManager.instance.canReactivate;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive && !canBeReactivated)
            return;

        Player player = collision.GetComponent<Player>();
        if (player != null)
            ActivateCheckpoint();

    }

    private void ActivateCheckpoint()
    {
        isActive = true;
        anim.SetTrigger("activate");
        PlayerManager.instance.UpdateRespawnPoint(transform);
    }
}
