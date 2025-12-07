using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [Header("Menu Root")]
    public GameObject go; // menu 객체 여기에 넣기
    public GameObject saveMenu; // saveMenu 객체 여기에 넣기
    private bool activated; // menu 화면 활성화/비활성화

    private void Start()
    {
        // 시작 시 메뉴 비활성화(에디터에서 켜져있어도 꺼줌)
        SetMenu(false);
    }

    void Update()
    {
        // Esc 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (saveMenu.activeSelf)
            {
                saveMenu.SetActive(false);
                return;
            }

            SetMenu(!activated);
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
    /// 세이브 버튼: 세이브 패널 열기
    /// </summary>
    public void SavePanel()
    {
        SetMenu(false);
        saveMenu.SetActive(true);
        SoundManager.Instance.PlaySFX(SfxType.UISelect, 0.5f, false);
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

        // 메뉴 켜질 때 효과음 재생
        if (show == true)
        {
            SoundManager.Instance.PlaySFX(SfxType.UISelect, 0.5f, false);
        }

        // 플레이어 움직임 설정
        if (PlayerController.Instance != null)
        {
            if (show)
            {
                PlayerController.Instance.StopMovement();
            }
            else
            {
                PlayerController.Instance.ResumeMovement();
            }
        }

        // 패널 활성화
        if (go != null) go.SetActive(show);

        // 시간 정지
        Time.timeScale = show ? 0f : 1f;
    }
}
