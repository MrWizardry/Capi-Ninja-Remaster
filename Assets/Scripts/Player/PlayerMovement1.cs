using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement1 : MonoBehaviour
{
    // --- MOVIMENTO ---
    [Header("Andar")]
    private float horizontal; // Direção do input horizontal
    [SerializeField] private float speed = 8f; // Velocidade do player
    [SerializeField] private float jumpingPower = 16f; // Força do pulo
    private bool isFacingRight = true; // Checa se o player tá virado pra direita
    private Rigidbody2D rb; // Rigidbody do player

    // --- REFERÊNCIAS / LAYERS ---
    [Header("Aplicações")]
    [SerializeField] private Transform groundCheck; // Posição usada pra checar se está no chão
    [SerializeField] private LayerMask groundLayer; // Layer do chão
    [SerializeField] private LayerMask RespawnLayer; // Layer de "morte"


    // --- JUMP SYSTEM ---
    [Header("Jump")]
    [SerializeField] private float coyoteTime = 0.2f; // Tempo extra pra permitir pulo depois de sair do chão
    private float coyoteTimecount; // Contador interno do coyote time

    private bool isJumping;
    [SerializeField] private float jumpbuffer = 0.2f; // Buffer pra quando o player aperta pulo cedo demais
    private float jumpbuffercount; // Contador interno do buffer

    [SerializeField] private float jumpCoolDown = 0.2f; // Tempo entre pulos
    private float jumpCooldownTimer = 0f; // Contador de cooldown do pulo

    private string currentSceneName; // Nome da cena atual pra reiniciar
    private AnimationManager animManager;
    private Dash dash;

    private WallSlide wallSlide;

    // --- START ---
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Pega o Rigidbody2D no Start
        currentSceneName = SceneManager.GetActiveScene().name; // Salva o nome da cena atual
        animManager= GetComponent<AnimationManager>();
        dash = GetComponent<Dash>(); // Pega o script de dash
        isJumping = false;
        wallSlide = GetComponent<WallSlide>();
    }

    // --- UPDATE ---
    void Update()
    {
        #region Jump Functions
        // INPUT HORIZONTAL (Setas ou A/D)
        horizontal = Input.GetAxisRaw("Horizontal");

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

            if(rb.velocityX != 0) animManager.PlayActionAnimation("Jump_Horiz_Start");
            else if(rb.velocityX == 0) animManager.PlayActionAnimation("Jump_Vert_Start");

        }

        // SE SOLTAR O BOTÃO DE PULO ENQUANTO SOBE, corta o pulo (pulo mais curto)
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            coyoteTimecount = 0f; // Zera o coyote pra evitar pulo duplo

        } 
        else{}
        #endregion
        // CHECA SE CAIU NA "ZONA DE MORTE"
        if (Respawncheck())
            SceneManager.LoadScene(currentSceneName);
        
        #region ANIMATIONS
            
        if((isJumping || !IsGrounded()) && !wallSlide.isWallSliding)
        {
            if(rb.velocityX != 0) animManager.PlayActionAnimation("Jump_Horiz_Middle");
            else if(rb.velocityX == 0) animManager.PlayActionAnimation("Jump_Vert_Middle");
        }
        else
        {
            if(rb.velocity == Vector2.zero && IsGrounded() == true)
                animManager.PlayActionAnimation("Idle");
            else if (rb.velocityX != 0 && rb.velocityY == 0)
                animManager.PlayActionAnimation("Run");
        }
        

        animManager.SetDirection(horizontal);
        #endregion
    } 

    // --- CHECA SE TOCOU NA ZONA DE MORTE ---
    private bool Respawncheck()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, RespawnLayer);
    }

    // --- CHECA SE ESTÁ NO CHÃO ---
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
}