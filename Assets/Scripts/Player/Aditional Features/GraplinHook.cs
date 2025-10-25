using System.Collections;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private float maxDistancePixels = 560f;
    [SerializeField] private float pullSpeed = 10f;
    [SerializeField] private float momentumForce = 15f;
    [SerializeField] private float pullForceUP = 5f;
    [SerializeField] private float grappleCooldown = 0.5f;

    [Header("Status (Read Only)")]
    [SerializeField] private Vector2 grapplePoint;
    [SerializeField] private bool isOnCooldown = false;

    private Rigidbody2D rb;
    private bool isPulling = false;
    private bool isHooked = false;
    private bool canGrapple = true;
    private Momentum momentum;

    private Camera mainCamera;
    private float maxDistanceUnits => maxDistancePixels / 100f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        momentum = GetComponent<Momentum>();
        mainCamera = Camera.main;
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        if (Game.Instance.isReceivingInputs())
        {
            if (Custom_Input.GetKeyDown("Grapple") && canGrapple)
            {
                TryGrapple();
            }

            if (Custom_Input.GetKey("Grapple") && isHooked)
            {
                isPulling = true;
            }

            if (Custom_Input.GetKeyUp("Grapple"))
            {
                if (!isPulling && isHooked)
                {
                    Vector2 direction = (grapplePoint - rb.position).normalized;
                    momentum.AddMomentum(direction, momentumForce);
                }

                ResetGrapple();
            }

            if (isHooked)
            {
                DrawLine();
            }

            if ((Vector2)transform.position == grapplePoint)
            {
                ResetGrapple();
            }
        }
    }

    void FixedUpdate()
    {
        if (isPulling && isHooked)
        {
            Vector2 direction = (grapplePoint - rb.position).normalized;
            float distance = Vector2.Distance(rb.position, grapplePoint);

            RaycastHit2D hit = Physics2D.Raycast(rb.position, direction, distance, grappleLayer);
            if (hit.collider != null && Vector2.Distance(hit.point, grapplePoint) > 0.1f)
            {
                ResetGrapple();
                return;
            }

            float currentSpeed = GetComponent<Movement>().currentSpeed;
            if (currentSpeed != 0)
                rb.velocity = currentSpeed * (direction * pullSpeed);
            else
                rb.velocity = direction * (pullSpeed * pullForceUP);

            if (distance < 0.5f)
            {
                direction = (grapplePoint - rb.position).normalized;
                GetComponent<Momentum>().AddMomentum(direction, momentumForce * 0.5f);
                ResetGrapple();
            }
        }
    }

    void TryGrapple()
    {
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 origin = rb.position;
        Vector2 direction = (mouseWorldPos - origin).normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxDistanceUnits, grappleLayer);

        if (hit.collider != null)
        {
            grapplePoint = hit.point;
            isHooked = true;
            lineRenderer.positionCount = 2;

            Movement movement = GetComponent<Movement>();
            float inputDir = Mathf.Sign(direction.x);
            float moveDir = Mathf.Sign(movement.rb.velocity.x);

            if (moveDir == inputDir && Mathf.Abs(movement.currentSpeed) > 0.1f)
            {
                rb.velocity += direction * movement.currentSpeed;
            }
            else
            {
                rb.velocity = Vector2.zero;
                movement.currentSpeed = 0f;
                movement.animManager.SetDirection(inputDir);
            }

            // Inicia cooldown ao usar o gancho
            StartCoroutine(StartGrappleCooldown());
        }
    }

    IEnumerator StartGrappleCooldown()
    {
        canGrapple = false;
        isOnCooldown = true;
        yield return new WaitForSeconds(grappleCooldown);
        canGrapple = true;
        isOnCooldown = false;
    }

    void ResetGrapple()
    {
        isPulling = false;
        isHooked = false;
        lineRenderer.positionCount = 0;
    }

    void DrawLine()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }

    public bool IsPulling()
    {
        return isPulling;
    }

    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }
}
