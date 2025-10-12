using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [Header("Menu Root")]
    public GameObject go; // menu 객체 여기에 넣기
    private bool activated; // menu 화면 활성화/비활성화

    private void Start()
    {
        // 시작 시 메뉴 비활성화(에디터에서 켜져있어도 꺼줌)
        SetMenu(false);
    }

    void Update()
    {
        if (!activated && Input.GetKeyDown(KeyCode.Escape))
        {
            SetMenu(true); // Esc 버튼은 열기 전용
        }
    }

    /// <summary>
    /// 계속 버튼: 메뉴 닫기 전용
    /// </summary>
    public void Continue()
    {
        SetMenu(false);
    }

    /// <summary>
    /// 타이틀로 버튼
    /// </summary>
    public void GoToStartScene()
    {
        // 필요하면 씬 이동 전 타임스케일 복구
        // Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }

    /// <summary>
    /// 메뉴 열기/닫기 공통 처리
    /// </summary>
    private void SetMenu(bool show)
    {
        activated = show;
        if (go != null) go.SetActive(show);

        // 게임 일시정지/해제(원하면 사용)
        // show가 true면 0으로 멈추고, false면 1로 재생
        // Time.timeScale = show ? 0f : 1f;
    }
}
