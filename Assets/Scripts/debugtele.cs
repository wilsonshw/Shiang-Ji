using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugtele : MonoBehaviour
{
    public Transform teleTo;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.transform.position = teleTo.position;
            other.GetComponent<shiangji>().isJumping = true;
        }
    }
}
