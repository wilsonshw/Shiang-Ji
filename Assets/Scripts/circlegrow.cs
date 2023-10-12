using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class circlegrow : MonoBehaviour
{
    public bool growX;
    public bool growY;
    public bool growZ;

    public float startScaleX;
    public float startScaleY;
    public float startScaleZ;

    public float growTime;
    public float growSpeed;

    public ParticleSystem myDust;
    public MeshRenderer myMesh;
    public Collider myCollider;

    // Start is called before the first frame update
    void Start()
    {
        /*myCollider.enabled = true;
        myMesh.enabled = true;
        transform.localScale = new Vector3(startScaleX, startScaleY, startScaleZ);
        StartCoroutine(GrowMe());*/
    }

    void OnEnable()
    {
        myCollider.enabled = true;
        myMesh.enabled = true;
        transform.localScale = new Vector3(startScaleX, startScaleY, startScaleZ);
        StartCoroutine(GrowMe());
    }

    IEnumerator GrowMe()
    {        
        float timeToGrow = growTime;
        float xScale = startScaleX;
        float yScale = startScaleY;
        float zScale = startScaleZ;

        while(timeToGrow>0)
        {
            timeToGrow -= Time.deltaTime;

            if (growX)
                xScale += Time.deltaTime * growSpeed;

            if (growY)
                yScale += Time.deltaTime * growSpeed;

            if (growZ)
                zScale += Time.deltaTime * growSpeed;

            transform.localScale = new Vector3(xScale, yScale, zScale);

            if (timeToGrow <= 0)
            {
                StartCoroutine(DestroySelf());
            }

            yield return null;
        }
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(0.05f);
        Destroy(transform.gameObject);
    }
}
