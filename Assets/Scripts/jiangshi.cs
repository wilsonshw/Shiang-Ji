using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.AI;

public class jiangshi : MonoBehaviour
{
    public bool isInvincible;
    public float invincibleTime;
    public bool isNiangshi;
    public bool modeSee;
    public int selfHP;
    public int maxHP;
    public rotator bigMist;
    public GameObject shockwavePrefab;
    public GameObject batPrefab;
    public GameObject gateObj;
    public GameObject projectilePrefab;
    public Transform projectilePos;
    public ParticleSystem gateParticles;
    public float nextBatCd;
    public float nextBatTime;
    public RuntimeAnimatorController contSeeMode; //normal mode
    public RuntimeAnimatorController contBlindMode; //cannot see player
    public nianglight myLight;
    public lightorb myLightAtk;
    public collectible myCharm;
    public ParticleSystem myCharmParticles;

    IEnumerator BlinkCo;
    IEnumerator BlinkRevertCo;
    public IEnumerator EndStunCo;

    public Transform patrolPoint1;
    public GameObject dropPrefab; //e.g. tooth, eye

    public NavMeshAgent selfNav;
    public NavMeshPath navPath;

    public bool isAggroed;
    public bool isAtk; //attack animation happening
    public bool isHome; //currently at home, can patrol
    public bool isKO;
    public bool isStunned; //for the vamp-bat pair
    public bool patrolMove;

    public Collider myColl;
    public SkinnedMeshRenderer myMesh;
    public Animator selfAnim;

    public shiangji targetScript;

    public float batSpeed;   
    public float nextAtk;
    public float nextAtkCd;
    public float selfDist;
    public float selfSpeed;
    public float stunDuration;

    public Vector3 patrolPos1;
    public Vector3 patrolPos2;
    public Vector3 targetCalc; //calculate target pos
    public Vector3 targetOffset; //offset from target position
    public Vector3 targetPos;

    public ParticleSystem koSmoke;
    public ParticleSystem breathMist; //sia only
    public ParticleSystem stunStar; //pair only
    public GameObject perpetualMist;
    public GameObject clearMist;

    public VisualEffect vfxFang;

    Quaternion selfRot;
    Vector3 homePos;
    Vector3 lookPos;
    IEnumerator PatrolWaitCo;

    public GameObject aggroRegion;//the red circle

    public bat myBat;
    public bool canSetDest;
    public float setDestTimer;

