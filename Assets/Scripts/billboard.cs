using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billboard : MonoBehaviour
{
    public Transform followThis;
    public float sideOffset;
    public float upOffset;

    private void LateUpdate()
    {
        Vector3 lookPos = transform.position - Camera.main.transform.forward;
        transform.LookAt(lookPos);

        transform.position = new Vector3(followThis.position.x, followThis.position.y + upOffset, followThis.position.z + sideOffset);
    }
}
