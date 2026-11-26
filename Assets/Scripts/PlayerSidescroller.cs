using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerSidescroller : MonoBehaviour
{
    [Header("Feel")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Movement")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpForce = 14f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Climbing")]
    [SerializeField] private float climbSpeed = 4f;
    [SerializeField] private float vineJumpCooldownTime = 0.2f; // Tiempo para soltar la liana al saltar
    private AnimationVine currentVine;
    private float vineJumpCooldown;

    private Rigidbody2D rb;
    private float horizontalInput;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private bool jumpRequested;
    private bool jumpCutRequested;
    public int FacingDirection { get; private set; } = 1;
    public bool IsJumping { get; private set; }
    private float originalGravityScale;

    public float knockbackTimer = 0f;

    // Propiedad para saber si realmente está agarrado a la liana
    private bool IsClimbing => currentVine != null && currentVine.estaLista && vineJumpCooldown <= 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        originalGravityScale = rb.gravityScale;
    }

    public void ClearBufferedInput()
    {
        horizontalInput = 0f;
        jumpBufferCounter = 0f;
        jumpRequested = false;
        jumpCutRequested = false;
    }

    private void Update()
    {
        if (vineJumpCooldown > 0f)
            vineJumpCooldown -= Time.deltaTime;

        horizontalInput = InputHelper.Horizontal();

        if (horizontalInput != 0)
            FacingDirection = horizontalInput > 0 ? 1 : -1;

        // Ahora el suelo O la liana permiten saltar (y conservan coyote time al salir del borde)
        if (IsGrounded() || IsClimbing)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;

        if (InputHelper.JumpPressed())
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            jumpRequested = true;
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }

        if (InputHelper.JumpReleased())
            jumpCutRequested = true;
    }

    private void FixedUpdate()
    {
        if (IsJumping && (IsClimbing || (rb.linearVelocityY <= 0f && IsGrounded())))
            IsJumping = false;

        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            return;
        }

        rb.linearVelocityX = horizontalInput * speed;

        if (jumpRequested)
        {
            // Si salta desde la liana, activamos el cooldown para que no la vuelva a agarrar al instante
            if (IsClimbing)
            {
                vineJumpCooldown = vineJumpCooldownTime;
            }

            rb.gravityScale = originalGravityScale;
            rb.linearVelocityY = jumpForce;
            IsJumping = true;
            jumpRequested = false;
        }

        if (jumpCutRequested)
        {
            if (rb.linearVelocityY > 0f)
                rb.linearVelocityY *= 0.5f;
            jumpCutRequested = false;
        }

        if (IsClimbing)
        {
            rb.gravityScale = 0f;
            rb.linearVelocityY = InputHelper.Vertical() * climbSpeed;
        }
        else
        {
            rb.gravityScale = originalGravityScale;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Vine"))
        {
            AnimationVine vine = other.GetComponent<AnimationVine>();
        

            if (vine != null && vine.estaLista)
                {
                    currentVine = vine;
                }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Vine") && currentVine == null)
        {
            AnimationVine vine = other.GetComponent<AnimationVine>();
            if (vine != null && vine.estaLista)
            {
                currentVine = vine;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Vine"))
        {
            AnimationVine vine = other.GetComponent<AnimationVine>();
            if (vine == currentVine)
            {
                currentVine = null;
            }
        }
    }
}
