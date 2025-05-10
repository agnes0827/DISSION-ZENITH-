using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public GameObject go; // menu ��ü ���⿡ �ֱ�
    private bool activated; // menu ȭ�� Ȱ��ȭ/��Ȱ��ȭ
   
    public void Continue()
    {
        activated = false; // activated ���� ����
        go.SetActive(false); // menu ȭ�� ��Ȱ��ȭ
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) // Esc ��ư Ŭ�� ��
        {
            activated = !activated; // activated ���� ����

            if (activated)
            {
                go.SetActive(true); // menu ȭ�� Ȱ��ȭ
            }
            else
            {
                go.SetActive(false); // menu ȭ�� ��Ȱ��ȭ
            }
        }
    }

    public void GoToStartScene()
    {
        SceneManager.LoadScene("StartScene");
    }
}
