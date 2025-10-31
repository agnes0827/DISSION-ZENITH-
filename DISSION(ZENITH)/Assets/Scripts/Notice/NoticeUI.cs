using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;

public class NoticeUI : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private TextMeshProUGUI noticeText;

    [Header("애니메이션 설정")]
    [SerializeField] private float fadeInDuration = 0.3f;   // 페이드 인
    [SerializeField] private float displayDuration = 2f;    // 유지 시간
    [SerializeField] private float fadeOutDuration = 0.3f;  // 페이드 아웃

    private Sequence currentSequence;

    // 초기화 함수
    // 메시지 설정 후 외부에서 해당 함수 호출
    public void StartNotice(string message)
    {
        if (noticeText == null) return;
        currentSequence?.Kill();

        noticeText.text = message;
        noticeText.alpha = 0f;

        // DOTween 시퀀스 생성
        currentSequence = DOTween.Sequence();

        // 1. 페이드 인
        currentSequence.Append(noticeText.DOFade(1f, fadeInDuration));

        // 2. 유지
        currentSequence.AppendInterval(displayDuration);

        // 3. 페이드 아웃
        currentSequence.Append(noticeText.DOFade(0f, fadeOutDuration));

        // 4. 애니메이션 완료 후 오브젝트 파괴
        currentSequence.AppendCallback(() => {
            if (gameObject != null)
                Destroy(gameObject);
        });

        currentSequence.Play();
    }

    // 오브젝트 파괴 시 코루틴/DOTween 정리
    private void OnDestroy()
    {
        currentSequence?.Kill();
    }
}