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
    private bool initialized;

    private void Awake()
    {
        Init(); // ← Awake에서 초기화
    }

    private void Init()
    {
        if (initialized) return;
        menuPanel = GetComponent<RectTransform>();
        originalPosition = menuPanel.anchoredPosition;
        targetPosition = new Vector2(originalPosition.x, originalPosition.y - 80f);

        slots = new List<Image>();
        foreach (Transform child in artifactMenuPanel)
        {
            var img = child.GetComponent<Image>();
            if (img != null)
            {
                img.enabled = false; // 비어있을 때 숨김
                slots.Add(img);
            }
        }
        initialized = true;
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

    public void Open()
    {
        gameObject.SetActive(true);
        Init();
        currentTween?.Kill();
        currentTween = menuPanel.DOAnchorPos(targetPosition, 0.25f).SetEase(Ease.OutQuad);
    }
}