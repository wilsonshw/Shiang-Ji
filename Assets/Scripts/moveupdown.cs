using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//for rect transform
public class moveupdown : MonoBehaviour
{
    //for rect transform
    public RectTransform myRect;
    public Vector2 homePos; 
    public Vector2 downPos;
    public float dist;
    public float selfSpeed;

    public Vector2 targetPos;
    // Start is called before the first frame update
    void Start()
    {
        homePos = myRect.anchoredPosition;
        downPos = homePos + Vector2.down * dist;
        targetPos = downPos;
    }

    // Update is called once per frame
    void Update()
    {
        myRect.anchoredPosition = Vector3.MoveTowards(myRect.anchoredPosition, targetPos, selfSpeed * Time.deltaTime);
        if(myRect.anchoredPosition == targetPos)
        {
            if (targetPos == downPos)
                targetPos = homePos;
            else if (targetPos == homePos)
                targetPos = downPos;
        }
        
    }
}
