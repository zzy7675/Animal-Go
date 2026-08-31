using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSpikeBall : MonoBehaviour
{
    [SerializeField] private float pushForce;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        rb.AddForce(transform.right * pushForce, ForceMode2D.Impulse);
    }
}
