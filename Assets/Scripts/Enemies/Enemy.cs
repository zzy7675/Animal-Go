using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private SpriteRenderer sr => GetComponent<SpriteRenderer>();
    protected Transform player;
    protected Animator anim;
    protected Rigidbody2D rb;
    protected Collider2D[] colliders;

    [Header("General")]
    [SerializeField] protected float moveSpeed = 2;
    [SerializeField] protected float idleDuration = 1.5f;
    public bool canMove = true;
    protected float idleTimer;

    [Header("Death")]
    [SerializeField] protected float deathImpact = 5;
    [SerializeField] protected float deathRotationSpeed = 150;
    protected int deathRotationDirection = 1;
    protected bool isDead;

    [Header("Collision")]
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected Transform groundCheckPoint;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected float playerDetectionDistance = 15;
    [SerializeField] protected LayerMask whatIsPlayer;
    protected bool playerDetected;
    protected bool facingRight = false;
    protected int facingDir = -1;

    protected bool IsOnTheGround;
    protected bool frontIsGround;
    protected bool frontIsWall;
    protected virtual void Awake() {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponentsInChildren<Collider2D>();
    }

    protected virtual void Start()
    {
        InvokeRepeating(nameof(UpdatePlayerRef), 0, 1);

        if (sr.flipX && !facingRight)
        {
            sr.flipX = false;
            Flip();
        }
    }

    private void UpdatePlayerRef()
    {
        if (player == null)
            player = GameManager.instance.player.transform;
    }

    protected virtual void Update()
    {
        HandleCollisions();
        HandleAnimator();
        idleTimer -= Time.deltaTime;


        if (isDead)
            HandleDeathRotation();
    }
    protected virtual void HandleFlip(float xValue)
    {
        if ((xValue < transform.position.x && facingRight) || (xValue > transform.position.x && !facingRight))
        {
            Flip();
        }
    }

    public virtual void Die()
    {
        // damageTrigger.SetActive(false);
        // col.enabled = false;
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
        anim.SetTrigger("hit");
        rb.velocity = new Vector2(rb.velocityX, deathImpact);
        isDead = true;
        if (Random.Range(0, 100) < 50)
            deathRotationDirection = deathRotationDirection * -1;
        Destroy(gameObject, 10);
    }

    [ContextMenu("Change Facing Direction")]
    public void FlipDefaultFacingDirection()
    {
        sr.flipX = !sr.flipX;
    }

    private void HandleDeathRotation()
    {
        transform.Rotate(0, 0, (deathRotationSpeed * deathRotationDirection) * Time.deltaTime);
    }
    
    protected virtual void Flip()
    {
        facingDir = facingDir * -1;
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    protected virtual void HandleCollisions()
    {
        frontIsGround = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistance, whatIsGround);
        frontIsWall = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        IsOnTheGround = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        playerDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, playerDetectionDistance, whatIsPlayer);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheckPoint.position, new Vector2(groundCheckPoint.position.x, groundCheckPoint.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + wallCheckDistance * facingDir, transform.position.y));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (playerDetectionDistance * facingDir), transform.position.y));
    }

    protected virtual void HandleAnimator()
    {
        anim.SetFloat("xVelocity", rb.velocityX);
    }
}
