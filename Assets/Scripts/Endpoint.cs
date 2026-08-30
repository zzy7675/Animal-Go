using UnityEngine;

public class Endpoint : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
            ActivateEndpoint();
    }

    private void ActivateEndpoint()
    {
        anim.SetTrigger("activate");
    }
}
