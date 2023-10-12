using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class forceskipcut : MonoBehaviour
{
    public cutsceneskipper skipper;
    public GameObject timelineObj;

    public void OnPause(InputAction.CallbackContext value)
    {

        if (value.started)
        {
            if(timelineObj.activeSelf)
                skipper.gameObject.SetActive(true);
        }

    }


}
