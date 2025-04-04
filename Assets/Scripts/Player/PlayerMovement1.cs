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

    // --- DASH ---
    [Header("Dash")]
    private bool canDash = true; // Controle de cooldown do dash
    private bool isDashing; // Checa se está dashing
    private float dashingPower = 24f; // Velocidade do dash
    private float dashingTime = 0.2f; // Duração do dash
    private float dashingCD = 1f; // Cooldown entre dashes

    // --- REFERÊNCIAS / LAYERS ---
    [Header("Aplicações")]
    [SerializeField] private Transform groundCheck; // Posição usada pra checar se está no chão
    [SerializeField] private LayerMask groundLayer; // Layer do chão
    [SerializeField] private LayerMask RespawnLayer; // Layer de "morte"
    [SerializeField] private LayerMask WallLayer; // (Não usado ainda)
    [SerializeField] private Button dashButton; // Botão de dash (UI)

    // --- JUMP SYSTEM ---
    [Header("Jump")]
    [SerializeField] private float coyoteTime = 0.2f; // Tempo extra pra permitir pulo depois de sair do chão
    private float coyoteTimecount; // Contador interno do coyote time

    [SerializeField] private float jumpbuffer = 0.2f; // Buffer pra quando o player aperta pulo cedo demais
    private float jumpbuffercount; // Contador interno do buffer

    [SerializeField] private float jumpCoolDown = 0.2f; // Tempo entre pulos
    private float jumpCooldownTimer = 0f; // Contador de cooldown do pulo

    private string currentSceneName; // Nome da cena atual pra reiniciar

    // --- DASH COM O MOUSE ---
    [Header("Mouse")]
    private Vector2 screenPosition;
    private Vector2 worldPosition;
    public GameObject dashTo; // (Não está sendo usado ainda)
    public float maxDashDistance = 5f; // Distância máxima que pode dar dash

    // --- START ---
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Pega o Rigidbody2D no Start
        currentSceneName = SceneManager.GetActiveScene().name; // Salva o nome da cena atual
    }

    // --- UPDATE ---
    void Update()
    {
        // INPUT HORIZONTAL (Setas ou A/D)
        horizontal = Input.GetAxisRaw("Horizontal");

        // COYOTE TIME — se está no chão, reseta o tempo
        if (IsGrounded())
            coyoteTimecount = coyoteTime;
        else
            coyoteTimecount -= Time.deltaTime;

        // SE ESTIVER DANDO DASH, ignora o resto do Update
        if (isDashing)
            return;

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
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower); // Aplica força do pulo
            jumpbuffercount = 0f; // Zera o buffer
            jumpCooldownTimer = jumpCoolDown; // Ativa o cooldown
        }

        // SE SOLTAR O BOTÃO DE PULO ENQUANTO SOBE, corta o pulo (pulo mais curto)
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            coyoteTimecount = 0f; // Zera o coyote pra evitar pulo duplo
        }

        // SE APERTAR SHIFT E PUDER DAR DASH
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        // FLIP — vira o personagem pro lado correto
        Flip();

        // CHECA SE CAIU NA "ZONA DE MORTE"
        if (Respawncheck())
            SceneManager.LoadScene(currentSceneName);
    }

    // --- FIXED UPDATE (Física) ---
    void FixedUpdate()
    {
        if (isDashing)
            return;

        // Aplica movimento horizontal
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    // --- FLIP SPRITE ---
    private void Flip()
    {
        // Se mudar de direção, vira o sprite horizontalmente
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    // --- DASH COROUTINE ---
    private IEnumerator Dash()
    {
        // Pega a posição do mouse na tela e converte pra mundo
        worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calcula direção do player até o mouse
        Vector2 direction = (worldPosition - (Vector2)transform.position).normalized;

        // Calcula a posição final limitada pela distância máxima
        Vector2 dashTarget = (Vector2)transform.position + direction * maxDashDistance;

        // Inicia o dash
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 5f; // Temporariamente aumenta a gravidade pra não sair flutuando
        dashButton.interactable = true; // Libera o botão de dash (caso esteja usando UI)

        // Aplica o dash com força na direção
        rb.velocity = direction * dashingPower;

        yield return new WaitForSeconds(dashingTime); // Espera o tempo do dash

        // Termina o dash
        isDashing = false;
        dashButton.interactable = false;
        rb.gravityScale = originalGravity;

        // Espera o cooldown antes de permitir outro dash
        yield return new WaitForSeconds(dashingCD);
        canDash = true;
        dashButton.interactable = true;
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