    // Start is called before the first frame update
    void Start()
    {
        if (isNiangshi)
            selfHP = maxHP;
        navPath = new NavMeshPath();
        setDestTimer = 0;
        canSetDest = false;
        selfNav.speed = selfSpeed;
        if (myBat)
            selfNav.speed = batSpeed; //faster
        nextAtkCd = nextAtk;
        homePos = transform.position;
        patrolPos1 = patrolPoint1.position;
        patrolPos2 = homePos;
        isHome = true;
        patrolMove = true; //start with patroling
        targetCalc = patrolPos1;
        ResetAnims();
        if(!isNiangshi)
            selfAnim.SetFloat("run", 1); //start with patroling, so run animation
        else
        {
            selfAnim.SetFloat("idle", 1);
        }
        //targetCalc = transform.position;
        //selfNav.SetDestination(homePos);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isKO)
        {
            if(!canSetDest)
            {
                if (setDestTimer >= 0)
                    setDestTimer -= Time.deltaTime;
                else
                {
                    canSetDest = true;
                    StartCoroutine(FalsifySetDest());
                }
            }

            if(isNiangshi)
            {
                if (isInvincible)
                {
                    if (invincibleTime > 0)
                        invincibleTime -= Time.deltaTime;
                    else
                    {
                        invincibleTime = 0;
                        isInvincible = false;
                        if (BlinkCo != null)
                            StopCoroutine(BlinkCo);
                        if (BlinkRevertCo != null)
                            StopCoroutine(BlinkRevertCo);
                        myMesh.enabled = true;
                    }
                }
            }
           

            if (isAggroed)
            {
                if (!isAtk && !isStunned)
                {
                    AggroMoveStuff();
                }               

                if(myBat)
                {
                    if(!myBat.isKO) //with bat around, ignore sneak
                    {
                        if (targetScript.isKO)
                        {
                            if (breathMist) //sia only
                                StopBreath();
                            isAggroed = false;
                            isStunned = false;
                            //only for niang
                            isInvincible = false;
                            if (BlinkCo != null)
                                StopCoroutine(BlinkCo);
                            if (BlinkRevertCo != null)
                                StopCoroutine(BlinkRevertCo);
                            myMesh.enabled = true;
                            //end for niang
                            if (stunStar)
                                stunStar.Stop();
                            StopAllCoroutines();
                            isHome = false;
                            patrolMove = true;
                            if(targetScript.aggroCounter > 0)
                                targetScript.aggroCounter--;

                            if(isNiangshi)
                            {
                                selfHP = maxHP;
                                myBat.DoKOStuff();
                                myBat = null;
                                myLight.myParticles2.Play();
                                if (myLight.DoTeleCo != null)
                                    myLight.StopCoroutine(myLight.DoTeleCo);
                                if (myLightAtk.GrowCo != null)
                                    myLightAtk.StopCoroutine(myLightAtk.GrowCo);
                                myLightAtk.myLight.gameObject.SetActive(false);
                                myLight.gameObject.SetActive(false);                               
                                bigMist.letsRotate = false;
                                bigMist.gameObject.SetActive(false);
                                myCharm.gameObject.SetActive(false);
                                gateObj.SetActive(false);
                                gateParticles.Play();
                            }
                        }

                        if (selfNav.speed != batSpeed)
                            selfNav.speed = batSpeed;
                    }
                    else
                    {
                        myBat = null;
                    }

                }
                else
                {
                    if(!isNiangshi)
                    {
                        if (targetScript.isSneak || targetScript.isKO) //shiangji is sneaking
                        {
                            if (breathMist) //sia only
                                StopBreath();
                            isAggroed = false;
                            if(targetScript.isKO)
                            {
                                isStunned = false;
                                if(stunStar)
                                    stunStar.Stop();
                            }
                            //patrolMove = true;
                            StopAllCoroutines();
                            if(targetScript.aggroCounter > 0)
                                targetScript.aggroCounter--;
                        }
                    }
                    else if(isNiangshi)
                    {
                        if(!targetScript.isKO)
                        {
                            if (targetScript.isSneak) //shiangji is sneaking
                            {
                                if (targetScript.aggroCounter > 0 && !isStunned)
                                {
                                    if (targetScript.aggroCounter > 0)
                                        targetScript.aggroCounter--;
                                    targetScript.markedObj.SetActive(false);
                                }
                            }
                            else
                            {
                                if (targetScript.aggroCounter <= 0 && !isStunned)
                                {
                                    //targetScript.isMarked = true;
                                    targetScript.markedObj.SetActive(true);
                                    targetScript.aggroCounter++;
                                }
                            }
                        }
                        else if(targetScript.isKO)
                        {
                            Debug.Log("1");
                            selfHP = maxHP;
                            isAggroed = false;
                            isAtk = false;
                            isHome = false;
                            patrolMove = true;
                            isStunned = false;
                            //only for niang
                            isInvincible = false;
                            if (BlinkCo != null)
                                StopCoroutine(BlinkCo);
                            if (BlinkRevertCo != null)
                                StopCoroutine(BlinkRevertCo);
                            myMesh.enabled = true;
                            if (myBat)
                            {
                                myBat.DoKOStuff();
                                myBat = null;
                            }
                            myLight.myParticles2.Play();
                            if (myLight.DoTeleCo != null)
                                myLight.StopCoroutine(myLight.DoTeleCo);
                            if (myLightAtk.GrowCo != null)
                                myLightAtk.StopCoroutine(myLightAtk.GrowCo);
                            myLightAtk.myLight.gameObject.SetActive(false);
                            myLight.gameObject.SetActive(false);                           
                            bigMist.letsRotate = false;
                            bigMist.gameObject.SetActive(false);
                            myCharm.gameObject.SetActive(false);
                            gateObj.SetActive(false);
                            gateParticles.Play();
                            //end niang
                            if (stunStar)
                                stunStar.Stop();
                            StopAllCoroutines();
                            if(targetScript.aggroCounter > 0)
                                targetScript.aggroCounter--;
                        }
                        
                    }                  
                } 
                
                if(isNiangshi)
                {
                    if(!myBat && !isStunned)
                    {
                        if (nextBatCd >= 0)
                            nextBatCd -= Time.deltaTime;
                        else
                        {
                            SummonBat();
                            nextBatCd = nextBatTime;
                        }
                    }
                }
            }
            else if (!isAggroed && !isStunned)
            {
                if(!isHome)
                    MoveHome();
                else if (isHome)
                    DoPatrol();
            }

            if (!isStunned)
            {
                if (!isNiangshi || (isNiangshi && myBat) || (isNiangshi && !myBat && !targetScript.isSneak)) //normal jiangshi, or niangshi with bat, OR niangshi without bat and target not sneaking
                    NormalSlerpStuff();
            }

            if(isNiangshi)
            {
                if(myBat) //has bat
                {
                    if (!modeSee && !isAtk)
                    {
                        modeSee = true;
                        selfAnim.runtimeAnimatorController = contSeeMode;
                    }
                }
                else //no bat
                {
                    if(!targetScript.isSneak)
                    {
                        if (!modeSee && !isAtk)
                        {
                            modeSee = true;
                            selfAnim.runtimeAnimatorController = contSeeMode;
                        }
                    }
                    else
                    {
                        if(modeSee && !isAtk)
                        {
                            modeSee = false;
                            selfAnim.runtimeAnimatorController = contBlindMode;
                        }
                    }
                }
            }
        }

    }

