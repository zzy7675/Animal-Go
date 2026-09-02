using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChicken : Enemy
{
    private BoxCollider2D cd;
    [Header("Chicken Details")]
    [SerializeField] private float aggroDuration;
    [SerializeField] private float detectionRange;
    private float aggroTimer;
    private bool playerDetected;
    private bool canFlip = true;

    protected override void Awake()
    {
        base.Awake();
        cd = GetComponent<BoxCollider2D>();
    }
    protected override void Update()
    {
        base.Update();
        anim.SetFloat("xVelocity", rb.velocityX);
        aggroTimer -= Time.deltaTime;
        if (isDead)
            return;
        if (playerDetected)
        {
            canMove = true;
            aggroTimer = aggroDuration;
        }

        if (aggroTimer < 0)
            canMove = false;
        
        HandleCollisions();
        HandleMovement();

        if (IsOnTheGround)
            HandleTurnAround();
    }

    private void HandleTurnAround()
    {
        if (!frontIsGround || frontIsWall)
        {
            Flip();
            canMove = false;
            rb.velocity = Vector2.zero;
        }
    } 

    private void HandleMovement()
    {
        if (!canMove)
            return;

        HandleFlip(player.position.x);


        rb.velocity = new Vector2(facingDir * moveSpeed, rb.velocityY);
    }

    protected override void HandleFlip(float xValue)
    {
        if ((xValue < transform.position.x && facingRight) || (xValue > transform.position.x && !facingRight))
        {
            if (canFlip)
            {
                canFlip = false;
                Invoke(nameof(Flip), .3f);
            }
        }

    }

    protected override void Flip()
    {
        base.Flip();
        canFlip = true;
    }

    protected override void HandleCollisions()
    {
        base.HandleCollisions();
        playerDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, detectionRange, whatIsPlayer);
    }

    // protected override void OnDrawGizmos() {
    //     base.OnDrawGizmos();
    //     Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (detectionRange * facingDir), transform.position.y));
    // }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (detectionRange * facingDir), transform.position.y));
    }
    public override void Die()
    {
        base.Die();
        cd.enabled = false;
    }
}
