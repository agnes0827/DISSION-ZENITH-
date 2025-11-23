using System.Collections;
using UnityEngine;
using Cinemachine;

public class PurificationCutsceneManager : MonoBehaviour
{
    [Header("조건 설정")]
    public string bossId = "LibraryBoss";

    [Header("오염 구역")]
    public PollutionController pollutionFake1F; // 구석 1층 그림 위 먼지
    public PollutionController pollutionReal2F; // 실제 맵 먼지 (플레이어 주변)
    // pollutionFake2F는 이제 필요 없음!

    [Header("시네머신 카메라")]
    public CinemachineVirtualCamera vcamPlayer;
    public CinemachineVirtualCamera vcamView1F;
    // vcamView2F는 이제 필요 없음!

    [Header("시간 설정")]
    public float startDelay = 1.0f;       // 시작 전 대기
    public float cleanDuration = 4.0f;    // 먼지 사라지는 시간 (넉넉하게)
    public float fadeDuration = 1.0f;     // 암전 시간

    private CinemachineBrain _brain;      // 카메라 감독 (블렌딩 제어용)

    void Start()
    {
        if (GameStateManager.Instance == null) return;

        if (Camera.main != null)
            _brain = Camera.main.GetComponent<CinemachineBrain>();

        if (GameStateManager.Instance.defeatedMonsterIds.Contains(bossId)
            && !GameStateManager.Instance.isLibraryPurified)
        {
            StartCoroutine(PlaySequence());
        }
        else if (GameStateManager.Instance.isLibraryPurified)
        {
            DisableAllPollutionImmediate();
        }
    }

    IEnumerator PlaySequence()
    {
        Debug.Log("정화 컷신 시작 (2층 리얼타임 버전)");

        if (PlayerController.Instance != null) PlayerController.Instance.StopMovement();

        // ====================================================
        // [장면 1] 1층 그림 보여주기
        // ====================================================
        vcamPlayer.Priority = 0;
        vcamView1F.Priority = 100; // 1층 카메라 ON

        yield return new WaitForSeconds(startDelay);

        // 1층 먼지 정화
        if (pollutionFake1F != null)
        {
            pollutionFake1F.StartPurification();
            yield return new WaitForSeconds(cleanDuration); // 사라지는 거 감상
        }

        // ====================================================
        // [장면 2] 플레이어로 복귀 (암전 이용)
        // ====================================================

        // 1. 화면 어둡게 (Fade Out)
        yield return FadeManager.Instance.FadeOut(fadeDuration);

        CinemachineBlendDefinition.Style originalStyle = CinemachineBlendDefinition.Style.EaseInOut;
        if (_brain != null)
        {
            originalStyle = _brain.m_DefaultBlend.m_Style; // 원래 설정 저장
            _brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;

        }
        // 2. 안 보이는 동안 카메라를 플레이어로 변경
        vcamView1F.Priority = 0;
        vcamPlayer.Priority = 100;

        yield return new WaitForSeconds(0.5f); // 잠깐 숨 고르기

        // 3. 화면 밝히기 (Fade In)
        // 플레이어와 아직 더러운 2층 맵이 보임
        FadeManager.Instance.FadeIn(fadeDuration);

        // 화면이 밝아지는 시간만큼 기다림
        yield return new WaitForSeconds(fadeDuration);

        // ====================================================
        // [장면 3] 2층 실제 정화 (눈앞에서 보여줌)
        // ====================================================

        // 4. 이제 내 눈앞에서 먼지가 사라짐!
        if (pollutionReal2F != null)
        {
            pollutionReal2F.StartPurification();
            yield return new WaitForSeconds(cleanDuration); // 사라지는 거 감상
        }

        GameStateManager.Instance.isLibraryPurified = true;

        if (_brain != null)
        {
            _brain.m_DefaultBlend.m_Style = originalStyle;
        }

        if (PlayerController.Instance != null)
            PlayerController.Instance.transform.GetComponent<PlayerController>().enabled = true;

        DisableAllPollutionImmediate();
        if (PlayerController.Instance != null) PlayerController.Instance.ResumeMovement();
        Debug.Log("정화 컷신 종료");
    }

    void DisableAllPollutionImmediate()
    {
        if (pollutionFake1F != null) pollutionFake1F.SetPurifiedImmediate();
        if (pollutionReal2F != null) pollutionReal2F.SetPurifiedImmediate();
    }
}