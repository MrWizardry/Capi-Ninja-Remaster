using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBase : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 5f;
    public LayerMask playerLayer;
    public LayerMask groundLayer;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    public float groundCheckOffsetX = 0.5f;
    public float groundCheckOffsetY = -1f;
    public float wallCheckOffsetX = 0.5f;
    public float groundCheckDistance = 1f;
    public float wallCheckDistance = 0.2f;

    private Rigidbody2D rb;
    private bool isPlayerDetected = false;
    private int direction = 1; // 1 = direita, -1 = esquerda

    [Range(0, 360)] public float fieldOfViewAngle = 90f; // Ângulo do cone de visão
    public int visionSegments = 30; // Quanto mais segmentos, mais suave o cone

    private bool canMove = true;

    private Animator animManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animManager = GetComponent<Animator>();
    }

    private void Update()
    {
        DetectPlayer();

       if(!canMove) return; // Se não pode mover, sai do Update

        if (isPlayerDetected)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void DetectPlayer()
    {
        Vector2 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= detectionRange)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer.normalized, detectionRange, playerLayer);
            isPlayerDetected = hit.collider != null && hit.collider.CompareTag("Player");
        }
        else
        {
            isPlayerDetected = false;
        }
    }

    private void ChasePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * chaseSpeed, rb.linearVelocity.y);
        if ((dir.x > 0 && direction < 0) || (dir.x < 0 && direction > 0))
        {
            Flip();
        }
        animManager.Play("Run");
        Debug.Log("Achou!");
    }

    private void Patrol()
    {
        // Posição dinâmica para checar o chão
        Vector2 groundCheckOrigin = (Vector2)transform.position + new Vector2(direction * groundCheckOffsetX, groundCheckOffsetY);
        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheckOrigin, Vector2.down, groundCheckDistance, groundLayer);

        // Posição dinâmica para checar a parede
        Vector2 wallCheckOrigin = (Vector2)transform.position + new Vector2(direction * wallCheckOffsetX, 0f);
        RaycastHit2D wallInfo = Physics2D.Raycast(wallCheckOrigin, Vector2.right * direction, wallCheckDistance, groundLayer);

        if (!groundInfo || wallInfo)
        {
            Flip();
        }
        animManager.Play("Walk");
        rb.linearVelocity = new Vector2(direction * patrolSpeed, rb.linearVelocity.y);
    }

    private void Flip()
    {
        direction *= -1;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isPlayerDetected ? Color.green : Color.red;

    // Desenhar o "cone de visão"
    if (Application.isPlaying && player != null)
    {
        DrawVisionCone();
    }

    // Raycasts de chão e parede
    if (Application.isPlaying)
    {
        Vector2 groundCheckOrigin = (Vector2)transform.position + new Vector2(direction * groundCheckOffsetX, groundCheckOffsetY);
        Vector2 wallCheckOrigin = (Vector2)transform.position + new Vector2(direction * wallCheckOffsetX, 0f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(groundCheckOrigin, groundCheckOrigin + Vector2.down * groundCheckDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(wallCheckOrigin, wallCheckOrigin + Vector2.right * direction * wallCheckDistance);
    }
    }

    private void DrawVisionCone()
    {
        Vector3 origin = transform.position;
        Vector3 forward = (player.position - transform.position).normalized;

        float halfFOV = fieldOfViewAngle / 2f;
        float startAngle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg - halfFOV;

        Vector3 prevPoint = origin;
        for (int i = 0; i <= visionSegments; i++)
        {
            float angle = startAngle + (fieldOfViewAngle * i / visionSegments);
            float rad = angle * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
            Vector3 point = origin + direction * detectionRange;

            if (i > 0)
            {
                Gizmos.DrawLine(prevPoint, point);
            }

            prevPoint = point;
        }

        // Conectar de volta ao centro para formar o "cone"
        Gizmos.DrawLine(origin, prevPoint);
    }

    public void StopMovement()
    {
        canMove = false;
        rb.linearVelocity = Vector2.zero; // Para o movimento imediatamente
    }
    public void ResumeMovement()
    {
        canMove = true;
    }
    public void PlayAttackAnim()
    {
        animManager.Play("Attack");
    }
}
