using UnityEngine;
using TMPro;
using DG.Tweening;

public class NoticeUI : MonoBehaviour
{
    [SerializeField] private RectTransform panelTransform; // NoticePanel
    [SerializeField] private TextMeshProUGUI noticeText;

    [Header("설정값")]
    [SerializeField] private float expandDuration = 0.4f;
    [SerializeField] private float displayDuration = 2f;

    private Vector3 hiddenScale = new Vector3(0f, 1f, 1f);
    private Vector3 shownScale = Vector3.one;

    void Awake()
    {
        ShowNotice("테스트");
        panelTransform.localScale = hiddenScale;
    }

    public void ShowNotice(string message)
    {
        noticeText.text = message;

        // 등장 (좌우 확장)
        panelTransform.DOScale(shownScale, expandDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                // 유지
                DOVirtual.DelayedCall(displayDuration, () =>
                {
                    // 사라짐 (다시 좌우 축소)
                    panelTransform.DOScale(hiddenScale, expandDuration)
                        .SetEase(Ease.InBack);
                });
            });
    }
}
