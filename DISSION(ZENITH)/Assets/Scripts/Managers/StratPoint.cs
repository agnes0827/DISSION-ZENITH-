using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StratPoint : MonoBehaviour
{
    public string startPoint;
    private PlayerController1 thePlayer;
    void Start()
    {
        thePlayer = FindObjectOfType<PlayerController1>();
        if (startPoint == thePlayer.currentMapName)
        {
            thePlayer.transform.position = this.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
