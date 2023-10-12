using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightorb : MonoBehaviour
{
    public Transform myLight;
    public ParticleSystem afterLight;

    public IEnumerator GrowCo;

    public void InitLight()
    {
        myLight.gameObject.SetActive(true);
        myLight.localScale = new Vector3(1, 1, 1);

        if (GrowCo != null)
            GrowCo = null;
        GrowCo = GrowLight();
        StartCoroutine(GrowCo);
    }

    public IEnumerator GrowLight()
    {
        float scale = myLight.localScale.x;
        while(scale < 10)
        {
            scale += Time.unscaledDeltaTime * 50;
            if (scale > 10)
                scale = 10;
            myLight.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        myLight.gameObject.SetActive(false);
        afterLight.Play();
    }

}
