using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    // 기본 대화 UI
    [SerializeField] private Text speakerText;     // 화자(캐릭터 이름)
    [SerializeField] private Text dialogueText;    // 대사 텍스트
    [SerializeField] private Image portraitImage;  // 초상화 이미지
    [SerializeField] private GameObject speakerPanel; // 이름 박스 전체 패널

    // 선택지 UI
    public GameObject choicePanel;
    public Button choiceButton1;
    public Button choiceButton2;
    public Text choiceText1;
    public Text choiceText2;

    // 타자(타이핑) 효과: 한 글자씩 출력
    [SerializeField] private float typingSpeed = 0.05f;     // 타이핑 속도(1글자 출력 간격)
    private Coroutine typingCoroutine;                      // 타자 효과 코루틴
    public bool IsTyping { get; private set; } = false;     // 타자 효과 진행 중 여부
    private string currentFullText = "";                    // 전체 문장(FullText) 저장

    // 화자, 대사, 초상화 파일 이름 UI 표시
    public void ShowDialogue(string speaker, string dialogue, string portraitName)
    {
        // 이름이 비어있으면 이름 UI 숨기기
        if (string.IsNullOrEmpty(speaker))
        {
            speakerPanel.SetActive(false);
        }
        else
        {
            speakerPanel.SetActive(true);
            speakerText.text = speaker;
        }

        currentFullText = dialogue;

        // 타자 효과 중이면 중지
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentence(dialogue));

        // 초상화 설정
        SetPortrait(portraitName);
    }


    // 초상화 설정
    private void SetPortrait(string portraitName)
    {
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
            portraitImage.gameObject.SetActive(true); // 정상 로드 시 보여줌
        }
    }


    // 타자 효과 코루틴
    private IEnumerator TypeSentence(string sentence)
    {
        IsTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
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

            dialogueText.text = currentFullText;
            IsTyping = false;
        }
    }

    public void ShowDialoguePanel()
    {
        gameObject.SetActive(true);
    }

    public void HideDialogue()
    {
        gameObject.SetActive(false);
    }

    public void ShowChoices(string c1, string c2)
    {
        choicePanel.SetActive(true);
        choiceText1.text = c1;
        choiceText2.text = c2;
    }

    public void HideChoices()
    {
        choicePanel.SetActive(false);
    }
}