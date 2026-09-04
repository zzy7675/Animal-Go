using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMushroom : Enemy
{

    protected override void Update()
    {
        base.Update();

        if (isDead)
            return;
        
        HandleMovement();

        if (IsOnTheGround)
            HandleTurnAround();
    }

    private void HandleTurnAround()
    {
        if (!frontIsGround || frontIsWall)
        {
            Flip();
            idleTimer = idleDuration;
            rb.velocity = Vector2.zero;
        }
    } 

    private void HandleMovement()
    {
        if (idleTimer > 0)
            return;


        rb.velocity = new Vector2(facingDir * moveSpeed, rb.velocityY);
    }

}
