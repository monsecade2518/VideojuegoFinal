using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerSidescroller : MonoBehaviour
{
    private enum LocomotionState
    {
        IdleRight = 0,
        IdleLeft = 1,
        WalkRight = 2,
        WalkLeft = 3
    }

    private static readonly int LocomotionStateParameter = Animator.StringToHash("LocomotionState");

    [Header("Feel")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Movement")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField, Min(0f)] private float walkHoldTime = 0.12f;

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
    private Animator playerAnimator;
    private float horizontalInput;
    private int heldDirection;
    private float directionHeldSince;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private bool jumpRequested;
    private bool jumpCutRequested;
    public int FacingDirection { get; private set; } = 1;
    private float originalGravityScale;

    public float knockbackTimer = 0f;

    // Propiedad para saber si realmente está agarrado a la liana
    private bool IsClimbing => currentVine != null && currentVine.estaLista && vineJumpCooldown <= 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponentInChildren<Animator>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        originalGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        if (vineJumpCooldown > 0f)
            vineJumpCooldown -= Time.deltaTime;

        UpdateHorizontalInput();

        bool isGrounded = IsGrounded();
        bool isClimbing = IsClimbing;
        UpdateLocomotionAnimation(isGrounded && !isClimbing && knockbackTimer <= 0f && horizontalInput != 0f);

        // Ahora el suelo O la liana permiten saltar (y conservan coyote time al salir del borde)
        if (isGrounded || isClimbing)
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

    private void UpdateHorizontalInput()
    {
        float input = InputHelper.Horizontal();
        int direction = input == 0f ? 0 : (input > 0f ? 1 : -1);

        if (direction != heldDirection)
        {
            heldDirection = direction;
            directionHeldSince = Time.time;
        }

        if (direction != 0)
            FacingDirection = direction;

        horizontalInput = Time.time - directionHeldSince >= walkHoldTime ? input : 0f;
    }

    private void UpdateLocomotionAnimation(bool isWalking)
    {
        if (playerAnimator == null)
            return;

        LocomotionState state;
        if (FacingDirection < 0)
            state = isWalking ? LocomotionState.WalkLeft : LocomotionState.IdleLeft;
        else
            state = isWalking ? LocomotionState.WalkRight : LocomotionState.IdleRight;

        playerAnimator.SetInteger(LocomotionStateParameter, (int)state);
    }

    private void FixedUpdate()
    {
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
