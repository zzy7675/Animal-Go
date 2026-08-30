using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{

    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float cooldown;
    [SerializeField] private float moveSpeed;
    private int waypointIndex = 1;
    private Animator anim;
    private SpriteRenderer sr;
    private bool active = true;
    private int moveDirection = 1;

    private Vector3[] waypointsPositions;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        GetWaypointsPostiions();
        transform.position = waypoints[0].position;
    }

    private void GetWaypointsPostiions()
    {
        waypointsPositions = new Vector3[waypoints.Length];
        for (int i = 0; i < waypoints.Length; ++i)
        {
            waypointsPositions[i] = waypoints[i].position;
        }
    }

    private void Update()
    {
        anim.SetBool("active", active);
        if (!active)
            return;

        transform.position = Vector2.MoveTowards(transform.position, waypointsPositions[waypointIndex], moveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, waypointsPositions[waypointIndex]) < 0.1f)
        {
            if (waypointIndex == waypointsPositions.Length - 1 || waypointIndex == 0)
                moveDirection = moveDirection * -1;
            waypointIndex = waypointIndex + moveDirection;
            StartCoroutine(InCooldown());
        }
    }

    private IEnumerator InCooldown()
    {
        active = false;

        yield return new WaitForSeconds(cooldown);

        active = true;
        sr.flipX = !sr.flipX;
    }
}
