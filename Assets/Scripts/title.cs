using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class title : MonoBehaviour
{
    public AudioSource audioSrc; //sfx
    public AudioClip pop;
    public AudioClip select;
    public CanvasGroup blackOut;
    public bool clickDisabled;
    public GameObject subOptions;
    public GameObject subClearData;
    public GameObject pauseMenu;
    public GameObject allDataClearedMsg;

    public string cutsceneName;

    private void Start()
    {
        clickDisabled = false;
    }

    public void PointerEnter()
    {
        audioSrc.PlayOneShot(pop);
    }

    public void ViewCutScene()
    {
        if (!clickDisabled)
        {
            audioSrc.PlayOneShot(select);
            allDataClearedMsg.SetActive(false);
            cutsceneName = "cutscenetitle";
            StartCoroutine(DoBlackoutToCutScene());
            clickDisabled = true;
        }
    }

    public void ClickStart()
    {
        if (!clickDisabled)
        {
           audioSrc.PlayOneShot(select);
           allDataClearedMsg.SetActive(false);
            if (PlayerPrefs.HasKey("cutsceneseen"))
                StartCoroutine(DoBlackoutToScene());//load normally
            else
            {
                cutsceneName = "cutscene";
                StartCoroutine(DoBlackoutToCutScene());//watch cutscene
                PlayerPrefs.SetInt("cutsceneseen", 1);
            }
           clickDisabled = true;
        }
    }

    public void ClickOptions()
    {
        if (!clickDisabled)
        {
            if (!subOptions.activeSelf)
            {
                audioSrc.PlayOneShot(select);
                allDataClearedMsg.SetActive(false);
                pauseMenu.SetActive(false);
                subOptions.SetActive(true);
            }
        }

    }

    public void ClickClearData()
    {
        if(!clickDisabled)
        {
            if(!subClearData.activeSelf)
            {
                audioSrc.PlayOneShot(select);
                allDataClearedMsg.SetActive(false);
                pauseMenu.SetActive(false);
                subClearData.SetActive(true);
            }
        }
    }

    public void ClearDataYes()
    {
        //audioSrc.PlayOneShot(select);
        PlayerPrefs.DeleteAll();
        ClearDataNo();
        allDataClearedMsg.SetActive(true);
    }

    public void ClearDataNo()
    {
        //audioSrc.PlayOneShot(select);
        subClearData.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void ClickQuit()
    {
        if (!clickDisabled)
        {
            audioSrc.PlayOneShot(select);
            allDataClearedMsg.SetActive(false);
            Application.Quit();
            clickDisabled = true;
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
        SceneManager.LoadScene("levelselect");
    }

    public IEnumerator DoBlackoutToCutScene()
    {
        CanvasGroup cvGroup = blackOut.GetComponent<CanvasGroup>();
        while (cvGroup.alpha < 1)
        {
            cvGroup.alpha += Time.unscaledDeltaTime;
            yield return null;
        }
        SceneManager.LoadScene(cutsceneName);
    }

    public void OnPause(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            if (!pauseMenu.activeSelf)
            {
                if (subOptions.activeSelf)
                {
                    //audioSrc.PlayOneShot(select);
                    subOptions.SetActive(false);
                    pauseMenu.SetActive(true);
                }
                else if(subClearData.activeSelf)
                {
                    //audioSrc.PlayOneShot(select);
                    ClearDataNo();
                }
            }
        }
    }
}
