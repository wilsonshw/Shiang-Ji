using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class volumeslider : MonoBehaviour
{
    public string myName;
    public Slider mySlider;
    public AudioSource[] myAudioSrc;
    // Start is called before the first frame update
    void Start()
    {
        LoadPref();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0;i<myAudioSrc.Length;i++)
            myAudioSrc[i].volume = mySlider.value;
    }

    public void SavePref()
    {
        PlayerPrefs.SetFloat(myName, mySlider.value);
    }

    public void LoadPref()
    {
        if (PlayerPrefs.HasKey(myName))
        {
            mySlider.value = PlayerPrefs.GetFloat(myName);
            for (int i = 0; i < myAudioSrc.Length; i++)
                myAudioSrc[i].volume = mySlider.value;
        }
        else
        {
            mySlider.value = 0.5f;
            for (int i = 0; i < myAudioSrc.Length; i++)
                myAudioSrc[i].volume = mySlider.value;
        }
    }
}
