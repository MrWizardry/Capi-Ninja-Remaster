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
    public float wallJumpForceX = 10f;
    public float wallJumpForceY = 15f;

    private Rigidbody2D rb;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canWallJump;
    private float wallStickCounter;
    private int wallDirection; // -1 (esquerda) ou 1 (direita)

    private float wallJumpGraceTime = 0.2f;
    private float wallJumpGraceCounter = 0f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Cria ponto de checagem da parede, se não tiver
        if (wallCheck == null)
        {
            GameObject check = new GameObject("WallCheck");
            check.transform.SetParent(transform);
            check.transform.localPosition = new Vector2(0.2f, 0f); // Ligeiramente à direita
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
                // Grudado na parede
                rb.velocity = new Vector2(0, 0);
            }
            else
            {
                // Escorrega
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }

        // ✅ Sempre contamos o tempo de "graça"
        wallJumpGraceCounter -= Time.deltaTime;

        // ✅ Pulo de parede com coyote time
        if ((isWallSliding || wallJumpGraceCounter > 0f) && Input.GetButtonDown("Jump"))
        {
            float inputX = Input.GetAxisRaw("Horizontal");

            // Decide direção do pulo:
            int jumpDirection = (inputX != 0 && Mathf.Sign(inputX) != wallDirection) ? (int)Mathf.Sign(inputX) : -wallDirection;

            // Ajusta força horizontal se for pulo de subidinha estilo Mega Man
            float appliedForceX = (inputX == 0) ? wallJumpForceX * 1.2f : wallJumpForceX;

            // Aplica pulo
            rb.velocity = new Vector2(appliedForceX * jumpDirection, wallJumpForceY);

            isWallSliding = false;
            wallJumpGraceCounter = 0f; // Resetamos o tempo de pulo de parede
        }
    }

    void CheckWall()
    {
        // Detecta parede à esquerda ou direita
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

            // ← Aqui salvamos o momento em que estava na parede
            wallJumpGraceCounter = wallJumpGraceTime;
        }
        else
        {
            isWallSliding = false;
        }
    }

    bool IsGrounded()
    {
        // Usa o mesmo método do seu script principal, você pode ajustar esse check depois
        return Physics2D.Raycast(transform.position, Vector2.down, 1f, LayerMask.GetMask("Ground"));
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
