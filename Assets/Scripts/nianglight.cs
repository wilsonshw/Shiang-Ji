using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nianglight : MonoBehaviour
{
    public float nextTeleCd;
    public float nextTeleTime;
    public Transform[] positions;
    public int posCounter;
    public ParticleSystem myParticles1;
    public ParticleSystem myParticles2;
    public IEnumerator DoTeleCo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (nextTeleCd >= 0)
            nextTeleCd -= Time.deltaTime;
        else
        {
            nextTeleCd = nextTeleTime;
            posCounter++;
            if (posCounter >= positions.Length)
                posCounter = 0;
            if (DoTeleCo != null)
                StopCoroutine(DoTeleCo);

            DoTeleCo = DoTele();
            StartCoroutine(DoTeleCo);
            myParticles1.Play(); //local particle
        }
    }

    public IEnumerator DoTele()
    {
        yield return new WaitForSeconds(0.2f);
        transform.position = positions[posCounter].position;
        myParticles2.transform.position = transform.position;
        myParticles2.Play(); //local particle
    }
}
