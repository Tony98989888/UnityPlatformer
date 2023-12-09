using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementComponent : MonoBehaviour
{
    [Header("Jump")] public float JumpPower;
    [SerializeField, Range(0, 1)] private float m_jumpDecreaseFactor;

    [Header("Gravity")] public float Gravity;

    public float MaxFallSpeed;

    // Make character fall faster
    public float FallSpeedFactor;


    [Header("Ground Check")] public Transform GroundCheckPosition;
    public Vector2 GroundCheckSize;
    public LayerMask GroundLayer;

    [SerializeField] Rigidbody2D m_rb;


    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGravity();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (UpdateGrounded())
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
    }

    bool UpdateGrounded()
    {
        return Physics2D.OverlapBox(GroundCheckPosition.position, GroundCheckSize, 0, GroundLayer);
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(GroundCheckPosition.position, GroundCheckSize);
    }
}