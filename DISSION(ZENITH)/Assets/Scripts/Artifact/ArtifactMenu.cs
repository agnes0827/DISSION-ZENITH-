using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ArtifactMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform menuPanel; // 메뉴 객체
    private Vector2 originalPosition;
    private Vector2 targetPosition;
    private Tween currentTween;

    [Header("아티팩트 슬롯 관리")]
    [SerializeField] private Transform artifactMenuPanel; // GridLayoutGroup 붙은 패널
    private List<Image> slots = new List<Image>();
    private bool initialized = false;

    [Header("데이터 연결")]
    [SerializeField] private ArtifactDatabase artifactDatabase;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        // 씬 시작 시 GameStateManager 데이터를 기반으로 UI 채우기
        RepopulateUI();
    }

    private void Init()
    {
        if (initialized) return;

        menuPanel = GetComponent<RectTransform>();
        if (menuPanel != null)
        {
            originalPosition = menuPanel.anchoredPosition;
            targetPosition = new Vector2(originalPosition.x, originalPosition.y - 80f);
        }
        else
        {
            Debug.LogError("ArtifactMenu: RectTransform 컴포넌트를 찾을 수 없습니다!");
        }


        slots.Clear();
        if (artifactMenuPanel != null)
        {
            foreach (Transform child in artifactMenuPanel)
            {
                var img = child.GetComponent<Image>();
                if (img != null)
                {
                    img.enabled = false; // 기본 숨김
                    img.sprite = null;
                    slots.Add(img);
                }
            }
        }
        else
        {
            Debug.LogError("ArtifactMenu: Artifact Menu Panel이 연결되지 않았습니다!");
        }
        initialized = true;
    }

    public void RepopulateUI()
    {
        if (!initialized) Init();

        if (GameStateManager.Instance == null || artifactDatabase == null)
        {
            Debug.LogError("ArtifactMenu: GameStateManager 또는 ArtifactDatabase를 찾을 수 없습니다!");
            foreach (var slot in slots) { slot.enabled = false; slot.sprite = null; }
            return;
        }
        Debug.Log("ArtifactMenu: GameState 기준으로 UI 다시 채우기...");

        // 1. 슬롯 초기화
        foreach (var slot in slots) { slot.enabled = false; slot.sprite = null; }

        // 2. GameStateManager에서 ID 목록 가져오기
        List<string> collectedIDs = GameStateManager.Instance.collectedArtifactIDs;

        // 3. ID 목록 순서대로 슬롯 채우기 (GetArtifactByID 사용)
        int slotIndex = 0;
        foreach (string artifactId in collectedIDs)
        {
            if (slotIndex >= slots.Count) break;

            ArtifactDefinition artifactDef = artifactDatabase.GetArtifactByID(artifactId);

            if (artifactDef != null && artifactDef.artifactSprite != null)
            {
                slots[slotIndex].sprite = artifactDef.artifactSprite;
                slots[slotIndex].enabled = true;
                slotIndex++;
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
        Debug.LogWarning("TryAddArtifact 직접 호출보다는 RepopulateUI() 사용 권장.");
        // ... (기존 로직 유지 가능하나, RepopulateUI와 중복될 수 있음) ...
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