    public void NormalSlerpStuff()
    {
        lookPos = targetCalc - transform.position;
        lookPos.y = 0;
        if (lookPos != Vector3.zero)
            selfRot = Quaternion.LookRotation(lookPos);
        else
            selfRot = Quaternion.LookRotation(transform.forward);

        transform.rotation = Quaternion.Slerp(transform.rotation, selfRot, 20 * Time.deltaTime);
    }

    public void AggroMoveStuff()
    {
        if (nextAtkCd >= 0)
            nextAtkCd -= Time.deltaTime;

        targetCalc = new Vector3(targetScript.transform.position.x, transform.position.y, targetScript.transform.position.z); //actual pos of target (disregard y)
        targetOffset = transform.position - targetCalc; //position of self relative to target, reuse targetcalc
        targetOffset.Normalize();
        targetPos = targetCalc + targetOffset * selfDist;
        if (canSetDest)
            selfNav.SetDestination(targetPos);

        //transform.position = Vector3.MoveTowards(transform.position, targetPos, runSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) <= 0.1f || Vector3.Distance(transform.position,targetCalc) < selfDist)
        {
            if(!isAtk)
            {
                if (nextAtkCd >= 0)
                {
                    ResetAnims();
                    if (canSetDest)
                    {
                        selfNav.SetDestination(transform.position);
                        selfAnim.SetFloat("idle", 1);
                    }
                }
                else
                {
                    isAtk = true;
                    ResetAnims();
                    selfNav.SetDestination(transform.position);
                    selfAnim.SetFloat("attack", 1);

                    nextAtkCd = nextAtk;
                }
               
            }         
        }
        else
        {
            if (selfAnim.GetFloat("run") != 1)
            {
                ResetAnims();
                selfAnim.SetFloat("run", 1);
            }
        }
       
    }

    public void MoveHome()
    {
        if (!aggroRegion.activeSelf)
            aggroRegion.SetActive(true);
        isAtk = false;
        targetCalc = homePos;
        if (canSetDest)
            selfNav.SetDestination(homePos);

        //transform.position = Vector3.MoveTowards(transform.position, homePos, runSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.position,homePos) <= 0.1f)
        {
            if (selfAnim.GetFloat("idle") != 1)
            {
                isHome = true;
                patrolMove = true;
                ResetAnims();
                selfNav.SetDestination(transform.position);
                selfAnim.SetFloat("idle", 1);
            }
        }
        else
        {
            if (selfAnim.GetFloat("run") != 1)
            {
                ResetAnims();
                selfAnim.SetFloat("run", 1);
            }
        }
    }

    public void DoPatrol()
    {
        if(!isNiangshi)
        {
            if (patrolMove)
            {
                if (canSetDest)
                    selfNav.SetDestination(targetCalc);

                lookPos = targetCalc - transform.position;
                lookPos.y = 0;
                if (lookPos != Vector3.zero)
                    selfRot = Quaternion.LookRotation(lookPos);
                else
                    selfRot = Quaternion.LookRotation(transform.forward);

                transform.rotation = Quaternion.Slerp(transform.rotation, selfRot, 20 * Time.deltaTime);

                if (Vector3.Distance(transform.position, targetCalc) <= 0.1f)
                {
                    patrolMove = false;

                    ResetAnims();
                    selfNav.SetDestination(transform.position);
                    selfAnim.SetFloat("idle", 1);

                    if (PatrolWaitCo != null)
                        StopCoroutine(PatrolWaitCo);

                    PatrolWaitCo = PatrolWait();
                    StartCoroutine(PatrolWaitCo);
                }
            }

        }

    }

