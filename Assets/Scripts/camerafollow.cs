using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerafollow : MonoBehaviour
{
    public Transform followThis;

    void FixedUpdate()
    {
        Vector3 smoothedPos;
        smoothedPos = Vector3.Lerp(transform.position, followThis.position, 5 * Time.deltaTime);

        transform.position = smoothedPos;
    }
}
