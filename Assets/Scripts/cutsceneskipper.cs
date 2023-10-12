using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cutsceneskipper : MonoBehaviour
{
    public string myNextScene;
    public CanvasGroup blackOut;
    public GameObject timelineObj;
    // Start is called before the first frame update
    void Start()
    {
        timelineObj.SetActive(false);
        StartCoroutine(DoBlackoutToScene());
    }

    public IEnumerator DoBlackoutToScene()
    {
        CanvasGroup cvGroup = blackOut.GetComponent<CanvasGroup>();
        while (cvGroup.alpha < 1)
        {
            cvGroup.alpha += Time.unscaledDeltaTime;
            yield return null;
        }
        SceneManager.LoadScene(myNextScene);
    }
}
