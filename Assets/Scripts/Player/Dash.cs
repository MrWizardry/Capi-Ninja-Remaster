using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class Dash : MonoBehaviour
{
    private Rigidbody2D rb; // Rigidbody do player
    private Animator animator;
    private float horizontal; // Direção do input horizontal
    [SerializeField] private float speed = 8f; // Velocidade do player

    // --- DASH ---
    [Header("Dash")]
    private bool canDash = true; // Controle de cooldown do dash
    public bool isDashing; // Checa se está dashing
    private float dashingPower = 24f; // Velocidade do dash
    private float dashingTime = 0.2f; // Duração do dash
    private float dashingCD = 1f; // Cooldown entre dashes

    [Header("Mouse")]
    public float maxDashDistance = 5f; // Distância máxima que pode dar dash
    private Vector2 worldPosition;
    [SerializeField] private Button dashButton; // Botão de dash (UI)

    private Vector2 dashTarget; // Posição final do dash (usado pra gizmo)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Pega o Rigidbody2D no Start
        animator = GetComponent<Animator>(); // Pega o Animator do player
    }


    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (isDashing)
            return;

        // SE APERTAR SHIFT E PUDER DAR DASH
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(FWDash());
            
        }
    }

    void FixedUpdate()
    {
            if (isDashing)
            return;
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    // --- DASH COROUTINE ---
    private IEnumerator FWDash()
    {
        // Pega a posição do mouse na tela e converte pra mundo
        worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calcula direção do player até o mouse
        Vector2 direction = (worldPosition - (Vector2)transform.position).normalized;

        // Calcula a posição final limitada pela distância máxima
        dashTarget = (Vector2)transform.position + direction * maxDashDistance;

        // Inicia o dash
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 5f; // Temporariamente aumenta a gravidade pra não sair flutuando
        dashButton.interactable = true; // Libera o botão de dash (caso esteja usando UI)

        // Aplica o dash com força na direção
        rb.velocity = direction * dashingPower;

        animator.SetBool("IsDashing", true); // Ativa animação de dash

        yield return new WaitForSeconds(dashingTime); // Espera o tempo do dash

        animator.SetBool("IsDashing", false);
        // Termina o dash
        isDashing = false;
        dashButton.interactable = false;
        rb.gravityScale = originalGravity;

        // Espera o cooldown antes de permitir outro dash
        yield return new WaitForSeconds(dashingCD);
        canDash = true;
        dashButton.interactable = true;
    }
    public bool IsDashing()
    {
        return isDashing;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, dashTarget);
    }
}