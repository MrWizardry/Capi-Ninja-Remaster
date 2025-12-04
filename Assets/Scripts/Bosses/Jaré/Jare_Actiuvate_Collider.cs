using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jare_Actiuvate_Collider : MonoBehaviour
{   
    public Collider2D raboObject;

    void Start()
    {  
        raboObject.enabled = false;
    }

    void ActivateCollider()
    {
        raboObject.enabled = true;
    }

    void DeactivateCollider()
    {
        raboObject.enabled = false;
    }
}
