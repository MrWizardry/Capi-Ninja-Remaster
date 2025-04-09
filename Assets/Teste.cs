using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teste : MonoBehaviour
{
    public GameObject objectToFollow;

    // Update is called once per frame
    void Update()
    {
        transform.position = objectToFollow.transform.position;
    }
}
