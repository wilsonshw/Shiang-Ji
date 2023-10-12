using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bat : MonoBehaviour
{
    public bool isNiangshi; //a summon by niangshi
    public Animator selfAnim;
    public SkinnedMeshRenderer[] myMesh;

    public jiangshi myMaster;
    public rotatefollow myRotator;
    public shiangji targetScript;

    public Quaternion homeRot;
    public Vector3 homePos;

    public bool isKO;
    public bool isGrounded;
    public bool isStunned;
    public bool waitToPoofHome;

    public GameObject aggroRegion;
    public SphereCollider myColl;

    RaycastHit groundHit;

    public ParticleSystem koSmoke;
    public ParticleSystem homeSmoke;
    public ParticleSystem homeSmoke1;
    public ParticleSystem stunStar;

    public GameObject dropPrefab;

    public bool isAggroed; //only applies to 'independent' bats with no master
    public float stunDuration;

    // Start is called before the first frame update
    void Start()
    {
        homePos = transform.localPosition;
        homeRot = transform.localRotation;
    }

    private void Update()
    {
        if (!isKO)
        {
            if (myMaster) //bat with master
            {
                if(myMaster.isKO)
                    DoKOStuff();
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(!isKO && !isStunned && !waitToPoofHome)
        {
            if(myMaster) //bat with master
            {
                if (myMaster.isAggroed)
                {
                    if(myRotator)
                    {
                        if (myRotator.letsRotate)
                            myRotator.letsRotate = false;
                    }

                    Vector3 smoothedPos;
                    if (!isNiangshi)
                    {                       
                        Vector3 targetPos = myMaster.transform.position + Vector3.up * 0.7f;
                        smoothedPos = Vector3.Lerp(transform.position, targetPos, 3 * Time.deltaTime);
                    }
                    else//belongs to Niangshi
                    {
                        Vector3 targetPos = targetScript.transform.position - targetScript.transform.forward + Vector3.up * 0.7f;
                        smoothedPos = Vector3.Lerp(transform.position, targetPos, 3 * Time.deltaTime);
                    }                    

                    transform.position = smoothedPos;
                    transform.LookAt(targetScript.transform.position);

                    if (selfAnim.GetFloat("flyfast") != 1)
                    {
                        ResetAnims();
                        selfAnim.SetFloat("flyfast", 1);
                    }

                    if (aggroRegion.activeSelf)
                        aggroRegion.SetActive(false);
                }
                else
                {
                    if(!isNiangshi)
                    {
                        if (myRotator)
                        {
                            if (!myRotator.letsRotate)
                                myRotator.letsRotate = true;
                        }

                        if (transform.localPosition != homePos)
                        {
                            for (int i = 0; i < myMesh.Length; i++)
                                myMesh[i].enabled = false;
                            waitToPoofHome = true;
                            myColl.enabled = false;
                            //transform.localPosition = homePos;
                            homeSmoke.Play();
                            StartCoroutine(TeleportHome());
                        }

                        if (transform.localRotation != homeRot)
                            transform.localRotation = homeRot;


                        if (selfAnim.GetFloat("fly") != 1)
                        {
                            ResetAnims();
                            selfAnim.SetFloat("fly", 1);
                        }

                        if (!aggroRegion.activeSelf)
                            aggroRegion.SetActive(true);
                    }
                    /*else if(isNiangshi)
                    {
                        DoKOStuff();
                    }*/
                   
                }
            }
            else //independent bat
            {
                if (isAggroed)
                {
                    if(myRotator)
                    {
                        if (myRotator.letsRotate)
                            myRotator.letsRotate = false;
                    }

                    Vector3 smoothedPos;
                    Vector3 targetPos = targetScript.transform.position - targetScript.transform.forward + Vector3.up * 0.7f;
                    smoothedPos = Vector3.Lerp(transform.position, targetPos, 3 * Time.deltaTime);

                    transform.position = smoothedPos;
                    transform.LookAt(targetScript.transform.position);

                    if (selfAnim.GetFloat("flyfast") != 1)
                    {
                        ResetAnims();
                        selfAnim.SetFloat("flyfast", 1);
                    }

                    if (aggroRegion.activeSelf)
                        aggroRegion.SetActive(false);

                    if(targetScript.isKO)
                    {
                        isAggroed = false;
                        if (targetScript.aggroCounter > 0)
                            targetScript.aggroCounter--;
                    }
                }
                else
                {
                    if(myRotator)
                    {
                        if (!myRotator.letsRotate)
                            myRotator.letsRotate = true;
                    }

                    if (transform.localPosition != homePos)
                    {
                        for (int i = 0; i < myMesh.Length; i++)
                            myMesh[i].enabled = false;
                        waitToPoofHome = true;
                        myColl.enabled = false;
                        //transform.localPosition = homePos;
                        homeSmoke.Play();
                        StartCoroutine(TeleportHome());
                    }

                    if (transform.localRotation != homeRot)
                        transform.localRotation = homeRot;


                    if (selfAnim.GetFloat("fly") != 1)
                    {
                        ResetAnims();
                        selfAnim.SetFloat("fly", 1);
                    }

                    if (!waitToPoofHome)
                    {
                        if (!aggroRegion.activeSelf)
                            aggroRegion.SetActive(true);
                    }

                }
            }
           
        }

        if(!isKO && !waitToPoofHome)
        {
            if(isStunned)
            {
                if (!isGrounded)
                {
                    if (Physics.Raycast(transform.TransformPoint(myColl.center), Vector3.down, out groundHit, transform.localScale.x * myColl.radius* 1.1f))
                    {
                        if (groundHit.transform.tag == "ground")
                        {
                            isGrounded = true;
                            ResetAnims();
                            selfAnim.SetFloat("stunend", 1);
                            StartCoroutine(EndStun());
                        }
                        else
                        {
                            transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.down, 2 * Time.deltaTime);
                        }
                    }
                    else
                    {
                        transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.down, 2 * Time.deltaTime);
                    }
                }


            }
        }      

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "atk")
        {
            collectible otherScript = other.GetComponent<collectible>();
            if(otherScript.myName == "jumpatk")
            {
                if (isStunned) //ShiangJi can only step on thi sif it's stunned
                {
                    /*if (objToActivate) //key
                    {
                        objToActivate.transform.position = transform.position + Vector3.up * 2f;
                        objToActivate.SetActive(true);
                    }*/
                    DoKOStuff();

                }
            }
            else if(otherScript.myName == "light")
            {
                if(!waitToPoofHome && !isKO && !isStunned)
                {
                    if (!myMaster)
                    {
                        if (isAggroed) //only applies to independent bat
                        {
                            isAggroed = false;
                            if (targetScript.aggroCounter > 0)
                                targetScript.aggroCounter--;
                        }
                    }
                    Vector3 dir = targetScript.transform.position - transform.position;
                    dir.y = 0;
                    if(myRotator)
                        myRotator.letsRotate = false;
                    transform.LookAt(transform.position + dir);
                    isStunned = true;
                    stunStar.Play();
                    ResetAnims();
                    selfAnim.SetFloat("stunstart", 1);

                    if (myMaster)
                    {
                        myMaster.isStunned = true;
                        if(myMaster.isNiangshi)
                        {
                            if(myMaster.targetScript.charmCounter == 0)
                            {
                                myMaster.myCharm.transform.position = myMaster.bigMist.transform.position + myMaster.bigMist.transform.forward * 2 + myMaster.bigMist.transform.right * myMaster.bigMist.rotateDir * 2;
                                myMaster.myCharmParticles.transform.position = myMaster.myCharm.transform.position;
                                myMaster.myCharm.gameObject.SetActive(true);
                                myMaster.myCharmParticles.Play();
                            }                         
                        }
                       
                        if (myMaster.EndStunCo != null)
                            myMaster.StopCoroutine(myMaster.EndStunCo);
                        myMaster.EndStunCo = myMaster.EndStun();
                        myMaster.StartCoroutine(myMaster.EndStunCo);
                        myMaster.isAtk = false;
                        if (myMaster.isAggroed)
                        {             
                            if(targetScript.aggroCounter>0)
                                targetScript.aggroCounter--;
                            myMaster.aggroRegion.SetActive(false);
                            myMaster.selfNav.SetDestination(myMaster.transform.position);

                            if (!isNiangshi)
                            {
                                myMaster.isAggroed = false;
                                myMaster.patrolMove = true;
                            }
                        }
                        myMaster.stunStar.Play();
                        myMaster.ResetAnims();
                        myMaster.selfAnim.SetFloat("idle", 1);
                        

                        /*if (isNiangshi)
                        {
                            targetScript.isMarked = false;
                            targetScript.isMarkedBoss = false;
                            targetScript.markedCd = targetScript.markedTime;
                            targetScript.markedObj.SetActive(false);
                        }*/

                    }
                }
               
            }
        }
    }

    public void DoKOStuff()
    {
        if (dropPrefab)
        {
            var instObj = Instantiate(dropPrefab, transform.position + Vector3.up * 2f, Quaternion.identity) as GameObject;
        }

        targetScript.audioSrc.PlayOneShot(targetScript.poof);
        isKO = true;
        StopAllCoroutines();
        isStunned = false;
        stunStar.Stop();
        for (int i = 0; i < myMesh.Length; i++)
            myMesh[i].enabled = false;
        myColl.enabled = false;
        if(myRotator)
            myRotator.letsRotate = false;
        aggroRegion.SetActive(false);
        koSmoke.Play();

        if(!myMaster)
        {
            if(isAggroed)
            {
                isAggroed = false;//remember, this only applies to independent bat
                if (targetScript.aggroCounter > 0)
                    targetScript.aggroCounter--;
            }        
        }
        else
        {
            myMaster.myBat = null;
            myMaster.selfNav.speed = myMaster.selfSpeed;
        }

        if(isNiangshi)
        {
            StartCoroutine(DestroyMyself());
        }
    }

    public void ResetAnims()
    {
        selfAnim.SetFloat("fly", 0);
        selfAnim.SetFloat("flyfast", 0);
        selfAnim.SetFloat("stunstart", 0);
        selfAnim.SetFloat("stunend", 0);
    }

    IEnumerator EndStun()
    {
        yield return new WaitForSeconds(stunDuration);
        /*if (isNiangshi)
        {
            targetScript.isMarked = true;
            targetScript.markedObj.SetActive(true);
        }*/
        isStunned = false;
        isGrounded = false;
        stunStar.Stop();
    }

    IEnumerator TeleportHome()
    {
        yield return new WaitForSeconds(0.2f);
        waitToPoofHome = false;
        for (int i = 0; i < myMesh.Length; i++)
            myMesh[i].enabled = true;
        myColl.enabled = true;
        transform.localPosition = homePos;
        homeSmoke1.Play();
    }

    IEnumerator DestroyMyself()
    {
        yield return new WaitForSeconds(1);
        Destroy(transform.gameObject);
    }
}
