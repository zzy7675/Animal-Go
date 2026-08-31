using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TrapFallingPlatform : MonoBehaviour
{
    private Vector3[] waypoints;
    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D[] colliders;

    [Header("Movement")]
    [SerializeField] private float totalDistance;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float deactivateDelay;
    private int waypointIndex = 0;
    private bool active;

    [Header("Impact")]
    [SerializeField] private float impactSpeed;
    [SerializeField] private float impactDuration;
    private float impactTimer;
    private bool impactHappend = false;


    
    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<Collider2D>();
    }
    private void Start()
    {
        SetupWaypoints();
        ActivatePlatform();
    }

    private void Update()
    {
        HandleImpact();
        HandleMovement();
    }

    private void HandleImpact()
    {
        if (impactTimer < 0)
            return;
        impactTimer -= Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3.down * 20), impactSpeed * Time.deltaTime);
    }

    private void SetupWaypoints()
    {
        waypoints = new Vector3[2];
        float yOffset = totalDistance / 2;
        waypoints[0] = new Vector3(transform.position.x, transform.position.y + yOffset);
        waypoints[1] = new Vector3(transform.position.x, transform.position.y - yOffset);
    }

    private void ActivatePlatform()
    {
        StartCoroutine(ActivatePlatformRoutine(Random.Range(0, 0.6f)));
    }
    private IEnumerator ActivatePlatformRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        active = true;
    }

    private void HandleMovement()
    {
        if (!active)
            return;

        transform.position = Vector2.MoveTowards(transform.position, waypoints[waypointIndex], moveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, waypoints[waypointIndex]) < 0.1f)
        {
            waypointIndex++;
            if (waypointIndex == waypoints.Length)
                waypointIndex = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (impactHappend)
            return;

        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            DeactivatePlatform();
            // Invoke(nameof(DeactivatePlatform), deactivateDelay);
            impactTimer = impactDuration;
            impactHappend = true;
        }
            
    }

    private void DeactivatePlatform()
    {
        StartCoroutine(DeactivatePlatformRoutine());
    }

    private IEnumerator DeactivatePlatformRoutine()
    {
        yield return new WaitForSeconds(deactivateDelay);
        active = false;
        anim.SetTrigger("deactivate");  
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3.5f;
        rb.drag = .5f;
        foreach(Collider2D cd in colliders)
        {
            cd.enabled = false;
        }
    }

    // private void DeactivatePlatform()
    // {

    //     anim.SetTrigger("deactivate");
    //     active = false;
    //     rb.isKinematic = false;
    //     rb.gravityScale = 3.5f;
    //     rb.drag = .5f;
    //     foreach(Collider2D cd in colliders)
    //     {
    //         cd.enabled = false;
    //     }
    // }
}
