using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Momentum : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 momentum;

    [Header("Momentum Config")]
    [SerializeField] private float momentumDecay = 2f; // Velocidade que o momentum se perde
    [SerializeField] private float momentumMultiplier = 1f; // Multiplicador geral

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // momentum decai suave com o tempo
        momentum = Vector2.Lerp(momentum, Vector2.zero, momentumDecay * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (momentum != Vector2.zero)
        {
            rb.linearVelocity += momentum * Time.fixedDeltaTime;
        }
    }

    public void AddMomentum(Vector2 direction, float force)
    {
        momentum += direction.normalized * force * momentumMultiplier;
    }

    public void ClearMomentum()
    {
        momentum = Vector2.zero;
    }
}
