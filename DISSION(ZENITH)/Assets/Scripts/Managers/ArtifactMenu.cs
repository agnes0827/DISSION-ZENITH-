using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening; // DOTween 네임스페이스 추가

public class ArtifactMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform menuPanel; // 메뉴 객체
    private Vector2 originalPosition;
    private Vector2 targetPosition;
    private Tween currentTween; // Tweening

    [Header("아티팩트 슬롯 관리")]
    [SerializeField] private Transform artifactMenuPanel; // GridLayoutGroup 붙은 패널

    private List<Image> slots;

    void Start()
    {
        menuPanel = GetComponent<RectTransform>();
        originalPosition = menuPanel.anchoredPosition;
        targetPosition = new Vector2(originalPosition.x, originalPosition.y - 80f);
        // 원위치보다 80 아래로 이동

        // 패널 ‘자신’을 제외하고, 바로 아래 자식들만 슬롯으로 수집
        slots = new List<Image>();
        foreach (Transform child in artifactMenuPanel)
        {
            var img = child.GetComponent<Image>();
            if (img != null)
            {
                img.enabled = false;   // 비어있을 때 안 보이게
                slots.Add(img);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentTween?.Kill(); // 기존 Tween이 실행 중이면 종료
        currentTween = menuPanel.DOAnchorPos(targetPosition, 0.3f).SetEase(Ease.OutQuad);
        // targetPosition으로 이동하는 Tween 실행
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        currentTween?.Kill();
        currentTween = menuPanel.DOAnchorPos(originalPosition, 0.3f).SetEase(Ease.OutQuad);
    }

    /// <summary>첫 번째 비어있는 슬롯을 찾아 활성화하고 스프라이트를 채움</summary>
    public bool TryAddArtifact(Sprite sprite)
    {
        foreach (var slot in slots)
        {
            if (!slot.enabled)
            {
                slot.enabled = true;
                slot.sprite = sprite;
                return true;
            }
        }
        Debug.Log("슬롯이 가득 찼습니다!");
        return false;
    }
}