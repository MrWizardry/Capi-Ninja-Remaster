using UnityEngine;
using System.Collections;

public class HookGrapple : MonoBehaviour
{
    [Header("Grapple Settings")]
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private float maxDistance = 15f;
    [SerializeField] private float grappleForce = 25f;
    [SerializeField] private float cooldown = 0.5f;
    [SerializeField] private KeyCode grappleKey = KeyCode.X;

    [Header("Visual")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float lineVisibleDuration = 0.15f;

    [Header("Cooldown Visual (opcional)")]
    [SerializeField] private Color colorReady = Color.white;
    [SerializeField] private Color colorCooldown = Color.red;
    private Rigidbody2D rb;
    private float cooldownTimer;
    private float lineTimer;
    private bool isGrappling;
    private Movement movement;
    public bool IsGrappling => isGrappling;

    public bool IsReady => cooldownTimer <= 0f;
    public float CooldownLeft => Mathf.Max(0f, cooldownTimer);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<Movement>();
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(grappleKey) && IsReady)
            TryGrapple();

        TickLineRenderer();
    }

    private void TryGrapple()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 origin = transform.position;
        Vector2 direction = ((Vector2)mouseWorld - origin).normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxDistance, grappleLayer);
        if (hit.collider == null) return;

        Vector2 grappleDir = (hit.point - origin).normalized;
        rb.AddForce(grappleDir * grappleForce, ForceMode2D.Impulse);

        // Salva e congela o estado do movimento ANTES do hook
        if (movement != null)
            movement.FreezeMovementState();

        isGrappling = true;
        StartCoroutine(StopGrappleAfter(0.3f));

        cooldownTimer = cooldown;
        ShowLine(origin, hit.point);

    }

    private void ShowLine(Vector2 from, Vector2 to)
    {
        if (lineRenderer == null) return;
        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, to);
        lineRenderer.enabled = true;
        lineTimer = lineVisibleDuration;
    }

    private void TickLineRenderer()
    {
        if (lineRenderer != null)
            lineRenderer.startColor = lineRenderer.endColor =
                IsReady ? colorReady : colorCooldown;

        if (lineTimer <= 0f) return;

        lineTimer -= Time.deltaTime;

        if (lineRenderer != null)
            lineRenderer.SetPosition(0, transform.position);

        if (lineTimer <= 0f && lineRenderer != null)
            lineRenderer.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsReady ? Color.cyan : Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }

    private IEnumerator StopGrappleAfter(float time)
    {
        yield return new WaitForSeconds(time);
        isGrappling = false;

        // Restaura o estado do movimento ao terminar o hook
        if (movement != null)
            movement.RestoreMovementState();
    }
}
