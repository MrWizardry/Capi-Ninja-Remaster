using System.Collections;
using System.Runtime.CompilerServices;
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

    [SerializeField] private Vector2 grapplePoint;
    private Rigidbody2D rb;
    private bool isPulling = false;
    private bool isHooked = false;

    private Camera mainCamera;
    private float maxDistanceUnits => maxDistancePixels / 100f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        if (Custom_Input.GetKeyDown("Grapple"))
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
                // aplicar momentum apenas se soltar rápido
                Vector2 direction = (grapplePoint - rb.position).normalized;
                rb.velocity += direction * momentumForce;
            }

            ResetGrapple();
        }

        if (isHooked)
        {
            DrawLine();
        }
        if ((Vector2)transform.position == grapplePoint) // Reset position if below a certain point
        {
            ResetGrapple();
        }
    }

    void FixedUpdate()
    {
        if (isPulling && isHooked)
        {
            Vector2 direction = (grapplePoint - rb.position).normalized;
            float distance = Vector2.Distance(rb.position, grapplePoint);

            // Verifica colisão no caminho
            RaycastHit2D hit = Physics2D.Raycast(rb.position, direction, distance, grappleLayer);
            if (hit.collider != null && Vector2.Distance(hit.point, grapplePoint) > 0.1f)
            {
                ResetGrapple(); // Colidiu com algo no meio, cancela
                return;
            }

            // Assuming you have a Movement component attached to the same GameObject
            float currentSpeed = GetComponent<Movement>().currentSpeed;
            if(currentSpeed != 0)
                rb.velocity = currentSpeed * (direction * pullSpeed);
            else
                rb.velocity = direction * (pullSpeed * pullForceUP);    // Move o Rigidbody na direção do ponto de grappling
            //direction * pullSpeed 
            if (distance < 0.5f)
            {
                ResetGrapple(); // Chegou ao ponto
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

            // --- NOVO SISTEMA DE IMPULSO ---
            Movement movement = GetComponent<Movement>();
            float inputDir = Mathf.Sign(direction.x); // Direção do grapple (esquerda -1 / direita +1)
            float moveDir = Mathf.Sign(movement.rb.velocity.x); // Direção atual do player

            if (moveDir == inputDir && Mathf.Abs(movement.currentSpeed) > 0.1f)
            {
                // Se a direção for a mesma, soma velocidade (impulso extra)
                rb.velocity += direction * movement.currentSpeed;
            }
            else
            {
                // Se a direção for contrária, zera a velocidade e "vira"
                rb.velocity = Vector2.zero;
                movement.currentSpeed = 0f;
                movement.animManager.SetDirection(inputDir); // Faz o personagem virar
            }
        }
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
}
