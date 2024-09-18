using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Andar")]
    private float horizontal;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 16f;
    private bool isFacingRight = true;
    private Rigidbody2D rb;

    [Header("Dash")]
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCD = 1f;

    [Header("Aplicações")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Button dashButton;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(isDashing)
            return;

        horizontal = Input.GetAxisRaw("Horizontal");

        if(Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpingPower);
        }

        if(Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        Flip();
    }

    void FixedUpdate()
    {
        if(isDashing)
            return;

        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if(isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        dashButton.interactable = true;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
        dashButton.interactable = false;
        rb.gravityScale = originalGravity;
        yield return new WaitForSeconds(dashingCD);
        canDash = true;
        dashButton.interactable = true;
    }
}