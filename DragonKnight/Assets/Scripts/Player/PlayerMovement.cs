using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private InputActionReference moveAction;

    [Header("Jump")]
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rayLength = 0.6f;

    [Header("Jump Gravity")]
    [SerializeField] private float fallGravityMultiplier = 2.5f;
    [SerializeField] private float lowJumpGravityMultiplier = 2f;

    [Header("Jump Assist")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Dash")]
    [SerializeField] private InputActionReference dashAction;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.8f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool isGrounded;
    private float defaultGravityScale;
    private bool isDashing;
    private float dashTimeLeft;
    private float dashCooldownTimer;
    private float dashDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        defaultGravityScale = rb.gravityScale;
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        dashAction.action.Enable();
        jumpAction.action.started += OnJumpPressed;
        jumpAction.action.canceled += OnJumpReleased;
        dashAction.action.started += OnDashPressed;
    }

    private void OnDisable()
    {
        jumpAction.action.started -= OnJumpPressed;
        jumpAction.action.canceled -= OnJumpReleased;
        dashAction.action.started -= OnDashPressed;
        moveAction.action.Disable();
        jumpAction.action.Disable();
        dashAction.action.Disable();
    }

    private void Update()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();

        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer);

        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        jumpBufferCounter -= Time.deltaTime;
        dashCooldownTimer -= Time.deltaTime;

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            Jump();
            jumpBufferCounter = 0f;
        }

        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0f)
                isDashing = false;
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            // 대시 중에는 수평으로만 빠르게 이동, 중력 무시
            rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);
            rb.gravityScale = 0f;
            return;
        }

        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        if (rb.linearVelocity.y < 0f)
        {
            // 하강 중: 중력을 높여서 빠르게 떨어짐
            rb.gravityScale = defaultGravityScale * fallGravityMultiplier;
        }
        else if (rb.linearVelocity.y > 0f && !jumpAction.action.IsPressed())
        {
            // 상승 중 + 점프 버튼 뗀 상태: 짧은 점프 처리
            rb.gravityScale = defaultGravityScale * lowJumpGravityMultiplier;
        }
        else
        {
            rb.gravityScale = defaultGravityScale;
        }

        if (moveInput.x != 0f)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput.x), 1f, 1f);
        }
    }

    private void OnJumpPressed(InputAction.CallbackContext context)
    {
        jumpBufferCounter = jumpBufferTime;
    }

    private void OnJumpReleased(InputAction.CallbackContext context)
    {
        if (rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }
        coyoteTimeCounter = 0f;
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        coyoteTimeCounter = 0f;
    }

    private void OnDashPressed(InputAction.CallbackContext context)
    {
        if (isDashing || dashCooldownTimer > 0f)
            return;

        isDashing = true;
        dashTimeLeft = dashDuration;
        dashCooldownTimer = dashCooldown;

        // 이동 입력 방향으로 대시, 입력 없으면 바라보는 방향으로
        dashDirection = moveInput.x != 0f ? Mathf.Sign(moveInput.x) : transform.localScale.x;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayLength);
    }
}