    IEnumerator PatrolWait()
    {
        yield return new WaitForSeconds(Random.Range(1, 3));
        if (targetCalc == patrolPos1)
            targetCalc = patrolPos2;
        else if (targetCalc == patrolPos2)
            targetCalc = patrolPos1;
        patrolMove = true;
        ResetAnims();
        if(!isStunned)
            selfAnim.SetFloat("run", 1);

    }

    public void PlayVfxFang()
    {
        vfxFang.Play();
    }

    public void FalsifyAtk()
    {
        isAtk = false;
        ResetAnims();
        selfNav.SetDestination(transform.position);
        selfAnim.SetFloat("idle", 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "atk")
        {
            collectible otherSCript = other.GetComponent<collectible>();
            if(otherSCript.myName == "player")
            {
                if(!isNiangshi)
                {
                    if (isAggroed)
                    {
                        isAggroed = false;
                        if (targetScript.aggroCounter > 0)
                            targetScript.aggroCounter--;
                    }
                    isStunned = false;
                    if(stunStar)
                        stunStar.Stop();
                    patrolMove = false;
                    isHome = false;
                    StopAllCoroutines();
                    aggroRegion.SetActive(false);
                    targetScript.audioSrc.PlayOneShot(targetScript.poof);
                    ResetAnims();
                    isKO = true;
                    myMesh.enabled = false;
                    myColl.enabled = false;
                    koSmoke.Play();

                    if (breathMist) //sia only
                        StopBreath();

                    if (perpetualMist)//sia only, and niang
                        perpetualMist.SetActive(false);

                    if (clearMist)//sia only, and niang
                        clearMist.SetActive(true);

                    if (dropPrefab)
                    {
                        var instObj = Instantiate(dropPrefab, transform.position + Vector3.up * 0.8f, Quaternion.identity) as GameObject;
                    }
                }
                else if(isNiangshi)
                {
                    if(!isInvincible)
                    {
                        selfHP--;
                        if (myBat) //dies as long as niang is hit
                            myBat.DoKOStuff();

                        if (selfHP > 0)
                        {
                            if (!isAggroed)
                            {
                                targetScript.audioSrc.PlayOneShot(targetScript.roarGirl);
                                isAggroed = true;
                                nextBatCd = nextBatTime;//for niangshi only
                                patrolMove = false;
                                StopAllCoroutines();
                                isHome = false;
                                aggroRegion.SetActive(false);
                                targetScript.aggroCounter++;
                                if (targetScript.aggroCounter > 0)
                                {
                                    //targetScript.isMarked = true;
                                    targetScript.markedObj.SetActive(true);
                                }

                                myLight.gameObject.SetActive(true);
                                myLight.nextTeleCd = myLight.nextTeleTime;
                                Vector3 dir = targetScript.transform.position - bigMist.transform.position;
                                dir.y = 0;
                                bigMist.transform.LookAt(bigMist.transform.position + dir);
                                bigMist.rotateCd = bigMist.rotateTimer;
                                bigMist.letsRotate = false;
                                bigMist.gameObject.SetActive(true);
                                gateObj.SetActive(true);
                                gateParticles.Play();
                            }

                            bigMist.rotateDir *= -1;//change direction
                            targetScript.audioSrc.PlayOneShot(targetScript.pop1);
                            DoBlink();
                            if (EndStunCo != null)
                                StopCoroutine(EndStunCo);
                            isAtk = false;
                            stunStar.Stop();
                            isStunned = false;
                            if (!isNiangshi)
                                aggroRegion.SetActive(true);
                            ResetAnims();
                            nextAtkCd = nextAtk;

                            if (targetScript.aggroCounter <= 0)
                            {
                                targetScript.aggroCounter++;
                                //targetScript.isMarked = true;
                            }
                        }
                        else if (selfHP <= 0)
                        {
                            selfHP = 0;
                            if (isAggroed)
                            {
                                isAggroed = false;
                                if (targetScript.aggroCounter > 0)
                                    targetScript.aggroCounter--;
                            }
                            myLight.myParticles2.Play();
                            if (myLight.DoTeleCo != null)
                                myLight.StopCoroutine(myLight.DoTeleCo);
                            if (myLightAtk.GrowCo != null)
                                myLightAtk.StopCoroutine(myLightAtk.GrowCo);
                            myLightAtk.myLight.gameObject.SetActive(false);
                            myLight.gameObject.SetActive(false);
                            myCharm.gameObject.SetActive(false);
                            bigMist.letsRotate = false;
                            bigMist.gameObject.SetActive(false);
                            gateObj.SetActive(false);
                            gateParticles.Play();
                            isStunned = false;
                            stunStar.Stop();
                            patrolMove = false;
                            isHome = false;
                            StopAllCoroutines();
                            aggroRegion.SetActive(false);
                            targetScript.audioSrc.PlayOneShot(targetScript.poof);
                            isKO = true;
                            myMesh.enabled = false;
                            myColl.enabled = false;
                            koSmoke.Play();

                            if (breathMist) //sia only
                                StopBreath();

                            if (perpetualMist)//sia only, and niang
                                perpetualMist.SetActive(false);

                            if (clearMist)//sia only, and  niang
                                clearMist.SetActive(true);

                            if (myBat)
                                myBat.DoKOStuff();

                            if (dropPrefab)
                            {
                                var instObj = Instantiate(dropPrefab, transform.position + Vector3.up * 0.8f, Quaternion.identity) as GameObject;
                            }
                        }
                    }
                   
                   
                }
               
            }          
        }
    }

