using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Super_Dash : MonoBehaviour
{
     private Rigidbody2D rb; 
    private Animator animator;
    private float horizontal; 
    [SerializeField] private float speed = 8f;
    public Dash disableDash;
    
    [Header("Dash")]
    private bool canDash = true; 
    public bool isDashing; 
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCD = 10f;

    [Header("Mouse")]
    public float maxDashDistance = 5f;
    private Vector2 worldPosition;
    [SerializeField] private Button dashButton;

    private Vector2 dashTarget;

    [Header("Dash Attack")]
    [SerializeField] private int dashDamage = 999; // Dano que o dash causa

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (isDashing)
            return;

        if (Input.GetKeyDown(KeyCode.C) && canDash)
        {
            disableDash.enabled = false; // Desabilita o script de dash normal
            StartCoroutine(FWDash());
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
            return;
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private IEnumerator FWDash()
    {
        worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (worldPosition - (Vector2)transform.position).normalized;
        dashTarget = (Vector2)transform.position + direction * maxDashDistance;

        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 5f;
        dashButton.interactable = true;

        rb.velocity = direction * dashingPower;

        animator.SetBool("IsDashing", true);

        yield return new WaitForSeconds(dashingTime);

        animator.SetBool("IsDashing", false);
        isDashing = false;
        dashButton.interactable = false;
        rb.gravityScale = originalGravity;

        yield return new WaitForSeconds(dashingCD);
        canDash = true;
        disableDash.enabled = true;
        dashButton.interactable = true;
    }

    public bool IsDashing()
    {
        return isDashing;
    }

    private void OnCollisionEnter2D(Collision2D collision)
{
    if (!isDashing) return;

    EnemyLife enemy = collision.collider.GetComponent<EnemyLife>();
    if (enemy != null)
    {
        enemy.TakeDamage(dashDamage);

        if (enemy.IsDead())
        {
            ResetDash();
        }
    }
}

    private void ResetDash()
    {
        StopAllCoroutines();
        isDashing = false;
        disableDash.enabled = true; // Reabilita o script de dash normal
        canDash = true;
        dashButton.interactable = true;
        animator.SetBool("IsDashing", false);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, dashTarget);
    }
}