using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    // --- MOVIMENTO ---
    [Header("Andar")]
    private float inputDirection = 0f;
    public float moveDirection = 0f;
    [SerializeField] private float walk = 6f; // Velocidade do player
    [SerializeField] private float running = 8f;
    [SerializeField] private float MaxSpeed = 20f;
    [SerializeField] private float accelerationTime = 2f;
    [SerializeField] private float decelerationTime = 1f; // Tempo pra acelerar e desacelerar
    public float currentSpeed;
    private float accelerationTimer = 0f;
    public Rigidbody2D rb; // Rigidbody do player

    [Header("Drift")]
    [SerializeField] private float driftSpeed = 5f; // Quanto mais baixo, mais demora pra trocar direção

    // --- LAYERS ---
    [Header("Aplicações")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    // --- JUMP SYSTEM ---
    [Header("Jump")]
    [SerializeField] private float jumpingPower = 16f;
    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteTimecount;
    private bool isJumping;
    [SerializeField] private float jumpbuffer = 0.2f;
    private float jumpbuffercount;
    [SerializeField] private float jumpCoolDown = 0.2f;
    private float jumpCooldownTimer = 0f;

    public AnimationManager animManager;
    private WallSlide wallSlide;
    private GrapplingHook grapplingHook;

    // --- START ---
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animManager = GetComponent<AnimationManager>();
        isJumping = false;
        wallSlide = GetComponent<WallSlide>();
        grapplingHook = GetComponent<GrapplingHook>();
        currentSpeed = 0f;
    }

    void Update()
    {
        #region Movement
        if (Custom_Input.GetKey("Right"))
            inputDirection = 1f;
        else if (Custom_Input.GetKey("Left"))
            inputDirection = -1f;
        else
            inputDirection = 0f;

        if(IsGrounded())
        {
            if (inputDirection != 0)
            {
                accelerationTimer += Time.deltaTime;
                currentSpeed = Mathf.Lerp(currentSpeed, running, accelerationTimer / accelerationTime);
                currentSpeed = Mathf.Clamp(currentSpeed, 0f, MaxSpeed);

                // Drift na troca de direção
                moveDirection = Mathf.MoveTowards(moveDirection, inputDirection, driftSpeed * Time.deltaTime);
            }
            else
            {
                accelerationTimer = 0f;

                if (currentSpeed > 0f)
                {
                    currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime);
                    if (currentSpeed < 1f)
                        currentSpeed = 0f;
                }

                // Drift de desaceleração (escorrega ao parar)
                moveDirection = Mathf.MoveTowards(moveDirection, 0f, driftSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (inputDirection != 0)
            {
                accelerationTimer += Time.deltaTime;
                currentSpeed = Mathf.Lerp(currentSpeed, running, accelerationTimer / accelerationTime);
                currentSpeed = Mathf.Clamp(currentSpeed, 0f, MaxSpeed);

                // Drift na troca de direção
                moveDirection = Mathf.MoveTowards(moveDirection, inputDirection, (driftSpeed * 0.25f) * Time.deltaTime);
            }
            else
            {
                accelerationTimer = 0f;

                if (currentSpeed > 0f)
                {
                    currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime);
                    if (currentSpeed < 1f)
                        currentSpeed = 0f;
                }

                // Drift de desaceleração (escorrega ao parar)
                moveDirection = Mathf.MoveTowards(moveDirection, 0f, (driftSpeed * 0.25f) * Time.deltaTime);
            }
        }
        #endregion

        #region Jump Functions
        if (IsGrounded())
        {
            coyoteTimecount = coyoteTime;
            isJumping = false;
        }
        else
            coyoteTimecount -= Time.deltaTime;

        if (jumpCooldownTimer > 0f)
            jumpCooldownTimer -= Time.deltaTime;

        if (Input.GetButtonDown("Jump"))
            jumpbuffercount = jumpbuffer;
        else
            jumpbuffercount -= Time.deltaTime;

        if (jumpbuffercount > 0f && coyoteTimecount > 0f && jumpCooldownTimer <= 0f)
        {
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            jumpbuffercount = 0f;
            jumpCooldownTimer = jumpCoolDown;

            if (rb.velocityX != 0) animManager.PlayActionAnimation("Jump_Horiz_Start");
            else if (rb.velocityX == 0) animManager.PlayActionAnimation("Jump_Vert_Start");
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            coyoteTimecount = 0f;
        }
        #endregion

        #region Animation
        if ((isJumping || !IsGrounded()) && !wallSlide.isWallSliding)
        {
            if (rb.velocityX != 0) animManager.PlayActionAnimation("Jump_Horiz_Middle");
            else if (rb.velocityX == 0) animManager.PlayActionAnimation("Jump_Vert_Middle");
        }
        else
        {
            if (rb.velocity == Vector2.zero && IsGrounded() == true)
                animManager.PlayActionAnimation("Idle");
            else if (rb.velocityX != 0 && rb.velocityY == 0 && !wallSlide.isWallSliding) 
                animManager.PlayActionAnimation("Run");
        }

        if (moveDirection != 0)
            animManager.SetDirection(moveDirection);
        #endregion

        #region Pixel Velocity
        /*float velocityUnitys = rb.velocity.magnitude;
        float pixelsPerUnity = 100f;
        float velocityPixels = velocityUnitys * pixelsPerUnity;

        Debug.Log($"Velocidade em UU/s: {velocityUnitys} | Velocidade em Pixels/s: {velocityPixels}");*/
        #endregion
    }

    void FixedUpdate()
    {
        if ((grapplingHook != null && grapplingHook.IsPulling()) || (wallSlide != null && wallSlide.overrideHorizontal))
            return;

        rb.velocity = new Vector2(moveDirection * currentSpeed, rb.velocity.y);

        float clampedX = Mathf.Clamp(rb.velocity.x, -MaxSpeed, MaxSpeed);
        float clampedY = Mathf.Clamp(rb.velocity.y, -MaxSpeed, MaxSpeed);

        rb.velocity = new Vector2(clampedX, clampedY);
    }
    public void SetDirection(int value)
    {
        moveDirection = value;
    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
}
