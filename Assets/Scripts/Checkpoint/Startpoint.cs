using UnityEngine;

public class Startpoint : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
            ActivateStartPoint();
    }

    private void ActivateStartPoint()
    {
        anim.SetTrigger("activate");
    }
}
