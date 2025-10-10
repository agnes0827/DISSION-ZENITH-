using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    public void Exit()
    {
        Application.Quit(); // ���� ����
    }

    /// <summary>
    /// ���� ���� ��ư
    /// </summary>
    public void GoToHouseScene()
    {
        // �ʿ��ϸ� �� �̵� �� Ÿ�ӽ����� ����
        // Time.timeScale = 1f;
        SceneManager.LoadScene("House");
    }

    void Update()
    {
        
    }
}
