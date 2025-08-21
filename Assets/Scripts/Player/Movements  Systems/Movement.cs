using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    // --- MOVIMENTO ---
    [Header("Andar")]
    private float inputDirection = 0f;
    private float moveDirection = 0f;
    [SerializeField] private float walk = 6f; // Velocidade do player
    [SerializeField] private float running = 8f;
    [SerializeField] private float accelerationTime = 2f;
    [SerializeField] private float decelerationTime = 1f; // Tempo pra acelerar e desacelerar
    public float currentSpeed;
    private float accelerationTimer = 0f;
    private Rigidbody2D rb; // Rigidbody do player


    // --- LAYERS ---
    [Header("Aplicações")]
    [SerializeField] private Transform groundCheck; // Posição usada pra checar se está no chão
    [SerializeField] private LayerMask groundLayer; // Layer do chão

    // --- JUMP SYSTEM ---
    [Header("Jump")]
    [SerializeField] private float jumpingPower = 16f; // Força do pulo
    [SerializeField] private float coyoteTime = 0.2f; // Tempo extra pra permitir pulo depois de sair do chão
    private float coyoteTimecount; // Contador interno do coyote time

    private bool isJumping;
    [SerializeField] private float jumpbuffer = 0.2f; // Buffer pra quando o player aperta pulo cedo demais
    private float jumpbuffercount; // Contador interno do buffer

    [SerializeField] private float jumpCoolDown = 0.2f; // Tempo entre pulos
    private float jumpCooldownTimer = 0f; // Contador de cooldown do pulo
    private AnimationManager animManager;
    private WallSlide wallSlide;
    private GrapplingHook grapplingHook;

    // --- START ---
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Pega o Rigidbody2D no Start
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
        {
            inputDirection = 1f; // Direção direita

        }
        else if (Custom_Input.GetKey("Left"))
        {
            inputDirection = -1f; // Direção esquerda

        }
        else
        {
            inputDirection = 0f; // Sem direção
        }
        if (inputDirection != 0)
        {
            accelerationTimer += Time.deltaTime;
            currentSpeed = Mathf.Lerp(0f, running, accelerationTimer / accelerationTime);
            moveDirection = inputDirection;
        }
        else
        {
            accelerationTimer = 0f;

            // Desacelera suavemente, mantendo a moveDirection
            if (currentSpeed > 0f)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime / decelerationTime);
                if (currentSpeed < 1f)
                {
                    currentSpeed = 0f;
                }
            }
        }

        #endregion

        #region Jump Functions

        // COYOTE TIME — se está no chão, reseta o tempo
        if (IsGrounded())
        {
            coyoteTimecount = coyoteTime;
            isJumping = false;
        }
        else
            coyoteTimecount -= Time.deltaTime;


        // COOLDOWN DO PULO — conta regressiva
        if (jumpCooldownTimer > 0f)
            jumpCooldownTimer -= Time.deltaTime;

        // BUFFER DE PULO — salva se o jogador apertar antes de poder pular
        if (Input.GetButtonDown("Jump"))
            jumpbuffercount = jumpbuffer;
        else
            jumpbuffercount -= Time.deltaTime;

        // CONDIÇÃO PRA PULAR:
        if (jumpbuffercount > 0f && coyoteTimecount > 0f && jumpCooldownTimer <= 0f)
        {
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower); // Aplica força do pulo
            jumpbuffercount = 0f; // Zera o buffer
            jumpCooldownTimer = jumpCoolDown; // Ativa o cooldown

            if (rb.velocityX != 0) animManager.PlayActionAnimation("Jump_Horiz_Start");
            else if (rb.velocityX == 0) animManager.PlayActionAnimation("Jump_Vert_Start");

        }

        // SE SOLTAR O BOTÃO DE PULO ENQUANTO SOBE, corta o pulo (pulo mais curto)
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            coyoteTimecount = 0f; // Zera o coyote pra evitar pulo duplo

        }
        else { }
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
            else if (rb.velocityX != 0 && rb.velocityY == 0)
                animManager.PlayActionAnimation("Run");
        }

        if (inputDirection != 0)
        {
            animManager.SetDirection(inputDirection);
        }
        #endregion

    }
    void FixedUpdate()
    {
        /*if (wallSlide == null || !wallSlide.overrideHorizontal)
        {
            rb.velocity = new Vector2(moveDirection * currentSpeed, rb.velocity.y);
        }*/

        if ((grapplingHook != null && grapplingHook.IsPulling()) || (wallSlide != null && wallSlide.overrideHorizontal))
        {
            return;
        }

        rb.velocity = new Vector2(moveDirection * currentSpeed, rb.velocity.y);
    }

    // --- CHECA SE ESTÁ NO CHÃO ---
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

}