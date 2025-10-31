using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    public void Exit()
    {
        Application.Quit(); // 게임 종료
    }

    /// <summary>
    /// 게임 시작 버튼
    /// </summary>
    public void GoToHouseScene()
    {
        // 필요하면 씬 이동 전 타임스케일 복구
        // Time.timeScale = 1f;
        SceneManager.LoadScene("House");
    }

    void Update()
    {
        
    }
}
