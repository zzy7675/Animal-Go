using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTrampoline : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private float pushPower;
    [SerializeField] private float duration;

    private void Awake() {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            anim.SetTrigger("activate");
            player.Push(transform.up * pushPower, duration);
        }

    }
}
