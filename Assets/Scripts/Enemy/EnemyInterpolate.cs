using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInterpolate : MonoBehaviour
{
    [SerializeField] private Transform a;
    [SerializeField] private Transform b;

    [SerializeField] float moveSpeed;
    private Transform current;
    private Transform target;
    private float sinTime;

    void Start()
    {
        current = a;
        target = b;
        transform.position = current.position;
    }
    void Update()
    {
        if(transform.position != target.position)
        {
            sinTime += Time.deltaTime * moveSpeed;
            sinTime = Mathf.Clamp(sinTime, 0, Mathf.PI);
            float t = evaluate(sinTime);
            transform.position = Vector3.Lerp(current.position, target.position, t);
        }

        swap();
    }

    public void swap()
    {
        if(transform.position != target.position)
        {
            return;
        }
        Transform t = current;
        current = target;
        target = t;
        sinTime = 0;
    }

    public float evaluate(float x)
    {
        return 0.5f * Mathf.Sin(x - Mathf.PI / 2F) + 0.5f;
    }
}
