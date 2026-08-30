using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private bool facingRight = true;
    private int facingDir = 1;
    private float xInput;
    private float yInput;
    private bool canDoubleJump = true;

    [Header("Buffer & Coyote Jump")]
    [SerializeField] private float bufferJumpWindow;
    [SerializeField] private float coyoteJumpWindow;
    private float bufferJumpActivated = -1f;
    private float coyoteJumpActivated = -1f;

    [Header("Wall Interactions")]
    [SerializeField] private Vector2 wallJumpForce;
    [SerializeField] private float wallJumpDuration;
    private bool isWallJumping;
    private bool isWall;

    [Header("Hit")]
    [SerializeField] private float hitDuration;
    [SerializeField] private Vector2 hitPower;
    private bool isHit;

    [Header("Collision Info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;
    private bool isInAir;

    [Header("Death")]
    [SerializeField] private GameObject vfxDeath;
    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D cd;
    private float initialGravityScale;
    private bool canBeControlled;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    void Start()
    {
        initialGravityScale = rb.gravityScale;
        InRespawn(true);
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerStatus();
        if (!canBeControlled)
            return;

        if (isHit)
            return;

        HandleInput();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollisions();
        HandleAnimations();
    }

    private void UpdatePlayerStatus()
    {
        if (isGrounded && isInAir)
            Landing();

        if (!isGrounded && !isInAir)
            IntoAir();
    }

    private void IntoAir()
    {
        isInAir = true;
        if (rb.velocityY <= 0)
            ActivateCoyoteJump();
    }

    private void Landing()
    {
        isInAir = false;
        canDoubleJump = true;
        AttemptBufferJump();
    }

    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleJump();
            RequestBufferJump();
        }
    }

    private void HandleJump()
    {
        bool coyoteJumpAvailable = (Time.time < coyoteJumpActivated + coyoteJumpWindow);
        if (isGrounded || coyoteJumpAvailable)
        {
            Jump();
        }
        else if (isWall && !isGrounded)
        {
            WallJump();
        }
        else if (canDoubleJump)
        {
            DoubleJump();
        }
        CancelCoyoteJump();
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocityX, jumpForce);
    }

    private void DoubleJump()
    {
        canDoubleJump = false;
        isWallJumping = false;
        rb.velocity = new Vector2(rb.velocityX, doubleJumpForce);
    }

    private void WallJump()
    {
        rb.velocity = new Vector2(wallJumpForce.x * -facingDir, wallJumpForce.y);

        Flip();
        StopAllCoroutines();
        StartCoroutine(WallJumpRoutine());
    }

    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;

        yield return new WaitForSeconds(wallJumpDuration);

        isWallJumping = false;
    }

    private void HandleWallSlide()
    {
        bool canWallSlide = (isWall && rb.velocityY < 0);
        float wallSlideSpeedModifier = (yInput < 0) ? 1f : 0.05f;

        if (!canWallSlide)
            return;

        rb.velocity = new Vector2(rb.velocityX, rb.velocityY * wallSlideSpeedModifier);
    }

    private void HandleMovement()
    {
        if (isWall)
            return;

        if (isWallJumping)
            return;

        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocityY);
    }

    private void HandleFlip()
    {
        if ((xInput < 0 && facingRight) || (xInput > 0 && !facingRight))
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingDir = facingDir * -1;
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    private void HandleCollisions()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWall = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity", rb.velocityX);
        anim.SetFloat("yVelocity", rb.velocityY);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWall", isWall);
    }

    public void GetHit(float sourceDamagePositionX)
    {
        int hitDir = 1;
        if (transform.position.x < sourceDamagePositionX)
            hitDir = -1;

        if (isHit)
            return;
        StartCoroutine(GetHitRoutine());
        rb.velocity = new Vector2(hitPower.x * hitDir, hitPower.y);
    }

    private IEnumerator GetHitRoutine()
    {
        isHit = true;
        anim.SetBool("getHit", true);
        yield return new WaitForSeconds(hitDuration);

        isHit = false;
        anim.SetBool("getHit", false);
    }

    public void Die()
    {
        Instantiate(vfxDeath, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void InRespawn(bool inRespawn)
    {
        if (inRespawn)
        {
            rb.gravityScale = 0;
            canBeControlled = false;
            cd.enabled = false;
        }
        else
        {
            rb.gravityScale = initialGravityScale;
            canBeControlled = true;
            cd.enabled = true;
        }
    }

    private void RequestBufferJump()
    {
        if (isInAir)
            bufferJumpActivated = Time.time;
    }

    private void AttemptBufferJump()
    {
        if (Time.time < bufferJumpActivated + bufferJumpWindow)
        {
            bufferJumpActivated = Time.time - 1;
            Jump();
        }
    }

    private void ActivateCoyoteJump()
    {
        coyoteJumpActivated = Time.time;
    }

    private void CancelCoyoteJump()
    {
        coyoteJumpActivated = Time.time - 1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + wallCheckDistance * facingDir, transform.position.y));
    }
}
