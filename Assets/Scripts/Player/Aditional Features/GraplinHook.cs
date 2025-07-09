using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GraplinHook : MonoBehaviour
{
    public float hookDistance = 10f;
    public float hookSpeedBoostPercent = 50f;
    public LayerMask Layers;
    public Transform player;
    private Rigidbody2D playerRb;
    
    private bool isGrappling = false;
    private Vector2 grapplePoint;

    void Start()
    {
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Botão esquerdo do mouse
        {
            FireHook();
        }
    }

    void FireHook()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, hookDistance, Layers);
        
        if (hit.collider != null)
        {
            grapplePoint = hit.point;
            ApplyBoost(direction);
            isGrappling = true;

            Debug.DrawLine(transform.position, grapplePoint, Color.green, 1f);
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + (Vector3)(direction * hookDistance), Color.red, 1f);
        }
    }

    void ApplyBoost(Vector2 direction)
    {
        float boostMultiplier = 1 + (hookSpeedBoostPercent / 100f);
        Vector2 boostForce = direction * playerRb.velocity.magnitude * boostMultiplier;

        playerRb.velocity = boostForce; // Substitui velocidade ou usa AddForce se quiser acumular
    }
}
