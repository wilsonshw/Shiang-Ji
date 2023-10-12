using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pause : MonoBehaviour
{
    public AudioSource audioSrc; //sfx
    public AudioClip pop;
    public AudioClip select;
    public CanvasGroup blackOut;
    public shiangji heroScript;
    public GameObject pauseMenu;
    public GameObject subOptions; //e.g. volume control
    public GameObject subQuickGuide; //quick guide

    public void PointerEnter()
    {
        audioSrc.PlayOneShot(pop);
    }

    public void ClickOptions()
    {
        if (!subOptions.activeSelf)
        {
            audioSrc.PlayOneShot(select);
            pauseMenu.SetActive(false);
            subOptions.SetActive(true);
            heroScript.pauseSubMenu = subOptions;
        }
    }

    public void ClickQuickGuide()
    {
        if(!subQuickGuide.activeSelf)
        {
            audioSrc.PlayOneShot(select);
            pauseMenu.SetActive(false);
            subQuickGuide.SetActive(true);
            heroScript.pauseSubMenu = subQuickGuide;
        }
    }

    public void ClickLevelSelect()
    {
        //audioSrc.PlayOneShot(select);
        Time.timeScale = 1;
        StartCoroutine(DoBlackoutToScene());
    }

    public void ClickRestart()
    {
        audioSrc.PlayOneShot(select);
        Time.timeScale = 1;       
        StartCoroutine(DoBlackoutToRestart());
    }

    public void ClickResume()
    {
        audioSrc.PlayOneShot(select);
        pauseMenu.SetActive(false);
        heroScript.escapeBack.SetActive(false);
        heroScript.escapeMenu.SetActive(true);
        Time.timeScale = 1;
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

    public IEnumerator DoBlackoutToRestart()
    {
        CanvasGroup cvGroup = blackOut.GetComponent<CanvasGroup>();
        while (cvGroup.alpha < 1)
        {
            cvGroup.alpha += Time.unscaledDeltaTime;
            yield return null;
        }
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
