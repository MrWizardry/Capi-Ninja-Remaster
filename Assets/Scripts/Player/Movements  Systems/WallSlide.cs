using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WallSlide : MonoBehaviour
{
    [Header("Wall Slide")]
    public Transform wallCheck;
    public LayerMask wallLayer;
    public float wallCheckDistance = 0.5f;
    public float wallSlideSpeed = 2f;
    public float wallStickTime = 1f;

    [Header("Wall Jump")]
    public float wallJumpForceX = 15f;
    public float wallJumpForceY = 15f;
    public float wallJumpControlTime = 0.2f; // quanto tempo bloqueia o movimento horizontal

    private Rigidbody2D rb;
    public bool isTouchingWall;
    public bool isWallSliding;
    private float wallStickCounter;
    private int wallDirection; // -1 (esquerda) ou 1 (direita)

    private float wallJumpGraceTime = 0.2f;
    private float wallJumpGraceCounter = 0f;
    private AnimationManager animManager;

    // Controle de wall jump fixo
    public bool overrideHorizontal = false;
    private float overrideTimer = 0f;
    private float overrideVelocityX = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animManager = GetComponent<AnimationManager>();

        if (wallCheck == null)
        {
            GameObject check = new GameObject("WallCheck");
            check.transform.SetParent(transform);
            check.transform.localPosition = new Vector2(0.2f, 0f);
            wallCheck = check.transform;
        }
    }

    void Update()
    {
        CheckWall();

        if (isWallSliding)
        {
            wallStickCounter -= Time.deltaTime;

            if (wallStickCounter > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }

            animManager.PlayActionAnimation("Wall_Slide");
        }

        wallJumpGraceCounter -= Time.deltaTime;

        if ((isWallSliding || wallJumpGraceCounter > 0f) && Input.GetButtonDown("Jump"))
        {
            int jumpDirection = -wallDirection;

            float input = Input.GetAxisRaw("Horizontal");
            float jumpForceX = 0f;

            if (Mathf.Sign(input) == jumpDirection && input != 0)
            {
                jumpForceX = wallJumpForceX * jumpDirection;
            }
            else
            {
                jumpForceX = 0f;
            }

            rb.velocity = new Vector2(jumpForceX, wallJumpForceY);

            overrideHorizontal = true;
            overrideTimer = wallJumpControlTime;
            overrideVelocityX = jumpForceX;

            isWallSliding = false;
            isTouchingWall = false;
            wallStickCounter = 0f;
            wallJumpGraceCounter = 0f;
        }
    }

    void FixedUpdate()
    {
        if (overrideHorizontal)
        {
            rb.velocity = new Vector2(overrideVelocityX, rb.velocity.y);
            overrideTimer -= Time.fixedDeltaTime;

            if (overrideTimer <= 0f)
            {
                overrideHorizontal = false;
            }
        }
    }

    void CheckWall()
    {
        if (IsGrounded())
        {
            isWallSliding = false;
            return;
        }

        RaycastHit2D hitRight = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, wallLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(wallCheck.position, Vector2.left, wallCheckDistance, wallLayer);

        isTouchingWall = hitRight.collider != null || hitLeft.collider != null;

        if (isTouchingWall && !IsGrounded() && rb.velocity.y <= 0)
        {
            if (!isWallSliding)
            {
                isWallSliding = true;
                wallStickCounter = wallStickTime;
            }

            wallDirection = hitRight.collider != null ? 1 : -1;
            animManager.SetDirection(wallDirection);
            wallJumpGraceCounter = wallJumpGraceTime;
        }
        else if (IsGrounded() || !isTouchingWall)
        {
            isWallSliding = false;
        }
    }

    bool IsGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, 1f, LayerMask.GetMask("groundLayer"));
    }

    void OnDrawGizmosSelected()
    {
        if (wallCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * wallCheckDistance);
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.left * wallCheckDistance);
        }
    }
}
