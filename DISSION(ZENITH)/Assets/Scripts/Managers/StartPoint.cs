using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
//스타트 포인트의 인스펙터창에 현재 맵 이름 넣기

public class StartPoint : MonoBehaviour
{
    public string startPointName; // 이 스타트포인트가 연결된 맵 이름
    private PlayerController thePlayer;

    void Start()
    {
        thePlayer = FindObjectOfType<PlayerController>();
        //thePlayer.previousMapName = SceneManager.GetActiveScene().name;
        // 이전 맵 이름과 스타트포인트 이름이 같으면 위치 이동
        if (thePlayer.currentMapName == startPointName)
        {
            thePlayer.transform.position = transform.position;
        }
    }
    //플레이어의 현재맵이름이 스타트포인트와 같으면 스타트포인트에서 시작
    
}
