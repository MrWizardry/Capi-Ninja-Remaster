using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEnemy : MonoBehaviour
{
    public float fireDamage = 1f;
    public GameObject fireParent;

    private Player_Life playerHealth;
    private bool fireActive = true;

    private void Awake()
    {
        fireActive = true;
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Life>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!fireActive) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            playerHealth.TakeDamage((int)fireDamage);
        }
    }

    public void DisableFireParent()
    {
        fireActive = false;
        if (fireParent != null)
            fireParent.SetActive(false);
    }



}
