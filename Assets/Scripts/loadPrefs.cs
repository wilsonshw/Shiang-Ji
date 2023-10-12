using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loadPrefs : MonoBehaviour
{
    public AudioSource[] bgSource;
    public AudioSource[] sfxSource;

    public GameObject[] mapObjects;
    // Start is called before the first frame update
    void Start()
    {
        LoadMyPrefs();
    }

    void LoadMyPrefs()
    {
        if(bgSource.Length>0)
        {
            if (PlayerPrefs.HasKey("bgvolume"))
            {
                for (int i = 0; i < bgSource.Length; i++)
                    bgSource[i].volume = PlayerPrefs.GetFloat("bgvolume");
            }
        }
       
        if(sfxSource.Length>0)
        {
            if (PlayerPrefs.HasKey("sfxvolume"))
            {
                for (int i = 0; i < sfxSource.Length; i++)
                    sfxSource[i].volume = PlayerPrefs.GetFloat("sfxvolume");
            }
        }
      

        if(mapObjects.Length>0)
        {
            if(PlayerPrefs.HasKey("mapcompleted"))
            {
                int mapCompleted = PlayerPrefs.GetInt("mapcompleted"); //if completed 0, means level 0 and level 1 can be set to active
                for(int i = 0;i<mapObjects.Length;i++)
                {
                    mapObjects[i].SetActive(false);
                    if (i <= mapCompleted + 1) //if completed 0, means level 0 and level 1 can be set to active
                        mapObjects[i].SetActive(true);
                }
            }
            else //fresh game
            {
                for (int i = 0; i < mapObjects.Length; i++)
                {
                    mapObjects[i].SetActive(false);
                }
                mapObjects[0].SetActive(true);
            }
        }

    }

}
