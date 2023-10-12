using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.InputSystem;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class shiangji : MonoBehaviour
{
    public bool hasSeenMarkInfo;
    public GameObject markedInfo;

    public AudioSource audioSrc;
    public AudioClip lightSound;
    public AudioClip batSound;
    public AudioClip chomp;
    public AudioClip ding;
    public AudioClip oof;
    public AudioClip piu;
    public AudioClip poof;
    public AudioClip pop1;
    public AudioClip roarBoy;
    public AudioClip roarGirl;
    public AudioClip squish;
    public AudioClip whoosh;
    public SkinnedMeshRenderer myMesh;

    public RuntimeAnimatorController contClimb; //climb controller
    public RuntimeAnimatorController contExhaust; //exhausted
    public RuntimeAnimatorController contNormal;//normal controller
    public RuntimeAnimatorController contSneak;//sneak controller override
    public RuntimeAnimatorController contVamp; //vampire controller override

    public bool infoActive;
    public bool isAtk; //attack anim playing
    public bool isClimb;
    public bool isDeepBreath; //drawing breath, 1 second duration
    public bool isExhausted; //triggered when fatigue reaches max
    public bool isInvincible;
    public bool isJumping;
    public bool isKO;
    //public bool isMarked; //if ANY vampire is aggro, this is true. Marked ShiangJi loses health every 3 seconds. Removing aggro removes this.
    public bool isMoving; //i.e. input detected
    public bool isSceneTransition;
    public bool isSneak;
    public bool isUp; //pushed up by mist
    public bool isVamp;
    public bool raycastEnabled;

    public Vector2 inputVec;

    public Vector3 checkPointPos;
    public Vector3 direction;
    public Vector3 lookPos;
    public Vector3 targetPos;

    public int aggroCounter;
    public int counterTooth;
    public int counterEye;
    public TextMeshProUGUI txtTooth;
    public TextMeshProUGUI txtEye;
    public int charmCounter;
    public TextMeshProUGUI charmTxt;
    public float climbSpeed;
    public float currentExhaust;
    public float currentFatigue;
    public float currentStamina;
    public float currentBreathDur;
    public float factorExhaust; //how fast exhaustion decreases
    public float factorFatigue; //how fast fatigue increases, this is fixed, set in inspector
    public float factorStamina; //how fast stamina recovers
    public float invincibleTime;
    public float jumpSpeed;
    public float markedCd;
    public float markedTime; //set in inspector, does not change
    public float maxBreathDur;//should not change, set in inspector
    public float maxFatigue;//should not change, set in inspector
    public float maxStamina;//should not change, set in inspector
    public float normalSpeed;
    public int pupCounter;
    public TextMeshProUGUI pupTxt;
    public float reboundSpeed; //bouncing off bat
    public float selfSpeed;
    public float sneakSpeed;
    public float sphereCastFactor;
    public float sphereCastRadius;

    public Image breathBar;
    public Image exhaustBar;
    public Image fatigueBar;
    public Image staminaBar;

    public GameObject audioJump;
    public GameObject audioSteps; //for normal mode running
    public GameObject camFollow;
    public GameObject climbObj;
    public GameObject escapeBack;
    public GameObject escapeMenu;
    public GameObject gateObj;
    public GameObject imgTooth;
    public GameObject imgEye;
    public GameObject imgCharm;
    public GameObject infoObj;
    public GameObject interactObj;
    public GameObject jumpAtkObj; //the stepping atkobj
    public GameObject lightObj;
    public GameObject markedObj;
    public Transform owParticles;
    public GameObject pauseMenu;
    public GameObject pauseSubMenu; //e.g. volume controls under options
    public GameObject pupImage; //enabled when first pup collected
    public GameObject textBreath;
    public GameObject textExhaust;
    public GameObject textFatigue;
    public GameObject textStamina;
    public Transform upMist; //the mist that pushes up

    public ParticleSystem charmFx; //when collecting charm
    public ParticleSystem charmGoneFx;
    public ParticleSystem pupFx; //when collecting pup
    public ParticleSystem keyFx; //when collecting key
    public ParticleSystem koFx; //when ko'ed
    public ParticleSystem noKeyFx; //when no key and try to unlock gate
    public ParticleSystem toothFx; //when collecting tooth
    public ParticleSystem eyeFx; //when collecting eye

    public Quaternion selfRot;

    public Rigidbody selfRigid;

    public CapsuleCollider selfCapsuleColl;

    public Animator selfAnim;

    RaycastHit groundHit;
    Vector3 camFollowOffset;

    public float selfHP;
    public float maxHP;
    public Image lifeBar;

    IEnumerator BlinkCo;
    IEnumerator BlinkRevertCo;
    public IEnumerator StopJumpatkCo;

    public CanvasGroup blackOut;

    // Start is called before the first frame update
    void Start()
    {
        selfAnim.runtimeAnimatorController = contNormal;
        markedCd = markedTime;
        isJumping = true;//raycast issue
        selfHP = maxHP;
        lifeBar.fillAmount = selfHP/maxHP;
        charmCounter = 0;
        charmTxt.text = charmCounter.ToString();

        camFollowOffset = camFollow.transform.position - transform.position;
        currentFatigue = 0;
        currentStamina = 0;
        currentBreathDur = 0;
        currentExhaust = 0;
        textStamina.SetActive(false);
        textBreath.SetActive(false);
        textFatigue.SetActive(false);
        textExhaust.SetActive(false);
        breathBar.fillAmount = 0;
        staminaBar.fillAmount = 0;
        fatigueBar.fillAmount = 0;
        exhaustBar.fillAmount = 0;
        selfSpeed = normalSpeed;
        sphereCastRadius = selfCapsuleColl.radius * transform.localScale.x * 0.8f; //smaller than capsule radius

        LoadPlayerPrefs();
    }

    private void Update()
    {
        if(!isKO && !isSceneTransition)
        {
            if (!isClimb && !isExhausted)
            {
                if(!isSneak)
                {
                    if (selfAnim.GetFloat("run") == 0)
                    {
                        if (audioSteps.activeSelf)
                            audioSteps.SetActive(false);
                    }
                    else if (selfAnim.GetFloat("run") == 1)
                    {
                        if (!audioSteps.activeSelf)
                            audioSteps.SetActive(true);
                    }
                }
               
                /*if (selfAnim.GetFloat("jump") == 0)
                {
                    if (audioJump.activeSelf)
                        audioJump.SetActive(false);
                }
                else if (selfAnim.GetFloat("jump") == 1)
                {
                    if (!audioJump.activeSelf && !isUp)
                        audioJump.SetActive(true);
                }*/
            }

            if (isSneak)
            {
                if (currentBreathDur > 0)
                {
                    currentBreathDur -= Time.deltaTime;
                    breathBar.fillAmount = currentBreathDur / maxBreathDur;
                }
                else
                {
                    FalsifySneak();
                    currentBreathDur = 0;
                    breathBar.fillAmount = currentBreathDur / maxBreathDur;
                    textBreath.SetActive(false);
                }

                if (currentFatigue < maxFatigue)
                {
                    currentFatigue += Time.deltaTime * factorFatigue;
                    fatigueBar.fillAmount = currentFatigue / maxFatigue;
                    if (!textFatigue.activeSelf)
                        textFatigue.SetActive(true);
                }
                else //reached max fatigue, danger! Force stop holding breath, wait 5 seconds before able to hold again
                {
                    DoExhausted();
                }
            }

            if(!isSneak)
            {
                if(!isExhausted)
                {
                    if (currentStamina < maxStamina)
                    {
                        currentStamina += Time.deltaTime * factorStamina;
                        staminaBar.fillAmount = currentStamina / maxStamina;
                        if (!textStamina.activeSelf)
                            textStamina.SetActive(true);
                    }
                    else
                    {
                        currentStamina = maxStamina;
                        staminaBar.fillAmount = currentStamina / maxStamina;
                    }
                }
                else if(isExhausted)
                {
                    if (currentExhaust > 0)
                    {
                        currentExhaust -= Time.deltaTime * factorExhaust;
                        exhaustBar.fillAmount = currentExhaust / maxFatigue; //note: max fatigue is max exhaust
                    }
                    else
                    {
                        selfAnim.runtimeAnimatorController = contNormal;
                        selfSpeed = normalSpeed;
                        isExhausted = false;
                        currentExhaust = 0;
                        exhaustBar.fillAmount = 0;
                        textExhaust.SetActive(false);
                    }
                }

                if (currentFatigue > 0)
                {
                    currentFatigue -= Time.deltaTime * factorFatigue * 2;
                    fatigueBar.fillAmount = currentFatigue / maxFatigue;
                }
                else
                {
                    currentFatigue = 0;
                    fatigueBar.fillAmount = currentFatigue / maxFatigue;
                    textFatigue.SetActive(false);
                }

            }

            if(isInvincible)
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

            //if (isMarked)
            //{
                if (aggroCounter > 0)
                {
                    DoMarkedCountdown();
                }
                else
                {
                    aggroCounter = 0;
                    //isMarked = false;
                    if (markedObj.activeSelf)
                        markedObj.SetActive(false);
                    markedCd = markedTime;
                }                    
            //}


        }

        if (!hasSeenMarkInfo)
        {
            if (markedObj.activeSelf)
            {
                markedInfo.SetActive(true);
                infoActive = true;
                infoObj = markedInfo;
                escapeBack.SetActive(true);
                escapeMenu.SetActive(false);
                ResetAnims();
                audioSteps.SetActive(false);
                audioJump.SetActive(false);
                FalsifySneak();
                Time.timeScale = 0;
                hasSeenMarkInfo = true;
                PlayerPrefs.SetInt("hasseenmark", 1);
            }
        }

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(!isSceneTransition)
        {
            if (!isClimb && !isKO)
            {
                if (raycastEnabled)
                {
                    if (Physics.SphereCast(transform.TransformPoint(selfCapsuleColl.center), sphereCastRadius, Vector3.down, out groundHit, selfCapsuleColl.height / 2 * transform.localScale.x - sphereCastRadius * sphereCastFactor))
                    {
                        if (groundHit.transform.tag == "ground")
                        {
                            if (isJumping)
                            {
                                selfRigid.velocity = Vector3.zero;
                                isJumping = false;
                            }
                        }
                        else if (groundHit.transform.tag == "enemy")
                        {
                            if (groundHit.transform.GetComponent<bat>())
                            {
                                jumpAtkObj.SetActive(true);
                                if (StopJumpatkCo != null)
                                    StopCoroutine(StopJumpatkCo);

                                StopJumpatkCo = StopJumpAtk();
                                StartCoroutine(StopJumpatkCo);

                                DoRebound();
                            }
                        }
                    }
                    else
                    {
                        if (!isJumping)
                        {
                            isJumping = true;
                        }

                    }
                }
            }

            if (isJumping && !isKO)
            {
                if (selfRigid.velocity.y <= 0) //going down
                    raycastEnabled = true;
            }

            if (!isDeepBreath && !isAtk && !isClimb && !isKO)
            {
                NormalMoveStuff();
                NormalSlerpStuff();
            }

            if (isClimb && !isKO)
            {
                ClimbStuff();
            }

            if(isUp)
            {
                selfRigid.velocity += 0.4f * Vector3.up / Mathf.Abs(transform.position.y - upMist.position.y);
            }
        }
       
    }

    private void LateUpdate()
    {
        camFollow.transform.position = transform.position + camFollowOffset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!isKO && !isSceneTransition)
        {
            if (other.tag == "collectible")
            {
                collectible otherScript = other.GetComponent<collectible>();
                if (otherScript.myName == "charm" || otherScript.myName == "niangcharm")
                {
                    audioSrc.PlayOneShot(ding);
                    if (!imgCharm.activeSelf)
                        imgCharm.SetActive(true);
                    charmFx.Play();
                    charmCounter++;
                    charmTxt.text = charmCounter.ToString();
                }
                else if(otherScript.myName == "pup")
                {
                    audioSrc.PlayOneShot(ding);
                    if (!pupImage.activeSelf)
                        pupImage.SetActive(true);

                    pupFx.Play();
                    pupCounter++;
                    pupTxt.text = pupCounter.ToString();

                    other.gameObject.SetActive(false);
                }
                else if(otherScript.myName == "checkpoint")
                {
                    audioSrc.PlayOneShot(ding);
                    otherScript.myMesh.material = otherScript.myMat;
                    otherScript.myShine.SetActive(true);
                    otherScript.myColl.enabled = false;
                    checkPointPos = other.transform.position + new Vector3(0, 1, 0);
                }
                else if(otherScript.myName == "tooth")
                {
                    audioSrc.PlayOneShot(ding);
                    if (!imgTooth.activeSelf)
                        imgTooth.SetActive(true);
                    toothFx.Play();
                    counterTooth++;
                    txtTooth.text = counterTooth.ToString();
                    Destroy(otherScript.myShine.gameObject);
                }
                else if(otherScript.myName == "eye")
                {
                    audioSrc.PlayOneShot(ding);
                    if (!imgEye.activeSelf)
                        imgEye.SetActive(true);
                    eyeFx.Play();
                    counterEye++;
                    txtEye.text = counterEye.ToString();
                    Destroy(otherScript.myShine.gameObject);
                }
            }
            else if (other.tag == "atk")
            {
                collectible otherScript = other.GetComponent<collectible>();
                if (otherScript.myName == "enemy")
                {
                    if(!isInvincible)
                    {
                        TakeDamage();
                    }
                   
                }
                else if(otherScript.myName == "lava")
                {
                    selfHP = 0;
                    lifeBar.fillAmount = 0;
                    DoKoStuff();
                }
                else if(otherScript.myName == "mist")
                {
                    if(!isSneak)
                    {
                        if (!isInvincible)
                        {
                            TakeDamage();
                        }
                    }
                }
            }
            else if (other.tag == "climb")
            {
                interactObj.SetActive(true);
                climbObj = other.gameObject;
            }
            else if(other.tag == "light")
            {
                interactObj.SetActive(true);
                lightObj = other.gameObject;
            }
            else if(other.tag == "gate") //usually used to activate a controller
            {
                interactObj.SetActive(true);
                gateObj = other.gameObject;
            }
            else if(other.tag == "end")
            {
                isSceneTransition = true;
                selfAnim.runtimeAnimatorController = contNormal;
                ResetAnims();
                audioJump.SetActive(false);
                audioSteps.SetActive(false);
                collectible otherScript = other.GetComponent<collectible>();
                pause pauseScript = other.GetComponent<pause>();
                int mapCompleted;
                if (PlayerPrefs.HasKey("mapcompleted"))
                {
                    mapCompleted = PlayerPrefs.GetInt("mapcompleted");
                    if (otherScript.myId >= mapCompleted)
                        PlayerPrefs.SetInt("mapcompleted", otherScript.myId);
                }
                else
                    PlayerPrefs.SetInt("mapcompleted", otherScript.myId);
                pauseScript.ClickLevelSelect();
            }
            else if(other.tag == "thankyou")
            {
                collectible otherScript = other.GetComponent<collectible>();
                otherScript.myShine.SetActive(true);
                StartCoroutine(DoBlackoutToTitle());
            }
            else if(other.tag == "upmist")
            {
                isUp = true;
                upMist = other.transform;                
                isJumping = true;
                audioSteps.SetActive(false);
                if (selfAnim.GetFloat("jump") != 1)
                {
                    ResetAnims();
                    selfAnim.SetFloat("jump", 1);
                }

                if(!isSneak)
                {
                    if (!isInvincible)
                    {
                        TakeDamage();
                    }
                }
               
            }
        }
      
    }

    private void OnTriggerStay(Collider other)
    {
        if(!isKO && !isSceneTransition)
        {
            if (other.tag == "atk")
            {
                collectible otherScript = other.GetComponent<collectible>();
                if (otherScript.myName == "enemy")
                {
                    if (!isInvincible)
                    {
                        selfHP--;
                        lifeBar.fillAmount = selfHP / maxHP;
                        if (selfHP > 0)
                        {
                            PlayOw();
                            DoBlink();
                        }
                        else if (selfHP <= 0)
                        {
                            selfHP = 0;
                            lifeBar.fillAmount = 0;
                            DoKoStuff();
                        }
                    }

                }
                else if (otherScript.myName == "lava")
                {
                    selfHP = 0;
                    lifeBar.fillAmount = 0;
                    DoKoStuff();
                }
                else if (otherScript.myName == "mist")
                {
                    if (!isSneak)
                    {
                        if (!isInvincible)
                        {
                            selfHP--;
                            lifeBar.fillAmount = selfHP / maxHP;
                            if (selfHP > 0)
                            {
                                PlayOw();
                                DoBlink();
                            }
                            else if (selfHP <= 0)
                            {
                                selfHP = 0;
                                lifeBar.fillAmount = 0;
                                DoKoStuff();
                            }
                        }
                    }
                }
            }
            else if (other.tag == "upmist")
            {
                isUp = true;
                upMist = other.transform;
                if(!isJumping)
                    isJumping = true;
                if(audioSteps.activeSelf)
                    audioSteps.SetActive(false);
                if (selfAnim.GetFloat("jump") != 1)
                {
                    ResetAnims();
                    selfAnim.SetFloat("jump", 1);
                }

                if (!isSneak)
                {
                    if (!isInvincible)
                    {
                        TakeDamage();
                    }
                }
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if(!isKO && !isSceneTransition)
        {
            if (other.tag == "climb")
            {
                if (isClimb)
                {
                    selfAnim.runtimeAnimatorController = contNormal;
                    isJumping = true;
                    isClimb = false;
                    selfRigid.useGravity = true;
                }
                climbObj = null;
                interactObj.SetActive(false);
            }
            else if(other.tag == "light")
            {
                lightObj = null;
                interactObj.SetActive(false);
            }
            else if(other.tag == "gate")
            {
                gateObj = null;
                interactObj.SetActive(false);
            }
            else if(other.tag == "upmist")
            {
                isUp = false;
                upMist = null;
            }
        }
      
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "shockwave")
        {
            if (!isInvincible)
            {
                TakeDamage();
            }
        }
    }

    public void DoKoStuff()
    {
        audioSrc.PlayOneShot(piu);
        if (StopJumpatkCo != null)
            StopCoroutine(StopJumpatkCo);
        jumpAtkObj.SetActive(false);

        if (BlinkCo != null)
            StopCoroutine(BlinkCo);
        if (BlinkRevertCo != null)
            StopCoroutine(BlinkRevertCo);

        isKO = true;
        selfRigid.useGravity = false;
        selfRigid.velocity = Vector3.zero;
        isAtk = false;
        isClimb = false;
        isDeepBreath = false;
        isJumping = true; //respawns above checkpoint
        isSneak = false;
        isUp = false;
        upMist = null;

        //isMarked = false;
        markedCd = markedTime;
        markedObj.SetActive(false);

        climbObj = null;
        interactObj.SetActive(false);
        //selfRigid.useGravity = true;

        if (!isExhausted)
        {
            selfSpeed = normalSpeed;
            selfAnim.runtimeAnimatorController = contNormal;
        }

        audioSteps.SetActive(false);
        audioJump.SetActive(false);

        inputVec = Vector2.zero;
        ResetAnims();
        selfAnim.SetFloat("idle", 1);

        selfHP = maxHP;
        lifeBar.fillAmount = 1;
        currentFatigue = 0;
        currentStamina = 0;
        currentBreathDur = 0;
        textStamina.SetActive(false);
        textBreath.SetActive(false);
        textFatigue.SetActive(false);
        breathBar.fillAmount = 0;
        staminaBar.fillAmount = 0;
        fatigueBar.fillAmount = 0;

        invincibleTime = 0;
        isInvincible = false;
        //myMesh.enabled = true;

        myMesh.enabled = false;
        koFx.Play();
        StopAllCoroutines();
        StartCoroutine(FalsifyKO());

        charmCounter = 0;
        charmTxt.text = charmCounter.ToString();
        //transform.position = checkPointPos;
    }

    public void DoBlink()
    {
        audioSrc.PlayOneShot(oof);
        isInvincible = true;
        invincibleTime = 1;

        if (BlinkCo != null)
            StopCoroutine(BlinkCo);
        BlinkCo = Blink();
        StartCoroutine(BlinkCo);
    }

    public void DoRebound()
    {
        selfRigid.velocity = new Vector3(selfRigid.velocity.x, 0, selfRigid.velocity.z);
        selfRigid.velocity += new Vector3(0, reboundSpeed, 0);
        isJumping = true;
        ResetAnims();
        selfAnim.SetFloat("jump", 1);
        raycastEnabled = false;
    }

    public void DoMarkedCountdown()
    {
        if (!markedObj.activeSelf)
            markedObj.SetActive(true);

        if (markedCd > 0)
            markedCd -= Time.deltaTime;
        else
        {
            selfHP--;
            lifeBar.fillAmount = selfHP / maxHP;
            if (selfHP > 0)
            {
                audioSrc.PlayOneShot(oof);

                PlayOw();
            }
            else if (selfHP <= 0)
            {
                selfHP = 0;
                lifeBar.fillAmount = 0;
                DoKoStuff();
            }
            markedCd = markedTime;
        }
    }


    public void PlayOw()
    {
        for (int i = 0; i < owParticles.childCount; i++)
        {
            if (!owParticles.GetChild(i).gameObject.activeSelf)
            {
                owParticles.GetChild(i).gameObject.SetActive(true);
                break;
            }
        }
    }

    public IEnumerator StopJumpAtk()
    {
        yield return new WaitForEndOfFrame();
        jumpAtkObj.SetActive(false);
    }

    IEnumerator Blink()
    {
        if(isInvincible)
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
        if(isInvincible)
        {
            yield return new WaitForSeconds(0.1f);
            myMesh.enabled = true;
            if (BlinkCo != null)
                StopCoroutine(BlinkCo);
            BlinkCo = Blink();
            StartCoroutine(BlinkCo);
        }
    }

    public void NormalMoveStuff()
    {
        if(!isJumping)
            selfRigid.velocity = Vector3.zero;
        direction = new Vector3(inputVec.x, 0, inputVec.y);
        var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
        var skewedDir = matrix.MultiplyPoint3x4(direction);

        skewedDir = Vector3.ProjectOnPlane(skewedDir, Vector3.up);
        skewedDir.Normalize();      
        //direction = Camera.main.transform.TransformDirection(direction);       
        float distance = selfSpeed * Time.deltaTime;
        skewedDir *= distance;
        targetPos = transform.position + skewedDir;       
        selfRigid.MovePosition(targetPos);

        if (inputVec == Vector2.zero)
        {
            if (isMoving) //meaning running animation on ground, or jumping animation in air
            {
                if(!isJumping)
                {
                    selfRigid.velocity = Vector3.zero;
                    ResetAnims();
                    selfAnim.SetFloat("idle", 1);

                }

                isMoving = false;
            }
            else
            {
                if(!isJumping)
                {
                    selfRigid.velocity = Vector3.zero;
                    if (selfAnim.GetFloat("idle") != 1)
                    {
                        ResetAnims();
                        selfAnim.SetFloat("idle", 1);
                    }
                }
               
            }
        }
        else
        {
            if(!isMoving)
            {
                if(!isJumping)
                {
                    ResetAnims();
                    selfAnim.SetFloat("run", 1);
                }

                isMoving = true;
            }
            else
            {
                if(!isJumping)
                {
                    if (selfAnim.GetFloat("run") != 1)
                    {
                        ResetAnims();
                        selfAnim.SetFloat("run", 1);
                    }
                }
               
            }
        }
    }

    public void NormalSlerpStuff()
    {
        lookPos = targetPos - transform.position;
        if (lookPos != Vector3.zero)
            selfRot = Quaternion.LookRotation(lookPos);
        else
            selfRot = Quaternion.LookRotation(transform.forward);

        transform.rotation = Quaternion.Slerp(transform.rotation, selfRot, 20 * Time.deltaTime);
    }

    public void ClimbStuff()
    {
        direction = new Vector3(0, inputVec.y, 0);
        float distance = climbSpeed * Time.deltaTime;
        direction *= distance;
        targetPos = transform.position + direction;
        selfRigid.MovePosition(targetPos);

        if (inputVec.y == 0)
        {
            if (isMoving) //meaning running animation on ground, or jumping animation in air
            {
                ResetAnims();
                selfAnim.SetFloat("idle", 1);
                isMoving = false;
            }
            else
            {
                if (selfAnim.GetFloat("idle") != 1)
                {
                    ResetAnims();
                    selfAnim.SetFloat("idle", 1);
                }
            }
        }
        else
        {
            if (!isMoving)
            {

                ResetAnims();
                selfAnim.SetFloat("run", 1);
                isMoving = true;
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
    }

    public void OnRun(InputAction.CallbackContext value)
    {
        if(!isKO && !isSceneTransition)
            ReadInputVec(value);
    }

    void ReadInputVec(InputAction.CallbackContext value)
    {
        inputVec = Vector2.zero;
        inputVec = value.ReadValue<Vector2>();
    } 

    public void OnDeepBreath(InputAction.CallbackContext value) //mouse down, start drawing breath
    {
        if (!isSceneTransition)
        {
            if (value.started)
            {
                if (!isJumping && !isKO && Time.timeScale != 0)
                {
                    if (!isDeepBreath && currentStamina == maxStamina)
                    {
                        isDeepBreath = true;
                        ResetAnims();
                        selfAnim.SetFloat("deepbreath", 1);
                    }
                }

            }
        }
    }

    public void OnDeepBreathCancelled(InputAction.CallbackContext value) //mouse up
    {
        if(!isSceneTransition)
        {
            if (value.performed)
            {
                if (isDeepBreath)
                {
                    isDeepBreath = false;
                }

                if (isSneak)
                {
                    FalsifySneak();
                }
            }
        }
       
    }

    public void OnAttack(InputAction.CallbackContext value)
    {
        if(!isSceneTransition)
        {
            if (value.started)
            {
                if (!isKO && !isJumping && !isAtk)
                {
                    if (charmCounter > 0)
                    {
                        isAtk = true;
                        ResetAnims();
                        selfAnim.SetFloat("attack", 1);

                        charmCounter--;
                        if (charmCounter <= 0)
                        {
                            imgCharm.SetActive(false);
                            charmCounter = 0;
                        }
                        charmTxt.text = charmCounter.ToString();
                    }
                }
            }
        }
      
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        if (!isSceneTransition)
        {
            if (value.started)
            {
                if (!isAtk && !isDeepBreath && !isClimb && !isKO && !isExhausted && Time.timeScale != 0)
                {
                    if (!isJumping)
                    {
                        selfRigid.velocity = new Vector3(selfRigid.velocity.x, 0, selfRigid.velocity.z);
                        selfRigid.velocity += new Vector3(0, jumpSpeed, 0);
                        isJumping = true;
                        ResetAnims();
                        selfAnim.SetFloat("jump", 1);
                        if (!audioJump.activeSelf)
                            audioJump.SetActive(true);
                        raycastEnabled = false;

                        if (isSneak)
                        {
                            currentBreathDur--; //immediately deduct some breath when jump performed during sneak
                            breathBar.fillAmount = currentBreathDur / maxBreathDur;
                            if (currentBreathDur <= 0)
                            {
                                FalsifySneak();
                                currentBreathDur = 0;
                                breathBar.fillAmount = currentBreathDur / maxBreathDur;
                                textBreath.SetActive(false);
                            }

                            currentFatigue += 0.5f; //increases fatigue faster when jump performed during sneak
                            fatigueBar.fillAmount = currentFatigue / maxFatigue;
                            if (currentFatigue >= maxFatigue)
                                DoExhausted();
                        }
                    }
                }
            }
        }

    }
    public void OnPause(InputAction.CallbackContext value)
    {
        if(!isSceneTransition)
        {
            if (value.started)
            {
                if (!infoActive)
                {
                    if (!pauseMenu.activeSelf) //2 possibilities: player is in normal game, or player is in a sub-menu
                    {
                        if (pauseSubMenu) //player in sub-menu
                        {
                            if (pauseSubMenu.activeSelf) //player in sub-menu
                            {
                                pauseSubMenu.SetActive(false);
                                pauseSubMenu = null;
                                pauseMenu.SetActive(true);
                                escapeBack.SetActive(true);
                                escapeMenu.SetActive(false);
                                Time.timeScale = 0;
                            }
                        }
                        else //player in game
                        {
                            pauseMenu.SetActive(true);
                            escapeBack.SetActive(true);
                            escapeMenu.SetActive(false);
                            Time.timeScale = 0;
                        }                       
                    }
                    else
                    {
                        pauseMenu.SetActive(false);
                        escapeBack.SetActive(false);
                        escapeMenu.SetActive(true);
                        Time.timeScale = 1;
                    }
                } 
                else if(infoActive)
                {
                    infoActive = false;
                    infoObj.SetActive(false);
                    escapeBack.SetActive(false);
                    escapeMenu.SetActive(true);
                    Time.timeScale = 1;
                }
            }
        }
       
    }

    public void OnInteract(InputAction.CallbackContext value)
    {
        if(!isSceneTransition)
        {
            if (value.started)
            {
                if (!isDeepBreath && !isJumping && !isKO && !isExhausted && !isAtk)
                {
                    if (!isClimb) //note: make sure climbObj and lightObj are NOT close to each other
                    {
                        if (climbObj)
                        {
                            float offSet = 0.5f * climbObj.GetComponent<BoxCollider>().size.z * climbObj.transform.localScale.z;
                            transform.position = new Vector3(climbObj.transform.position.x, transform.position.y, climbObj.transform.position.z) - climbObj.transform.forward * offSet;
                            transform.LookAt(transform.position + climbObj.transform.forward);
                            selfAnim.runtimeAnimatorController = contClimb;
                            isJumping = false;
                            isClimb = true;
                            audioJump.SetActive(false);
                            audioSteps.SetActive(false);
                            selfRigid.useGravity = false;

                            if (isSneak)
                            {
                                isSneak = false;
                                currentBreathDur = 0;
                                breathBar.fillAmount = 0;
                                textBreath.SetActive(false);
                            }
                        }

                        if (lightObj)
                        {
                            lightorb lightScript = lightObj.GetComponent<lightorb>();
                            if (!lightScript.myLight.gameObject.activeSelf)
                            {
                                audioSrc.PlayOneShot(lightSound);
                                lightScript.InitLight();
                            }
                        }

                        if (gateObj)
                        {
                            /*collectible otherScript = gateObj.GetComponent<collectible>();
                            if (keyCounter > 0)
                            {
                                keyCounter--;
                                otherScript.myShine.SetActive(true); //gate controller
                                otherScript.myColl.enabled = false; //falsifies the gatetrigger collider because no future use
                                interactObj.SetActive(false);
                                gateObj = null;
                            }
                            else
                            {
                                if (!noKeyFx.isPlaying)
                                    noKeyFx.Play();
                            }*/
                            gatescript otherScript = gateObj.GetComponent<gatescript>();
                            if (counterEye >= otherScript.numEye && counterTooth >= otherScript.numTooth && pupCounter >= otherScript.numPup)
                            {
                                if (counterEye > 0)
                                    counterEye -= otherScript.numEye;
                                if (counterTooth > 0)
                                    counterTooth -= otherScript.numTooth;
                                if (pupCounter > 0)
                                    pupCounter -= otherScript.numPup;
                                txtTooth.text = counterTooth.ToString();
                                txtEye.text = counterEye.ToString();
                                pupTxt.text = pupCounter.ToString();
                                if (counterEye <= 0)
                                {
                                    counterEye = 0;
                                    imgEye.SetActive(false);
                                }
                                if (counterTooth <= 0)
                                {
                                    counterTooth = 0;
                                    imgTooth.SetActive(false);
                                }
                                if (pupCounter <= 0)
                                {
                                    pupCounter = 0;
                                    pupImage.SetActive(false);
                                }
                                otherScript.myController.SetActive(true);
                                otherScript.myColl.enabled = false;
                                interactObj.SetActive(false);
                                gateObj = null;
                            }
                            else
                            {
                                if (!otherScript.myTxt.enabled)
                                {
                                    otherScript.myTxt.enabled = true;
                                    otherScript.StartCoroutine(otherScript.DisableTxt());
                                }
                            }
                        }
                    }
                    else
                    {
                        selfAnim.runtimeAnimatorController = contNormal;
                        isJumping = true;
                        isClimb = false;
                        selfRigid.useGravity = true;
                        climbObj = null;
                        interactObj.SetActive(false);
                    }
                }

            }
        }
        
    }

    public void DoExhausted()
    {
        currentFatigue = 0;
        fatigueBar.fillAmount = 0;
        FalsifySneak();
        currentBreathDur = 0;
        breathBar.fillAmount = 0;
        textBreath.SetActive(false);
        currentStamina = 0;
        staminaBar.fillAmount = 0;
        textStamina.SetActive(false);

        currentExhaust = maxFatigue;
        exhaustBar.fillAmount = 1;
        textExhaust.SetActive(true);
        isExhausted = true;
        selfSpeed = sneakSpeed * 0.5f;
        selfAnim.runtimeAnimatorController = contExhaust;
    }

    public void FalsifyDeepBreath()
    {
        isDeepBreath = false;
    }

    public void FalsifySneak()
    {
        isSneak = false;
        selfSpeed = normalSpeed;
        selfAnim.runtimeAnimatorController = contNormal;
        currentBreathDur = 0;
        breathBar.fillAmount = 0;
        textBreath.SetActive(false);
    }


    public void TruifySneak()
    {
        if(!isKO)
        {
            isSneak = true;
            currentBreathDur = maxBreathDur;
            breathBar.fillAmount = currentBreathDur / maxBreathDur;
            textBreath.SetActive(true);
            currentStamina = 0;
            staminaBar.fillAmount = 0;
            textStamina.SetActive(false);
            selfSpeed = sneakSpeed;
            selfAnim.runtimeAnimatorController = contSneak;
        }
       
    }

    public void FalsifyAtk()
    {
        isAtk = false;
    }

    public void PlayCharmGoneFx()
    {
        charmGoneFx.Play();
    }

    public void ResetAnims()
    {
        audioSteps.SetActive(false);
        audioJump.SetActive(false);
        selfAnim.SetFloat("idle", 0);
        selfAnim.SetFloat("run", 0);
        selfAnim.SetFloat("jump", 0);
        selfAnim.SetFloat("deepbreath", 0);
        selfAnim.SetFloat("attack", 0);
    }

    public void TakeDamage()
    {
        if(!isKO)
        {
            selfHP--;
            lifeBar.fillAmount = selfHP / maxHP;
            if (selfHP > 0)
            {
                PlayOw();
                DoBlink();
            }
            else if (selfHP <= 0)
            {
                selfHP = 0;
                lifeBar.fillAmount = 0;
                DoKoStuff();
            }
        }
       
    }

    public IEnumerator FalsifyKO()
    {
        yield return new WaitForSeconds(1);
        selfRigid.useGravity = true;
        transform.position = checkPointPos;
        myMesh.enabled = true;
        isKO = false;
        aggroCounter = 0;
    }

    public void PlayWhoosh()
    {
        audioSrc.PlayOneShot(whoosh);
    }

    public IEnumerator DoBlackoutToTitle()
    {
        CanvasGroup cvGroup = blackOut.GetComponent<CanvasGroup>();
        while (cvGroup.alpha < 1)
        {
            cvGroup.alpha += Time.unscaledDeltaTime * 0.1f;
            yield return null;
        }
        SceneManager.LoadScene("title");
    }

    public void LoadPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("hasseenmark"))//been marked before
            hasSeenMarkInfo = true;
        else
            hasSeenMarkInfo = false;
    }
}
