using System.Data;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;

public class MovementComponent : MonoBehaviour
{
    [Header("Move")] public float MoveSpeed;
    float m_horizontalMovement;

    [Header("Jump")] public float JumpPower;
    [SerializeField, Range(0, 1)] float m_jumpDecreaseFactor;

    [Header("Gravity")] public float Gravity;

    public float MaxFallSpeed;

    // Make character fall faster
    public float FallSpeedFactor;

    [Header("Ground Check")] public Transform GroundCheckPosition;
    public Vector2 GroundCheckSize;
    public LayerMask GroundLayer;
    bool m_isOnGround;

    [Header("Wall Detection")] public Transform WallCheckPosition;
    public Vector2 WallCheckSize;
    public LayerMask WallLayer;
    bool m_isAttachedToWall;

    [Header("Wall Movement")] private bool m_isWallSliding;
    public float WallMoveSpeed;


    [Header("Facing")] bool m_isFacingRight;

    [Header("Wall Jump")] private bool m_isWallJumping;
    float m_wallJumpDirection;
    float m_wallJumpTime;
    float m_wallJumpTimer;
    Vector2 m_wallJumpForce;
    

    [SerializeField] Rigidbody2D m_rb;


    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_isFacingRight = true;
        m_isAttachedToWall = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Environment detection
        m_isAttachedToWall = UpdateWallState();
        m_isOnGround = UpdateGrounded();
        
        // Move
        UpdateVelocity();
        UpdateGravity();
        UpdateFacing();
        UpdateWallSlide();
    }

    public void Move(InputAction.CallbackContext context)
    {
        m_horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        // Normal jump
        if (m_isOnGround)
        {
            if (context.performed)
            {
                // Button is pressed
                m_rb.velocity = new Vector2(m_rb.velocity.x, JumpPower);
            }
            else if (context.canceled)
            {
                // Button is not pressed
                m_rb.velocity = new Vector2(m_rb.velocity.x, m_rb.velocity.y * m_jumpDecreaseFactor);
            }
        }
        // Wall Jump when not on ground
        if (context.performed && m_wallJumpTimer > 0f)
        {
            m_isWallJumping = true;
            m_rb.velocity = new Vector2(m_wallJumpDirection * m_wallJumpForce.x, m_wallJumpForce.y);
        }
    }

    bool UpdateGrounded()
    {
        return Physics2D.OverlapBox(GroundCheckPosition.position, GroundCheckSize, 0, GroundLayer);
    }

    bool UpdateWallState()
    {
        return Physics2D.OverlapBox(WallCheckPosition.position, WallCheckSize, 0, WallLayer);
    }

    void UpdateVelocity()
    {
        m_rb.velocity = new Vector2(m_horizontalMovement * MoveSpeed, m_rb.velocity.y);
    }

    void UpdateGravity()
    {
        if (m_rb.velocity.y < 0)
        {
            m_rb.gravityScale = Gravity * FallSpeedFactor;
            m_rb.velocity = new Vector2(m_rb.velocity.x, Mathf.Max(m_rb.velocity.y, -MaxFallSpeed));
        }
        else
        {
            m_rb.gravityScale = Gravity;
        }
    }

    void UpdateWallSlide()
    {
        if (!m_isOnGround && m_horizontalMovement != 0 && m_isAttachedToWall)
        {
            m_isWallSliding = true;
            m_rb.velocity = new Vector2(m_rb.velocity.x, Mathf.Max(m_rb.velocity.y, -WallMoveSpeed));
        }
        else
        {
            m_isWallSliding = false;
        }
    }

    void UpdateFacing()
    {
        if (m_isFacingRight && m_horizontalMovement < 0 || !m_isFacingRight && m_horizontalMovement > 0)
        {
            m_isFacingRight = !m_isFacingRight;
            var localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    void UpdateWallJump()
    {
        if (m_isWallSliding)
        {
            m_isWallJumping = false;
            m_wallJumpDirection = -transform.localScale.x;
            m_wallJumpTimer = m_wallJumpTime;
        }else if (m_wallJumpTimer > 0f)
        {
            m_wallJumpTimer -= Time.deltaTime;
        }
    }

    void CancelWallJump()
    {
        m_isWallJumping = false;    
    }

    void OnDrawGizmosSelected()
    {
        // Ground check
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(GroundCheckPosition.position, GroundCheckSize);

        // Wall Check
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(WallCheckPosition.position, WallCheckSize);
    }
}