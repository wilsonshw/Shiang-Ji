using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectible : MonoBehaviour
{
    public string myName;
    public Material myMat;
    public Material grayMat;
    public MeshRenderer myMesh;
    public GameObject myShine;
    public Collider myColl;

    public bool causePause;
    public GameObject myOtherParticle;

    public int myId; //for end of level trigger

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(myName=="charm")
            {
                myMesh.material = grayMat;
                myShine.SetActive(false);
                myColl.enabled = false;
                StartCoroutine(ResetMyself());
            }
            else if(myName == "niangcharm")
            {
                transform.gameObject.SetActive(false);
            }
            else if(myName == "info")
            {
                
                myShine.SetActive(true);
                if(causePause)
                {
                    shiangji otherScript = other.GetComponent<shiangji>();
                    otherScript.infoActive = true;
                    otherScript.infoObj = myShine;
                    otherScript.escapeBack.SetActive(true);
                    otherScript.escapeMenu.SetActive(false);
                    otherScript.ResetAnims();
                    otherScript.audioSteps.SetActive(false);
                    otherScript.audioJump.SetActive(false);
                    otherScript.FalsifySneak();
                    Time.timeScale = 0;                   
                    myColl.enabled = false;
                }
               
            }
        }
        else if(other.tag == "atk")
        {
            collectible otherScript = other.GetComponent<collectible>();
            if (myName == "mist")
            {
                if (otherScript.myName == "light")
                {
                    myShine.SetActive(false); //the atkobject of the mist
                    myColl.enabled = false;
                    myOtherParticle.SetActive(true); //blown away particles
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            if (myName == "info")
            {
                myShine.SetActive(false);
                myColl.enabled = false;
            }
        }
    }

    IEnumerator ResetMyself()
    {
        yield return new WaitForSeconds(3);
        myMesh.material = myMat;
        myShine.SetActive(true);
        myColl.enabled = true;
    }
}
