using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // DOTween ���ӽ����̽� �߰�

public class ArtifactMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector2 targetPosition;
    private Tween currentTween; // Tweening

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        targetPosition = new Vector2(originalPosition.x, originalPosition.y - 80f);
        // ����ġ���� 80 �Ʒ��� �̵�
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentTween?.Kill(); // ���� Tween�� ���� ���̸� ����
        currentTween = rectTransform.DOAnchorPos(targetPosition, 0.3f).SetEase(Ease.OutQuad);
        // targetPosition���� �̵��ϴ� Tween ����
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        currentTween?.Kill();
        currentTween = rectTransform.DOAnchorPos(originalPosition, 0.3f).SetEase(Ease.OutQuad);
    }
}