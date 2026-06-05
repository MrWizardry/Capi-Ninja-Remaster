using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Momentum : MonoBehaviour
{
    [Header("Momentum Config")]
    [SerializeField] private float momentumMultiplier = 1f;
    [SerializeField] private float maxMomentum = 20f;
    private Rigidbody2D rb;
    private Vector2 pendingImpulse;
    public float MaxMomentum => maxMomentum;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (pendingImpulse == Vector2.zero) return;

        rb.AddForce(pendingImpulse, ForceMode2D.Impulse);
        pendingImpulse = Vector2.zero;
    }

    public void AddMomentum(Vector2 direction, float force)
    {
        Vector2 added = direction.normalized * (force * momentumMultiplier);
        pendingImpulse += added;

        if (pendingImpulse.magnitude > maxMomentum)
            pendingImpulse = pendingImpulse.normalized * maxMomentum;
    }

    public void ClearMomentum()
    {
        pendingImpulse    = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }
}
