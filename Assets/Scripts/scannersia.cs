using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scannersia : MonoBehaviour
{
    public shiangji targetScript;
    public siavamp myScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!myScript.isKO && !myScript.isAggroed && !targetScript.isKO && !targetScript.isSneak)
            {
                myScript.isAggroed = true;
                myScript.aggroRegion.SetActive(false);
                targetScript.aggroCounter++;
                if (targetScript.aggroCounter > 0)
                {
                    //targetScript.isMarked = true;
                    targetScript.markedObj.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!myScript.isKO && !myScript.isAggroed && !targetScript.isKO && !targetScript.isSneak)
            {
                myScript.isAggroed = true;
                myScript.aggroRegion.SetActive(false);
                targetScript.aggroCounter++;
                if (targetScript.aggroCounter > 0)
                {
                    //targetScript.isMarked = true;
                    targetScript.markedObj.SetActive(true);
                }
            }
        }
    }
}
