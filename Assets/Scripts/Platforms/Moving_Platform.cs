using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving_Platform : MonoBehaviour
{
    public Transform pointA; // Starting point of the platform
    public Transform pointB; // Ending point of the platform
    public float speed = 2f; // Speed of the platform movement

    private Vector3 nextPoint;

    void Start()
    {
        nextPoint = pointB.position; // Set the initial target point to pointB
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextPoint, speed * Time.deltaTime);

        if(transform.position == nextPoint)
        {
            nextPoint = (nextPoint == pointA.position) ? pointB.position : pointA.position; // Switch target point
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.parent = transform; // Make the player a child of the platform
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.parent = null; // Remove the player from the platform
        }
    }
}
