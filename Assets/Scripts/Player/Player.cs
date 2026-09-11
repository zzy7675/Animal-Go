using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject fruitDrop;
    [SerializeField] private DifficultyType gameDifficulty;
    private GameManager gameManager;

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
    [Space]
    [SerializeField] private float enemyCheckRadius;
    [SerializeField] private Transform enemyCheck;
    [SerializeField] private LayerMask whatIsEnemy;
    private bool isGrounded;
    private bool isInAir;

    [Header("Player Visuals")]
    [SerializeField] private AnimatorOverrideController[] animators;
    [SerializeField] private GameObject vfxDeath;
    [SerializeField] private int skinIndex;
    [SerializeField] ParticleSystem vfxDust;
    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D cd;
    private float initialGravityScale;
    private bool canBeControlled;

    private Joystick joystick;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        cd = GetComponent<CapsuleCollider2D>();

        FindFirstObjectByType<UI_JumpButton>().UpdatePlayersRef(this);
        joystick = FindFirstObjectByType<Joystick>();
    }

    void Start()
    {
        initialGravityScale = rb.gravityScale;

        gameManager = GameManager.instance;
        UpdateGameDifficulty();
        InRespawn(true);
        UpdateSkin();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerStatus();
        if (!canBeControlled)
        {
            HandleCollisions();
            HandleAnimations();
            return;
        }

        if (isHit)
            return;
        HandleEnemyDetection();
        HandleInput();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollisions();
        HandleAnimations();
    }

    public void Damage()
    {
        if (gameDifficulty == DifficultyType.Normal)
        {

            if (gameManager.FruitsCollected() <= 0)
            {
                Die();
                gameManager.RestartLevel();
            } else
            {
                ObjectCreator.instance.CreateObject(fruitDrop, transform, 0, true);
                gameManager.RemoveFruit();
            }

            return;
        }

        if (gameDifficulty == DifficultyType.Hard)
        {
            Die();
            // restart level
            gameManager.RestartLevel();
        }
    }

    private void UpdateGameDifficulty()
    {
        DifficultyManager difficultyManager = DifficultyManager.instance;
        if (difficultyManager != null)
            gameDifficulty = difficultyManager.difficulty;
    }

    public void UpdateSkin()
    {
        SkinManager skinManager = SkinManager.instance;

        if (skinManager == null)
            return;
        anim.runtimeAnimatorController = animators[skinManager.chosenSkinIndex];
    }

    private void HandleEnemyDetection()
    {
        if (rb.linearVelocityY >= 0)
            return;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemyCheck.position, enemyCheckRadius, whatIsEnemy);

        foreach (var enemy in colliders)
        {
            Enemy newEnemy = enemy.GetComponent<Enemy>();
            if (newEnemy != null)
            {
                AudioManager.instance.PlaySFX(((int)SFXType.SFX_EnemyKicked));
                newEnemy.Die();
                Jump();
            }
        }
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
        if (rb.linearVelocityY <= 0)
            ActivateCoyoteJump();
    }

    private void Landing()
    {
        vfxDust.Play();
        isInAir = false;
        canDoubleJump = true;
        AttemptBufferJump();
    }

    private void HandleInput()
    {
        //xInput = Input.GetAxisRaw("Horizontal");
        //yInput = Input.GetAxisRaw("Vertical");
        xInput = joystick.Horizontal;
        yInput = joystick.Vertical;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();
        }
    }
    public void JumpButton()
    {
        HandleJump();
        RequestBufferJump();
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
        vfxDust.Play();
        AudioManager.instance.PlaySFX(((int)SFXType.SFX_Jump));
        rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
    }

    private void DoubleJump()
    {
        vfxDust.Play();
        AudioManager.instance.PlaySFX(((int)SFXType.SFX_Jump));
        canDoubleJump = false;
        isWallJumping = false;
        rb.linearVelocity = new Vector2(rb.linearVelocityX, doubleJumpForce);
    }

    private void WallJump()
    {
        vfxDust.Play();
        AudioManager.instance.PlaySFX(((int)SFXType.SFX_Walljump));

        canDoubleJump = true;
        rb.linearVelocity = new Vector2(wallJumpForce.x * -facingDir, wallJumpForce.y);

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
        bool canWallSlide = (isWall && rb.linearVelocityY < 0);
        float wallSlideSpeedModifier = (yInput < 0) ? 1f : 0.05f;
        if (!canWallSlide)
            return;

        rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * wallSlideSpeedModifier);
    }

    private void HandleMovement()
    {
        if (isWall)
            return;

        if (isWallJumping)
            return;

        rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocityY);
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
        anim.SetFloat("xVelocity", rb.linearVelocityX);
        anim.SetFloat("yVelocity", rb.linearVelocityY);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWall", isWall);
    }

    public void GetHit(float sourceDamagePositionX)
    {
        float hitDir = 1;
        if (transform.position.x < sourceDamagePositionX)
            hitDir = -1;

        if (isHit)
            return;
        AudioManager.instance.PlaySFX(((int)SFXType.SFX_PlayerKnocked));
        CameraManager.instance.ScreenShake(hitDir);
        StartCoroutine(GetHitRoutine());
        rb.linearVelocity = new Vector2(hitPower.x * hitDir, hitPower.y);
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
        AudioManager.instance.PlaySFX(((int)SFXType.SFX_Death));
        Instantiate(vfxDeath, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void Push(Vector2 direction, float duration)
    {
        StartCoroutine(PushRoutine(direction, duration));
    }

    private IEnumerator PushRoutine(Vector2 direction, float duration)
    {
        canBeControlled = false;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction, ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);

        canBeControlled = true;
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

            AudioManager.instance.PlaySFX(((int)SFXType.SFX_Respawn2));
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
        Gizmos.DrawWireSphere(enemyCheck.position, enemyCheckRadius);
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + wallCheckDistance * facingDir, transform.position.y));
    }
}
