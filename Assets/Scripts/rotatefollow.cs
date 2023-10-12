using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotatefollow : MonoBehaviour
{
    public float rotateSpeed;
    public float rotateDir;
    public bool letsRotate;

    public Transform followTarget;

    void FixedUpdate()
    {
        if(letsRotate)
            transform.Rotate(0, rotateSpeed * rotateDir, 0);
    }

    private void LateUpdate()
    {
        if (letsRotate)
        {
            if(followTarget)
                transform.position = new Vector3(followTarget.position.x, transform.position.y, followTarget.position.z);
        }
    }

}
