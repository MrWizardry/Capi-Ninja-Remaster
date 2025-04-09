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
    public bool isTouchingWall;
    public bool isWallSliding;
    private bool canWallJump;
    private float wallStickCounter;
    private int wallDirection; // -1 (esquerda) ou 1 (direita)

    private float wallJumpGraceTime = 0.2f;
    private float wallJumpGraceCounter = 0f;
    private Animator anim;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

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

        // Wall Slide Physics
        if (isWallSliding)
        {
            wallStickCounter -= Time.deltaTime;

            if (wallStickCounter > 0)
            {
                rb.velocity = new Vector2(0, 0);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }

        // Wall jump buffer
        wallJumpGraceCounter -= Time.deltaTime;

        // Wall jump input
        if ((isWallSliding || wallJumpGraceCounter > 0f) && Input.GetButtonDown("Jump"))
        {
            float inputX = Input.GetAxisRaw("Horizontal");

            int jumpDirection = (inputX != 0 && Mathf.Sign(inputX) != wallDirection) ? (int)Mathf.Sign(inputX) : -wallDirection;
            float appliedForceX = (inputX == 0) ? wallJumpForceX * 1.2f : wallJumpForceX;

            rb.velocity = new Vector2(appliedForceX * jumpDirection, wallJumpForceY);

            isWallSliding = false;
            isTouchingWall = false;
            wallStickCounter = 0f;
            wallJumpGraceCounter = 0f;

            // FORÇA a animação parar após pulo
            anim.SetBool("IsWS", false);
        }

        UpdateWallSlideAnimation(); // Deixa isso separado
    }

    void CheckWall()
{
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
        wallJumpGraceCounter = wallJumpGraceTime;
    }
    else if(IsGrounded() || !isTouchingWall)
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

    void UpdateWallSlideAnimation()
    {
        bool isGrounded = IsGrounded();

        // Se tocar o chão, força sair da animação
        if (isGrounded)
        {
            anim.SetBool("IsWS", false);
        }
        // WS continua enquanto estiver deslizando, ou seja: tocando parede, no ar e descendo
        bool isWallSticking = isWallSliding && !isGrounded;

        anim.SetBool("IsWS", isWallSticking);
        anim.SetInteger("WallCon", wallDirection);
        anim.SetFloat("VerticalSpeed", rb.velocity.y);

    }

}