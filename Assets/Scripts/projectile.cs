using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    public Vector3 direction;
    public jiangshi myParent;
    public bool hitSomething;
    public float selfSpeed;
    // Update is called once per frame
    void Update()
    {
        if (!hitSomething)
        {
            if (myParent.isAggroed)
            {
                transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, selfSpeed * Time.deltaTime);
                transform.LookAt(transform.position + direction);
            }
            else
            {
                hitSomething = true;
                StartCoroutine(DestroySelf());
            }
        }
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            shiangji otherScript = other.GetComponent<shiangji>();
            if(!otherScript.isSneak)
            {
                if (!hitSomething)
                {
                    hitSomething = true;
                    StartCoroutine(DestroySelf());
                }

            }

        }
        else if(other.tag == "ground")
        {
            if (!hitSomething)
            {
                hitSomething = true;
                StartCoroutine(DestroySelf());
            }
        }
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(transform.gameObject);
    }


}
