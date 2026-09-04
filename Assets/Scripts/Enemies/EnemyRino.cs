using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRino : Enemy
{
    [Header("Rino Details")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedUpRate;
    private float defaultSpeed;
    [SerializeField] private Vector2 impactPower;


    protected override void Start()
    {
        base.Start();
        defaultSpeed = moveSpeed;
    }

    protected override void Update()
    {
        base.Update();

        HandleCharge();
    }


    protected override void HandleCollisions()
    {
        base.HandleCollisions();

        if (playerDetected && IsOnTheGround)
        {
            canMove = true;
        }
    }

    private void HandleCharge()
    {
        if (!canMove)
            return;

        HandleSpeedup();

        rb.velocity = new Vector2(facingDir * moveSpeed, rb.velocityY);

        if (!frontIsGround)
        {
            TurnAround();
        }

        if (frontIsWall)
        {
            WallHit();
        }
    }

    private void HandleSpeedup()
    {
        moveSpeed = moveSpeed + speedUpRate * Time.deltaTime;

        if (moveSpeed >= maxSpeed)
            maxSpeed = moveSpeed;
    }

    private void TurnAround()
    {
        SpeedReset();
        canMove = false;
        rb.velocity = Vector2.zero;
        Flip();

    }

    private void WallHit()
    {
        canMove = false;
        SpeedReset();
        anim.SetBool("hitWall", true);
        rb.velocity = new Vector2(impactPower.x * -facingDir,
            impactPower.y);
    }

    private void SpeedReset()
    {
        moveSpeed = defaultSpeed;
    }

    private void ChargeIsOver()
    {
        anim.SetBool("hitWall", false);
        Invoke(nameof(Flip), 1);
    }
}
