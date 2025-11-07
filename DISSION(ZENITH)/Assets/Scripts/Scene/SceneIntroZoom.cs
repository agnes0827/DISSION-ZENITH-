using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SceneIntroZoom : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera vcam;
    [SerializeField] Transform axe;

    [Header("Timing")]
    [SerializeField] float delayAfterLoad = 2f; // 씬 로드 후 기다릴 시간

    [SerializeField] float focusZoom = 6f;   // 도끼에 비출 때 줌(작을수록 더 확대)
    [SerializeField] float focusHold = 1.0f; // 도끼를 비추는 시간
    [SerializeField] float blendTime = 0.6f; // 줌 인/아웃 보간 시간

    float originalSize;
    Transform originalFollow;

    IEnumerator Start()
    {
        if (!vcam) vcam = FindObjectOfType<CinemachineVirtualCamera>();
        originalSize = vcam.m_Lens.OrthographicSize;
        originalFollow = vcam.Follow;

        // 씬 로드 후 2초 기다렸다가 연출 시작
        yield return new WaitForSecondsRealtime(delayAfterLoad);
        PlayerController.Instance.StopMovement();

        // 도끼로 전환 + 줌 인
        vcam.Follow = axe;
        yield return LerpSize(originalSize, focusZoom, blendTime);

        // 잠시 유지
        yield return new WaitForSeconds(focusHold);

        // 플레이어로 복귀 + 줌 아웃
        vcam.Follow = originalFollow; // <- null일 수 있는 player 변수 대신, 처음에 저장한 originalFollow로 복구
        yield return LerpSize(focusZoom, originalSize, blendTime);
        PlayerController.Instance.ResumeMovement();
    }

    IEnumerator LerpSize(float from, float to, float dur)
    {
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(from, to, Mathf.SmoothStep(0, 1, t / dur));
            yield return null;
        }
        vcam.m_Lens.OrthographicSize = to;
    }
}
