using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIntagibility : MonoBehaviour
{
    [SerializeField] private Collider2D playerCollider;
[SerializeField] private LayerMask intangivelMask; // objetos que ele pode atravessar

private LayerMask defaultLayer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            gameObject.layer = LayerMask.NameToLayer("Intangivel");
            StartCoroutine(DesativarIntangibilidade(10f));
            Debug.Log("Intangivel");
        }
    }

    private IEnumerator DesativarIntangibilidade(float duracao)
    {
        yield return new WaitForSeconds(duracao);
        gameObject.layer = LayerMask.NameToLayer("playerLayer");
    }

}
