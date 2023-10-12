using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class markerscript : MonoBehaviour
{
    RaycastHit groundHit;
    float yPos;
    public shiangji playerScript;

    public MeshRenderer myMesh;
    // Start is called before the first frame update

    // Update is called once per frame
    private void FixedUpdate()
    {      

        if (!playerScript.isClimb)
        {
            if (Physics.Raycast(playerScript.transform.position + new Vector3(0, 0.1f, 0), Vector3.down, out groundHit, Mathf.Infinity, ~LayerMask.GetMask("Player", "Ignore Raycast")))
            {

                if (groundHit.transform.tag == "ground" || groundHit.transform.tag == "lava")
                {
                    if (!myMesh.enabled)
                        myMesh.enabled = true;
                    yPos = groundHit.point.y;
                    //myQuad.transform.position = new Vector3(selfOwner.transform.position.x, groundHit.point.y + 0.02f, selfOwner.transform.position.z);
                }
                else
                {
                    if (myMesh.enabled)
                        myMesh.enabled = false;
                }
            }
            else
            {
                if (myMesh.enabled)
                    myMesh.enabled = false;
            }

        }
        else
        {

            if (myMesh.enabled)
                myMesh.enabled = false;

        }      

    }

    private void LateUpdate()
    {
        transform.position = new Vector3(playerScript.transform.position.x, yPos, playerScript.transform.position.z);
    }

}
