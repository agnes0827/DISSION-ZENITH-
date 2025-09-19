using UnityEngine;
using TMPro;
using DG.Tweening;

public class NoticeUI : MonoBehaviour
{
    [SerializeField] private RectTransform panelTransform; // NoticePanel
    [SerializeField] private TextMeshProUGUI noticeText;

    [Header("������")]
    [SerializeField] private float expandDuration = 0.4f;
    [SerializeField] private float displayDuration = 2f;

    private Vector3 hiddenScale = new Vector3(0f, 1f, 1f);
    private Vector3 shownScale = Vector3.one;

    void Awake()
    {
        ShowNotice("�׽�Ʈ");
        panelTransform.localScale = hiddenScale;
    }

    public void ShowNotice(string message)
    {
        noticeText.text = message;

        // ���� (�¿� Ȯ��)
        panelTransform.DOScale(shownScale, expandDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                // ����
                DOVirtual.DelayedCall(displayDuration, () =>
                {
                    // ����� (�ٽ� �¿� ���)
                    panelTransform.DOScale(hiddenScale, expandDuration)
                        .SetEase(Ease.InBack);
                });
            });
    }
}
