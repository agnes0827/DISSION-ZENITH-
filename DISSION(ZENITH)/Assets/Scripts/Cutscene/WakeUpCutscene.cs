using UnityEngine;
using Cinemachine;
using System.Collections;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class WakeUpCutscene : MonoBehaviour
{
    [Header("설정")]
    [Tooltip("이 연출을 한 번만 보기 위한 이벤트 ID")]
    public string eventId = "Event_WakeUp";
    [Tooltip("n초 뒤에 띄울 안내 텍스트 오브젝트")]
    public GameObject wakeUpGuideUI;

    [Header("연출 효과")]
    [Tooltip("잠자는 동안 켜둘 비네트(가장자리 어두운) 이미지")]
    public GameObject vignetteOverlay;
    [Tooltip("잠에서 깰 때 깜빡일 검은색 전체 화면 이미지")]
    public Image blackFadeScreen;
    [Tooltip("일어난 후 실행할 알림판 스크립트")]
    public NoticePanel noticePanel;

    [Header("오브젝트 연결")]
    [Tooltip("눈 감은 플레이어 이미지")]
    public GameObject sleepingPlayerObject;
    [Tooltip("일어났을 때 플레이어가 서 있을 위치")]
    public Transform wakeUpPosition;

    [Header("카메라 세팅")]
    public CinemachineVirtualCamera normalCam; // 평소 카메라
    public CinemachineVirtualCamera sleepCam;  // 줌인 + 흔들림 카메라
    public PixelPerfectCamera pixelPerfectCam;

    [Header("퀘스트")]
    public string questIdToAccept = "Q02";

    private GameObject _realPlayer;
    private bool _isWakingUp = false;

    IEnumerator Start()
    {
        if (_realPlayer == null)
            _realPlayer = GameObject.FindGameObjectWithTag("Player");

        if (GameStateManager.Instance.HasExecutedEvent(eventId))
        {
            CleanupScene();
            Destroy(gameObject);
            yield break;
        }

        // 초기 상태 세팅 (잠든 상태)
        if (pixelPerfectCam != null)
        {
            pixelPerfectCam.enabled = false;
        }
        else
        {
            if (Camera.main != null)
            {
                pixelPerfectCam = Camera.main.GetComponent<PixelPerfectCamera>();
                if (pixelPerfectCam != null) pixelPerfectCam.enabled = false;
            }
        }

        // 플레이어 숨기기
        if (_realPlayer != null) _realPlayer.SetActive(false);

        // 침대 위 플레이어 스프라이트 켜기
        if (vignetteOverlay != null) vignetteOverlay.SetActive(true);
        if (sleepingPlayerObject != null) sleepingPlayerObject.SetActive(true);

        // sleepCam 우선순위 높임
        if (sleepCam != null) sleepCam.Priority = 20;
        if (normalCam != null) normalCam.Priority = 10;

        if (wakeUpGuideUI != null) wakeUpGuideUI.SetActive(false);

        // 대기
        yield return new WaitForSeconds(4.2f);

        // 입력 대기
        if (wakeUpGuideUI != null) wakeUpGuideUI.SetActive(true);

        while (!_isWakingUp)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            if (h != 0 || v != 0) _isWakingUp = true;
            yield return null;
        }

        // 기상 연출
        if (wakeUpGuideUI != null) wakeUpGuideUI.SetActive(false);

        // 페이드
        yield return StartCoroutine(FadeRoutine(0f, 1f, 1.5f));

        // 플레이어 교체
        if (sleepingPlayerObject != null) sleepingPlayerObject.SetActive(false);
        if (vignetteOverlay != null) vignetteOverlay.SetActive(false); // 비네트 off

        if (_realPlayer != null)
        {
            if (wakeUpPosition != null)
                _realPlayer.transform.position = wakeUpPosition.position;

            _realPlayer.SetActive(true);
            var controller = _realPlayer.GetComponent<PlayerController>();
            if (controller != null) controller.ResumeMovement();
        }

        // 카메라 복귀(MainCamera), 픽셀 퍼펙트 복구
        if (sleepCam != null) sleepCam.Priority = 0;
        if (pixelPerfectCam != null) pixelPerfectCam.enabled = true;

        yield return StartCoroutine(FadeRoutine(1f, 0f, 2.5f));

        // 6. 퀘스트 수락 및 저장
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.AcceptQuest(questIdToAccept);
        }
        GameStateManager.Instance.SetEventExecuted(eventId);
        DialogueManager.Instance.StartDialogue("10000");

        if (noticePanel != null) noticePanel.ShowNotice();
    }

    // 페이드 효과용 코루틴
    IEnumerator FadeRoutine(float startAlpha, float endAlpha, float duration)
    {
        if (blackFadeScreen == null) yield break;

        float time = 0f;
        SetAlpha(startAlpha);

        while (time < duration)
        {
            time += Time.deltaTime;
            float a = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            SetAlpha(a);
            yield return null;
        }
        SetAlpha(endAlpha);
    }

    void SetAlpha(float alpha)
    {
        if (blackFadeScreen != null)
        {
            Color c = blackFadeScreen.color;
            c.a = alpha;
            blackFadeScreen.color = c;
        }
    }

    void CleanupScene()
    {
        if (sleepingPlayerObject != null) sleepingPlayerObject.SetActive(false);
        if (wakeUpGuideUI != null) wakeUpGuideUI.SetActive(false);
        if (vignetteOverlay != null) vignetteOverlay.SetActive(false);
        if (blackFadeScreen != null) blackFadeScreen.gameObject.SetActive(false);
    }
}