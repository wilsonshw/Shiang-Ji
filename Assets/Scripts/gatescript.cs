using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class gatescript : MonoBehaviour
{
    public string myString;
    public TextMeshProUGUI myTxt;
    public int numTooth;
    public int numEye;
    public int numPup;

    public GameObject myController;
    public Collider myColl;

    private void Start()
    {
        myTxt.text = myString;
    }

    public IEnumerator DisableTxt()
    {
        yield return new WaitForSeconds(5);
        myTxt.enabled = false;
    }
}
