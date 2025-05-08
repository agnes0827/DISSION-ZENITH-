using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public GameObject go; // menu 객체 여기에 넣기
    private bool activated; // menu 화면 활성화/비활성화
   
    public void Continue()
    {
        activated = false; // activated 상태 변경
        go.SetActive(false); // menu 화면 비활성화
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) // Esc 버튼 클릭 시
        {
            activated = !activated; // activated 상태 변경

            if (activated)
            {
                go.SetActive(true); // menu 화면 활성화
            }
            else
            {
                go.SetActive(false); // menu 화면 비활성화
            }
        }
    }

    public void GoToStartScene()
    {
        SceneManager.LoadScene("StartScene");
    }
}
