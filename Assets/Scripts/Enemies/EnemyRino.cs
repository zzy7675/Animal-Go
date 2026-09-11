using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class EnemyRino : Enemy
{
    [Header("Rino Details")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedUpRate;
    private float defaultSpeed;
    [SerializeField] private Vector2 impactPower;

    [Header("Effects")]
    [SerializeField] private ParticleSystem vfxDust;
    [SerializeField] private Vector2 cameraImpulseVelocity;
    private CinemachineImpulseSource impulseSource;

    protected override void Start()
    {
        base.Start();
        defaultSpeed = moveSpeed;
        canMove = false;
        impulseSource = GetComponent<CinemachineImpulseSource>();
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

    private void HitWallImpact()
    {
        vfxDust.Play();
        impulseSource.DefaultVelocity = new Vector2(cameraImpulseVelocity.x * facingDir, cameraImpulseVelocity.y);
        impulseSource.GenerateImpulse();
    }

    private void HandleCharge()
    {
        if (!canMove)
            return;

        HandleSpeedup();

        rb.linearVelocity = new Vector2(facingDir * moveSpeed, rb.linearVelocityY);

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
        rb.linearVelocity = Vector2.zero;
        Flip();

    }

    private void WallHit()
    {
        canMove = false;

        HitWallImpact();
        SpeedReset();
        anim.SetBool("hitWall", true);
        rb.linearVelocity = new Vector2(impactPower.x * -facingDir,
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
