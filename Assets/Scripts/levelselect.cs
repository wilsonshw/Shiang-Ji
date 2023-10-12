using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class levelselect : MonoBehaviour
{
    public AudioSource audioSrc; //sfx
    public AudioClip pop;
    public AudioClip select;
    public string myScene;
    public bool clickDisabled;
    //public GameObject myIcon;
    public GameObject myText;
    public CanvasGroup blackOut;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ClickMe()
    {
        if (!clickDisabled)
        {
            audioSrc.PlayOneShot(select);
            StartCoroutine(DoBlackoutToScene());
            clickDisabled = true;
        }
    }

    public void PointerEnter()
    {
        if(!clickDisabled)
        {
            audioSrc.PlayOneShot(pop);
            myText.SetActive(true);
        }
    }

    public void PointerExit()
    {
        if (!clickDisabled)
        {
            //myIcon.SetActive(false);
            myText.SetActive(false);
        }
    }

    public IEnumerator DoBlackoutToScene()
    {
        CanvasGroup cvGroup = blackOut.GetComponent<CanvasGroup>();
        while (cvGroup.alpha < 1)
        {
            cvGroup.alpha += Time.unscaledDeltaTime;
            yield return null;
        }
        SceneManager.LoadScene(myScene);
    }

    public void OnPause(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            if (!clickDisabled)
            {
                StartCoroutine(DoBlackoutToScene());
                clickDisabled = true;
            }
        }
    }
}
