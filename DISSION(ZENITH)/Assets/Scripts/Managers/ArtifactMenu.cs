using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // DOTween ���ӽ����̽� �߰�

public class ArtifactMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform menuPanel; // �޴� ��ü
    private Vector2 originalPosition;
    private Vector2 targetPosition;
    private Tween currentTween; // Tweening

    void Start()
    {
        menuPanel = GetComponent<RectTransform>();
        originalPosition = menuPanel.anchoredPosition;
        targetPosition = new Vector2(originalPosition.x, originalPosition.y - 80f);
        // ����ġ���� 80 �Ʒ��� �̵�
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentTween?.Kill(); // ���� Tween�� ���� ���̸� ����
        currentTween = menuPanel.DOAnchorPos(targetPosition, 0.3f).SetEase(Ease.OutQuad);
        // targetPosition���� �̵��ϴ� Tween ����
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        currentTween?.Kill();
        currentTween = menuPanel.DOAnchorPos(originalPosition, 0.3f).SetEase(Ease.OutQuad);
    }
}