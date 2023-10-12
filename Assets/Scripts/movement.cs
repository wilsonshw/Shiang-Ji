using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    public Transform moveThis;
    public Transform lastPos;

    Vector3 targetPos;

    // Start is called before the first frame update
    void Start()
    {
        targetPos = lastPos.position;
    }

    // Update is called once per frame
    void Update()
    {
        moveThis.position = Vector3.MoveTowards(moveThis.position, targetPos, 2 * Time.deltaTime);
    }
}
