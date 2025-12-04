using Unity.VisualScripting;
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
    [SerializeField] private float AirMultiplier = 0.75f;

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
        #region Input Reading
        if(Game.Instance.isReceivingInputs())
        {
            #region Movement
            if (Custom_Input.GetKey("Right"))
                inputDirection = 1f;
            else if (Custom_Input.GetKey("Left"))
                inputDirection = -1f;
            else
                inputDirection = 0f;

            if (IsGrounded())
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
                    moveDirection = Mathf.MoveTowards(moveDirection, inputDirection, (driftSpeed * AirMultiplier) * Time.deltaTime);
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
                    moveDirection = Mathf.MoveTowards(moveDirection, 0f, (driftSpeed * AirMultiplier) * Time.deltaTime);
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
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
                jumpbuffercount = 0f;
                jumpCooldownTimer = jumpCoolDown;

                if (rb.linearVelocityX != 0) animManager.PlayActionAnimation("Jump_Horiz_Start");
                else if (rb.linearVelocityX == 0) animManager.PlayActionAnimation("Jump_Vert_Start");
            }

            if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                coyoteTimecount = 0f;
            }
            #endregion

            #region Animation
            if ((isJumping || !IsGrounded()) && !wallSlide.isWallSliding)
            {
                if (rb.linearVelocityX != 0) animManager.PlayActionAnimation("Jump_Horiz_Middle");
                else if (rb.linearVelocityX == 0) animManager.PlayActionAnimation("Jump_Vert_Middle");
            }
            else
            {
                if (rb.linearVelocity == Vector2.zero && IsGrounded() == true)
                    animManager.PlayActionAnimation("Idle");
                else if (rb.linearVelocityX != 0 && rb.linearVelocityY == 0 && !wallSlide.isWallSliding)
                    animManager.PlayActionAnimation("Run");
            }

            if (moveDirection != 0)
                animManager.SetDirection(moveDirection);
        }
        #endregion

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

        rb.linearVelocity = new Vector2(moveDirection * currentSpeed, rb.linearVelocity.y);

        float clampedX = Mathf.Clamp(rb.linearVelocity.x, -MaxSpeed, MaxSpeed);
        float clampedY = Mathf.Clamp(rb.linearVelocity.y, -MaxSpeed, MaxSpeed);

        rb.linearVelocity = new Vector2(clampedX, clampedY);
    }
    public void SetDirection(int value)
    {
        moveDirection = value;
    }
    private bool IsGrounded()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, 0.3f, groundLayer);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * 0.3f);
    }
}
