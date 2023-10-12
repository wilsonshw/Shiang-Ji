using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scanner : MonoBehaviour
{
    public bool dependsOnLos; //true means can hide behind tree to avoid them. false means cannot (e.g. jiangshi's have this false)
    //holding breath does not affect this, because seers can see you regardless
                        
    public jiangshi myScript;
    public shiangji targetScript;
    public bool objDetected; //only used for objects that depend on Los
    //Note: jiangshi's can smell you, so they do NOT depend on LOS. Once you're near enough, they aggro, whether they can see you or not.
    //Other enemies that can see, they depend on LOS. You can hide behind trees to avoid aggroing them.
    public bat myBat;

    RaycastHit targetHit;

    private void FixedUpdate()
    {
        if(dependsOnLos)
        {
            if (objDetected)
            {
                Vector3 dir = targetScript.transform.position + new Vector3(0,0.5f,0) - transform.position;
                //dir.y = 0;
                dir.Normalize();
                if (Physics.Raycast(transform.position, dir, out targetHit, Mathf.Infinity))
                {
                    if (targetHit.transform.tag == "Player")
                    {
                        if(myBat) //indicates this script belongs to bat scanner
                        {
                            if(!myBat.isStunned && !myBat.isKO)
                            {
                                if (myScript) //master of the bat
                                {
                                    if (!myScript.isKO && !myScript.isAggroed && !targetScript.isKO)
                                    {
                                        myScript.isAggroed = true;
                                        myScript.patrolMove = false;
                                        myScript.StopAllCoroutines();
                                        myScript.isHome = false;
                                        objDetected = false;
                                        myScript.aggroRegion.SetActive(false);
                                        if(!myScript.isNiangshi) //only non-boss bats increase aggro counter of player
                                            targetScript.aggroCounter++;
                                        if (targetScript.aggroCounter>0)
                                        {
                                            //targetScript.isMarked = true;
                                            targetScript.markedObj.SetActive(true);
                                        }

                                        targetScript.audioSrc.PlayOneShot(targetScript.batSound);
                                    }
                                }
                                else//no master, 'independent' bat
                                {
                                    if (!myBat.isKO && !myBat.isAggroed && !targetScript.isKO)
                                    {
                                        myBat.isAggroed = true;
                                        objDetected = false;
                                        targetScript.aggroCounter++;
                                        if (targetScript.aggroCounter>0)
                                        {
                                            //targetScript.isMarked = true;
                                            targetScript.markedObj.SetActive(true);
                                        }

                                        targetScript.audioSrc.PlayOneShot(targetScript.batSound);

                                    }
                                }
                                
                            }
                            else
                            {
                                objDetected = false;
                            }
                        }
                        else //this scanner belongs to jiangshi (redundant?)
                        {
                            if (!myScript.isKO && !myScript.isAggroed && !targetScript.isKO)
                            {
                                myScript.isAggroed = true;
                                myScript.patrolMove = false;
                                myScript.StopAllCoroutines();
                                myScript.isHome = false;
                                objDetected = false;
                                myScript.aggroRegion.SetActive(false);
                                targetScript.aggroCounter++;
                                if (targetScript.aggroCounter>0)
                                {
                                    //targetScript.isMarked = true;
                                    targetScript.markedObj.SetActive(true);
                                }
                            }
                        }
                      
                    }
                }

            }
        }  

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (dependsOnLos)
            {
                if(myBat) //indicates this scanner script belongs to bat
                {
                    if (!myBat.isStunned && !myBat.isKO && !targetScript.isKO)
                        objDetected = true;
                }
                else //could be redundant??
                {
                    objDetected = true; //redundant??
                }
                    
            }
            else if (!dependsOnLos) //jiangshi aggroes
            {
                if (!myScript.isKO && !myScript.isAggroed && !targetScript.isSneak && !targetScript.isKO)
                {
                    if (!myScript.isStunned)
                    {
                        myScript.isAggroed = true;
                        myScript.nextBatCd = myScript.nextBatTime;//for niangshi only
                        myScript.patrolMove = false;
                        myScript.StopAllCoroutines();
                        myScript.isHome = false;
                        myScript.aggroRegion.SetActive(false);
                        targetScript.aggroCounter++;
                        if (targetScript.aggroCounter>0)
                        {
                            //targetScript.isMarked = true;
                            targetScript.markedObj.SetActive(true);
                        }

                        if(myScript.isNiangshi)
                        {
                            myScript.myLight.gameObject.SetActive(true);
                            myScript.myLight.nextTeleCd = myScript.myLight.nextTeleTime;
                            Vector3 dir = myScript.targetScript.transform.position - myScript.bigMist.transform.position;
                            dir.y = 0;
                            myScript.bigMist.transform.LookAt(myScript.bigMist.transform.position + dir);
                            myScript.bigMist.rotateCd = myScript.bigMist.rotateTimer;
                            myScript.bigMist.letsRotate = false;
                            myScript.bigMist.gameObject.SetActive(true);
                            myScript.gateObj.SetActive(true);
                            myScript.gateParticles.Play();
                        }

                        if (myScript.isNiangshi)
                            targetScript.audioSrc.PlayOneShot(targetScript.roarGirl);
                        else
                            targetScript.audioSrc.PlayOneShot(targetScript.roarBoy);

                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            if (dependsOnLos)
            {
                objDetected = false;
                if (myBat)
                {
                    if(myBat.myMaster)
                    {
                        if (!myBat.isKO && !myBat.isStunned && !myBat.myMaster.isAggroed)
                            myBat.aggroRegion.SetActive(true);
                    }
                    else
                    {
                        if (!myBat.isKO && !myBat.isStunned)
                            myBat.aggroRegion.SetActive(true);

                        if (myBat.isAggroed)
                        {
                            if (targetScript.aggroCounter > 0)
                                targetScript.aggroCounter--;
                        }
                    }
                    
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            if (other.tag == "Player")
            {
                if (dependsOnLos)
                {
                    if (myBat) //indicates this scanner script belongs to bat
                    {
                        if (!myBat.isStunned && !myBat.isKO && !targetScript.isKO)
                            objDetected = true;
                    }
                    else //could be redundant??
                    {
                        objDetected = true; //redundant??
                    }

                }
                else if (!dependsOnLos) //jiangshi aggroes
                {
                    if (!myScript.isKO && !myScript.isAggroed && !targetScript.isSneak && !targetScript.isKO)
                    {
                        if(!myScript.isStunned)
                        {
                            Debug.Log("3");
                            myScript.isAggroed = true;
                            myScript.nextBatCd = myScript.nextBatTime;//for niangshi only
                            myScript.patrolMove = false;
                            myScript.StopAllCoroutines();
                            myScript.isHome = false;
                            myScript.aggroRegion.SetActive(false);
                            targetScript.aggroCounter++;
                            if (targetScript.aggroCounter>0)
                            {
                                //targetScript.isMarked = true;
                                targetScript.markedObj.SetActive(true);
                            }

                            if (myScript.isNiangshi)
                            {
                                myScript.myLight.gameObject.SetActive(true);
                                myScript.myLight.nextTeleCd = myScript.myLight.nextTeleTime;
                                Vector3 dir = myScript.targetScript.transform.position - myScript.bigMist.transform.position;
                                dir.y = 0;
                                myScript.bigMist.transform.LookAt(myScript.bigMist.transform.position + dir);
                                myScript.bigMist.rotateCd = myScript.bigMist.rotateTimer;
                                myScript.bigMist.letsRotate = false;
                                myScript.bigMist.gameObject.SetActive(true);
                                myScript.gateObj.SetActive(true);
                                myScript.gateParticles.Play();
                            }

                            if (myScript.isNiangshi)
                                targetScript.audioSrc.PlayOneShot(targetScript.roarGirl);
                            else
                                targetScript.audioSrc.PlayOneShot(targetScript.roarBoy);

                        }
                      
                    }
                }
            }
        }
     
    }
}
