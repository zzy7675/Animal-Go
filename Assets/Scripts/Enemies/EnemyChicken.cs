using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChicken : Enemy
{
    [Header("Chicken Details")]
    [SerializeField] private float aggroDuration;
    [SerializeField] private float detectionRange;
    private float aggroTimer;
    private bool canFlip = true;

    protected override void Update()
    {
        base.Update();

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

}
