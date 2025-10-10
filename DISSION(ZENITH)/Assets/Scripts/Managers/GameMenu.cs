using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [Header("Menu Root")]
    public GameObject go; // menu ��ü ���⿡ �ֱ�
    private bool activated; // menu ȭ�� Ȱ��ȭ/��Ȱ��ȭ

    private void Start()
    {
        // ���� �� �޴� ��Ȱ��ȭ(�����Ϳ��� �����־ ����)
        SetMenu(false);
    }

    void Update()
    {
        if (!activated && Input.GetKeyDown(KeyCode.Escape))
        {
            SetMenu(true); // Esc ��ư�� ���� ����
        }
    }

    /// <summary>
    /// ��� ��ư: �޴� �ݱ� ����
    /// </summary>
    public void Continue()
    {
        SetMenu(false);
    }

    /// <summary>
    /// Ÿ��Ʋ�� ��ư
    /// </summary>
    public void GoToStartScene()
    {
        // �ʿ��ϸ� �� �̵� �� Ÿ�ӽ����� ����
        // Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }

    /// <summary>
    /// �޴� ����/�ݱ� ���� ó��
    /// </summary>
    private void SetMenu(bool show)
    {
        activated = show;
        if (go != null) go.SetActive(show);

        // ���� �Ͻ�����/����(���ϸ� ���)
        // show�� true�� 0���� ���߰�, false�� 1�� ���
        // Time.timeScale = show ? 0f : 1f;
    }
}
