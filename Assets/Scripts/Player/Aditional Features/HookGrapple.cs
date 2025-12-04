using UnityEngine;
using System.Collections;

public class GrapplingHookSimple : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private float grappleSpeed = 20f;
    [SerializeField] private float maxDistance = 15f;
    [Range(0.1f,0.9f)]
    [SerializeField] private float stopDuration = 0.5f; // tempo parado no ponto
    [Range(2, 10)]
    [SerializeField] private int gravityReturnSpeed = 2; // velocidade do retorno da gravidade
    [SerializeField] private KeyCode grappleKey = KeyCode.X;
    [SerializeField] private float overshootForce = 10f;

    [Header("Dependências")]
    [SerializeField] private LineRenderer lineRenderer;

    private Vector3 targetPoint;
    private bool isGrappling = false;

    private Rigidbody2D rb;
    private float originalGravity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            originalGravity = rb.gravityScale;
    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey))
        {
            TryStartGrapple();
        }

        if (isGrappling)
        {
            MoveTowardsTarget();
        }
    }

    private void TryStartGrapple()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - transform.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grappleLayer);
        if (hit.collider != null)
        {
            targetPoint = hit.point;
            isGrappling = true;

            if (rb != null)
                rb.gravityScale = 0f;

            if (lineRenderer)
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, targetPoint);
            }
        }
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, grappleSpeed * Time.deltaTime);

        if (lineRenderer)
            lineRenderer.SetPosition(0, transform.position);

        // Chegou no ponto → pausa por 0.5s
        if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        {
            StartCoroutine(StopAtPoint());
        }
    }

    private IEnumerator StopAtPoint()
    {
        isGrappling = false;

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;
        }

        // Calcula direção do impulso além do ponto
        Vector2 direction = (targetPoint - transform.position).normalized;

        // Aplica impulso imediatamente
        rb.AddForce(direction * overshootForce, ForceMode2D.Impulse);

        // Opcional: espera um pouquinho antes de voltar gravidade
        yield return new WaitForSeconds(0.1f);

        // Ativa gravidade suave de novo
        if (rb != null)
            yield return StartCoroutine(SmoothGravityReturn());

        if (lineRenderer)
            lineRenderer.enabled = false;
    }

    private IEnumerator SmoothGravityReturn()
    {
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * gravityReturnSpeed;
            rb.gravityScale = Mathf.Lerp(0f, originalGravity, elapsed);
            yield return null;
        }
        rb.gravityScale = originalGravity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}
