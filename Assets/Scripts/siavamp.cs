using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class siavamp : MonoBehaviour
{
    public Animator selfAnim;
    public shiangji targetScript;

    public bool isAggroed;
    public bool isAttack;
    public bool isKO;

    public SkinnedMeshRenderer myMesh;
    public Collider myColl;

    public ParticleSystem koSmoke;
    public ParticleSystem breathMist;

    public float atkCd;
    public float atkTimer;

    Vector3 lookPos;
    Quaternion selfRot;
    Vector3 homeLookPos;

    public GameObject aggroRegion;
    public GameObject perpetualMist; //mist around sia
    public GameObject clearMist; //when sia defeated, this bursts

    private void Start()
    {
        homeLookPos = transform.position + transform.forward;
    }
    // Update is called once per frame
    void Update()
    {
        if(!isKO)
        {
            if (isAggroed)
            {
                lookPos = targetScript.transform.position - transform.position;
                lookPos.y = 0;
                if (lookPos != Vector3.zero)
                    selfRot = Quaternion.LookRotation(lookPos);
                else
                    selfRot = Quaternion.LookRotation(transform.forward);

                transform.rotation = Quaternion.Slerp(transform.rotation, selfRot, 20 * Time.deltaTime);

                if (!isAttack)
                {
                    if (atkCd >= 0)
                        atkCd -= Time.deltaTime;
                    else
                    {
                        atkCd = atkTimer;
                        ResetAnims();
                        selfAnim.SetFloat("attack", 1);
                        isAttack = true;
                    }
                }

                if (targetScript.isSneak || targetScript.isKO)
                {
                    isAggroed = false;
                    targetScript.aggroCounter--;
                    aggroRegion.SetActive(true);
                }
            }
            else
            {
                ResetAnims();
                selfAnim.SetFloat("idle", 1);

                lookPos = homeLookPos - transform.position;
                if (lookPos != Vector3.zero)
                    selfRot = Quaternion.LookRotation(lookPos);
                else
                    selfRot = Quaternion.LookRotation(transform.forward);

                transform.rotation = Quaternion.Slerp(transform.rotation, selfRot, 20 * Time.deltaTime);

            }          
            
        }
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "atk")
        {
            collectible otherScript = other.GetComponent<collectible>();
            if(otherScript.myName == "player")
            {
                if (isAggroed)
                {
                    isAggroed = false;
                    targetScript.aggroCounter--;
                }
                breathMist.Stop();
                perpetualMist.SetActive(false);
                clearMist.SetActive(true);
                targetScript.audioSrc.PlayOneShot(targetScript.poof);
                aggroRegion.SetActive(false);
                isKO = true;
                myMesh.enabled = false;
                myColl.enabled = false;
                koSmoke.Play();
            }
        }
    }

    public void FalsifyAtk()
    {
        isAttack = false;
        ResetAnims();
        selfAnim.SetFloat("idle", 1);
    }

    public void PlayBreath()
    {
        breathMist.Play();
    }

    public void StopBreath()
    {
        breathMist.Stop();
    }

    void ResetAnims()
    {
        selfAnim.SetFloat("idle", 0);
        selfAnim.SetFloat("run", 0);
        selfAnim.SetFloat("jump", 0);
        selfAnim.SetFloat("attack", 0);
    }
}
