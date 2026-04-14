using System;
using System.Collections;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(Rigidbody2D))]
public class Controller_Jaré : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 4f;

    /*[Header("Delay inicial")]
    public float tempoEspera = 3f;*/

    private Transform player;
    private Rigidbody2D rb;
    [SerializeField] private bool perseguindo = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {

        if (player == null || !perseguindo) return;

        if (transform.position.x < player.position.x)
        {
            rb.linearVelocity = new Vector2(velocidade, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("AttackPoint"))
        {
            col.gameObject.SetActive(false);
            StartCoroutine(ExecuteAttack());
        }
        if (col.CompareTag("Destructable")) ;
        {
            Destroy(col.gameObject);
        }
    }

    IEnumerator ExecuteAttack()
    {
        perseguindo = false;
        rb.linearVelocity = Vector2.zero;

        Debug.Log("Ataque executado!");

        yield return new WaitForSeconds(2f);
        perseguindo = true;
    }
}