    public void ResetAnims()
    {
        selfAnim.SetFloat("idle", 0);
        selfAnim.SetFloat("run", 0);
        selfAnim.SetFloat("jump", 0);
        selfAnim.SetFloat("attack", 0);
    }

    IEnumerator FalsifySetDest()
    {
        yield return new WaitForEndOfFrame();
        canSetDest = false;
        setDestTimer = 0.2f;

    }

    public void PlayBreath() //sia only
    {
        breathMist.Play();
    }

    public void StopBreath() //sia only
    {
        if(breathMist)
            breathMist.Stop();
    }

    public void SummonBat()
    {
        var instBat = Instantiate(batPrefab, targetScript.transform.position - targetScript.transform.forward + Vector3.up * 0.7f, Quaternion.identity) as GameObject;
        bat batScript = instBat.GetComponent<bat>();
        batScript.koSmoke.Play();
        batScript.myMaster = this;
        batScript.targetScript = targetScript;
        //targetScript.isMarked = true;
        if (targetScript.aggroCounter <= 0)
        {
            targetScript.aggroCounter++;
            targetScript.markedObj.SetActive(true);
        }
        if (!myBat)
            myBat = batScript;
    }

    public IEnumerator EndStun()
    {
        yield return new WaitForSeconds(stunDuration);
        isAtk = false;
        if(stunStar)
            stunStar.Stop();
        isStunned = false;
        if (isNiangshi)
        {
            if (targetScript.aggroCounter <= 0)
                targetScript.aggroCounter++;
        }
        if (!isNiangshi)
            aggroRegion.SetActive(true);
        ResetAnims();
    }

    public void DoBlink()
    {
        //audioSrc.PlayOneShot(oof);
        isInvincible = true;
        invincibleTime = 1;

        if (BlinkCo != null)
            StopCoroutine(BlinkCo);
        BlinkCo = Blink();
        StartCoroutine(BlinkCo);
    }

    IEnumerator Blink()
    {
        if (isInvincible)
        {
            yield return new WaitForSeconds(0.1f);
            myMesh.enabled = false;

            if (BlinkRevertCo != null)
                StopCoroutine(BlinkRevertCo);
            BlinkRevertCo = BlinkRevert();
            StartCoroutine(BlinkRevertCo);
        }
    }

    IEnumerator BlinkRevert()
    {
        if (isInvincible)
        {
            yield return new WaitForSeconds(0.1f);
            myMesh.enabled = true;
            if (BlinkCo != null)
                StopCoroutine(BlinkCo);
            BlinkCo = Blink();
            StartCoroutine(BlinkCo);
        }
    }

    public void GenerateProjectile()
    {
        if(isNiangshi)
        {
            var projInst = Instantiate(projectilePrefab, projectilePos.position, Quaternion.identity) as GameObject;
            projInst.GetComponent<projectile>().direction = projectilePos.forward;
            projInst.GetComponent<projectile>().myParent = this;
            projInst.transform.LookAt(transform.position + projectilePos.forward);
        }
    }

    public void GenerateShockwave()
    {
        if(isNiangshi)
        {
            var shockInst = Instantiate(shockwavePrefab, transform.position, Quaternion.identity) as GameObject;
        }
    }

    public void PlaySquish()
    {
        targetScript.audioSrc.PlayOneShot(targetScript.squish);
    }

    public void PlayChomp()
    {
        targetScript.audioSrc.PlayOneShot(targetScript.chomp);
    }
}
