using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//startPoint변수에 현재 씬 이름 쓰기


public class StartPoint : MonoBehaviour
{
    public string startPoint;
    private PlayerController thePlayer;
    void Start()
    {
        thePlayer = FindObjectOfType<PlayerController>();
        if (startPoint == thePlayer.currentMapName)
        {
            thePlayer.transform.position = this.transform.position;
        }
    }

}
