using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [Header("UI 구성 요소")]
    [SerializeField] private GameObject dialogueContainer;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image portraitImage;
    [SerializeField] private GameObject speakerPanel;
    [SerializeField] private RectTransform choicePanelRectTransform;
    [SerializeField] private RectTransform dialogueTextRectTransform;

    [Header("선택지 UI")]
    public GameObject choicePanel;
    public Button choiceButton1;
    public Button choiceButton2;
    public TextMeshProUGUI choiceText1;
    public TextMeshProUGUI choiceText2;
    public GameObject choice1Arrow;
    public GameObject choice2Arrow;

    private Vector2 originalAnchorMin;
    private Vector2 originalAnchorMax;
    private Vector2 originalOffsetMin;
    private Vector2 originalOffsetMax;

    // 타자(타이핑) 효과: 한 글자씩 출력
    [SerializeField] private float typingSpeed = 0.05f;       // 1글자 출력 간격(초)
    private Coroutine typingCoroutine;                        // 타자 효과 코루틴
    public bool IsTyping { get; private set; } = false;       // 타자 효과 진행 중 여부
    private string currentFullText = "";                      // 전체 문장(FullText) 저장

    private void Awake()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.RegisterDialogueUI(this);
        }
        else
        {
            Debug.LogError("DialogueManager가 씬에 없습니다! UI를 등록할 수 없습니다.");
        }

        if (dialogueTextRectTransform != null)
        {
            originalAnchorMin = dialogueTextRectTransform.anchorMin;
            originalAnchorMax = dialogueTextRectTransform.anchorMax;
            originalOffsetMin = dialogueTextRectTransform.offsetMin;
            originalOffsetMax = dialogueTextRectTransform.offsetMax;
        }
    }

    private void OnDestroy()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.UnregisterDialogueUI();
        }
    }

    public void ShowDialoguePanel()
    {
        dialogueContainer.SetActive(true);
    }

    public void HideDialoguePanel()
    {
        dialogueContainer.SetActive(false);
    }

    // 화자, 대사, 초상화 파일 이름 UI 표시
    public void ShowDialogue(string speaker, string dialogue, string portraitName)
    {
        if (string.IsNullOrEmpty(portraitName))
        {
            // 1. 시스템 메시지(or 혼잣말)
            portraitImage.gameObject.SetActive(false);
            speakerPanel.SetActive(false);

            // 대화 텍스트를 중앙 정렬
            dialogueText.alignment = TextAlignmentOptions.Center;

            dialogueTextRectTransform.anchorMin = new Vector2(0, 0);
            dialogueTextRectTransform.anchorMax = new Vector2(1, 1);
            dialogueTextRectTransform.offsetMin = new Vector2(20, 20);
            dialogueTextRectTransform.offsetMax = new Vector2(-20, -20);
        }
        else
        {
            // 2. 일반 캐릭터 대화 모드
            SetPortrait(portraitName);

            // 이름 패널 설정
            if (string.IsNullOrEmpty(speaker))
            {
                speakerPanel.SetActive(false);
            }
            else
            {
                speakerPanel.SetActive(true);
                speakerText.text = speaker;
            }

            // 대화 텍스트를 왼쪽 정렬
            dialogueText.alignment = TextAlignmentOptions.Left;

            // 대화 텍스트의 RectTransform을 원래 값으로 복원
            dialogueTextRectTransform.anchorMin = originalAnchorMin;
            dialogueTextRectTransform.anchorMax = originalAnchorMax;
            dialogueTextRectTransform.offsetMin = originalOffsetMin;
            dialogueTextRectTransform.offsetMax = originalOffsetMax;
        }

        // 타자 효과
        currentFullText = dialogue;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        // TMP 방식: 원문(텍스트 태그 포함)을 먼저 세팅
        dialogueText.richText = true;               
        dialogueText.text = currentFullText;
        dialogueText.ForceMeshUpdate();            
        dialogueText.maxVisibleCharacters = 0;     
        typingCoroutine = StartCoroutine(TypeSentenceTMP());
    }

    // 초상화 설정
    private void SetPortrait(string portraitName)
    {
        if (portraitImage == null)
            return;

        if (string.IsNullOrEmpty(portraitName))
        {
            portraitImage.gameObject.SetActive(false); // 안 보이게
            return;
        }

        Sprite portrait = Resources.Load<Sprite>("Portraits/" + portraitName);

        if (portrait == null)
        {
            portraitImage.gameObject.SetActive(false); // 로드 실패 시 숨김
        }
        else
        {
            portraitImage.sprite = portrait;
            portraitImage.gameObject.SetActive(true);  // 정상 로드 시 보여줌
        }
    }

    // TMP 타자 효과 코루틴
    private IEnumerator TypeSentenceTMP()
    {
        IsTyping = true;
        int total = dialogueText.textInfo.characterCount;

        if (total == 0)
        {
            yield return null;
            dialogueText.ForceMeshUpdate();
            total = dialogueText.textInfo.characterCount;
        }

        int visible = 0;
        while (visible < total)
        {
            visible++;
            dialogueText.maxVisibleCharacters = visible;
            yield return new WaitForSeconds(typingSpeed);
        }

        IsTyping = false;
    }

    // 타자 효과 중 스페이스 바 입력 시,
    // 효과 즉시 종료하고 전체 문장 출력
    public void FinishTyping()
    {
        if (IsTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            dialogueText.ForceMeshUpdate();
            dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;

            IsTyping = false;
        }
    }

    public void ShowChoices(string c1, string c2)
    {
        choicePanel.SetActive(true);
        choiceText1.text = c1;
        choiceText2.text = c2;

        LayoutRebuilder.ForceRebuildLayoutImmediate(choicePanelRectTransform);
    }

    public void HideChoices()
    {
        choicePanel.SetActive(false);
    }

    public void HighlightChoice(int index)
    {
        Color highlightColor = Color.gray;
        Color normalColor = Color.black;

        choiceButton1.GetComponent<Image>().color = (index == 1) ? highlightColor : normalColor;
        choiceButton2.GetComponent<Image>().color = (index == 2) ? highlightColor : normalColor;

        choice1Arrow.SetActive(index == 1);
        choice2Arrow.SetActive(index == 2);
    }
}
