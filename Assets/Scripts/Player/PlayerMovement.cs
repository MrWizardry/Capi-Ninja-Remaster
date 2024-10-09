using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;
using NUnit.Framework;
using UnityEngine.SceneManagement;

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
    [SerializeField] private LayerMask RespawnLayer;
    [SerializeField] private Button dashButton;

    [Header("Coyote & Buffer")]
    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteTimecount;
    [SerializeField] private float jumpbuffer = 0.2f;
    private float jumpbuffercount;

    private string currentSceneName;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    void Update()
    {   
        horizontal = Input.GetAxisRaw("Horizontal");

        if(IsGrounded())
            coyoteTimecount = coyoteTime;
        else
            coyoteTimecount -= Time.deltaTime;

        if(isDashing)
            return;
        if(Input.GetButtonDown("Jump"))
            jumpbuffercount = jumpbuffer;
        else
            jumpbuffercount -= Time.deltaTime;

        if(jumpbuffercount > 0f && coyoteTimecount > 0f)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpingPower);

            jumpbuffercount = 0f;
        }

        if(Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.5f);

            coyoteTimecount = 0f;
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
        
        Flip();

        if(Respawncheck())
            SceneManager.LoadScene(currentSceneName);
        
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

    private bool Respawncheck()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, RespawnLayer);
    }
}