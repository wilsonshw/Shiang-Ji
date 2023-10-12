using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "atk")
        {
            collectible otherScript = other.GetComponent<collectible>();
            if (otherScript.myName == "light")
                Debug.Log("hmmmm");
        }
    }
}
