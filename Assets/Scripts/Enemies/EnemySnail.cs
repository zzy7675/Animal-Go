using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySnail : Enemy
{
    [Header("Snail Details")]
    [SerializeField] private EnemySnailBody snailBodyPrefab;
    [SerializeField] private float maxSpeed = 10f;
    public bool hasBody = true;
    protected override void Update()
    {
        base.Update();

        if (isDead)
            return;

        HandleMovement();

        if (IsOnTheGround)
            HandleTurnAround();
    }

    public override void Die()
    {
        if (hasBody)
        {
            canMove = false;
            hasBody = false;
            anim.SetTrigger("hit");
            rb.linearVelocity = Vector2.zero;
            idleDuration = 0;
        } else if (!canMove && !hasBody)
        {
            anim.SetTrigger("hit");
            canMove = true;
            moveSpeed = maxSpeed;
        }
        else
        {
            base.Die();
        }

    }

    protected override void Flip()
    {
        base.Flip();

        if (!hasBody)
            anim.SetTrigger("wallHit");
    }

    private void HandleTurnAround()
    {
        bool canFlipFromLedge = (!frontIsGround && hasBody);
        if (canFlipFromLedge || frontIsWall)
        {
            Flip();
            idleTimer = idleDuration;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void HandleMovement()
    {
        Debug.Log(rb.linearVelocity);
        if (idleTimer > 0)
            return;

        if (!canMove)
            return;

        rb.linearVelocity = new Vector2(facingDir * moveSpeed, rb.linearVelocityY);
    }

    private void CreateBody()
    {
        EnemySnailBody newBody = Instantiate(snailBodyPrefab, transform.position, Quaternion.identity);

        if (Random.Range(0, 100) < 50)
            deathRotationDirection = deathRotationDirection * -1;

        newBody.SetupBody(deathImpact, deathRotationSpeed * deathRotationDirection, facingDir);

        Destroy(newBody.gameObject, 10);
    }
}
