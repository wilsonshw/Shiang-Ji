using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour
{
    public float rotateSpeed;
    public float rotateDir;
    public bool letsRotate;
    public float rotateCd;
    public float rotateTimer;
    // Update is called once per frame
    void Update()
    {
        if(!letsRotate)
        {
            if (rotateCd >= 0)
                rotateCd -= Time.deltaTime;
            else
            {
                letsRotate = true;
            }
        }

        if(letsRotate)
            transform.Rotate(0, rotateSpeed * rotateDir, 0);
    }
}
