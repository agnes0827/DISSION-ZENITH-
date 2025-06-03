using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private DialogueLoader dialogueLoader;
    private Dialogue currentDialogue;

    [SerializeField] private DialogueUI dialogueUI;   // UI와 연결
    public bool isDialogue = false;             // 대화창 활성화 여부(기본: false)
    public bool isChoice = false;                     // 선택지 활성화 여부(기본: false)

    void Start()
    {
        dialogueLoader = FindObjectOfType<DialogueLoader>();

        if (dialogueUI == null)
        {
            Debug.LogError("DialogueUI가 연결되지 않았습니다!");
            return;
        }

        dialogueUI.HideDialogue();
    }

    void Update()
    {
        if (!isChoice && Input.GetKeyDown(KeyCode.Space))
        {
            if (dialogueUI.IsTyping)
                dialogueUI.FinishTyping();  // 타자 효과 중이면 전체 대화 출력
            else DisplayNextDialogue();
        }
    }

    // 대화 출력
    public void StartDialogue(string startId)
    {
        if (string.IsNullOrEmpty(startId))
        {
            Debug.LogError("시작 ID가 올바르지 않습니다!");
            return;
        }

        isDialogue = true;
        dialogueUI.ShowDialoguePanel();  // 대화 시작 시 패널 표시
        DisplayDialogue(startId);
    }

    // 특정 ID 대화 출력
    void DisplayDialogue(string currentId)
    {
        currentDialogue = dialogueLoader.GetDialogueId(currentId);
        if (currentDialogue == null)
        {
            Debug.LogError("대화 데이터를 찾을 수 없습니다: " + currentId);
            EndDialogue();
            return;
        }

        dialogueUI.ShowDialogue(currentDialogue.speaker, currentDialogue.dialogue, currentDialogue.portrait);

        // 선택지 있을 경우 버튼 표시
        if (!string.IsNullOrEmpty(currentDialogue.choice1) && !string.IsNullOrEmpty(currentDialogue.choice2))
        {
            isChoice = true; // 선택지 출력 중에는 스페이스바 입력 방지
            dialogueUI.ShowChoices(currentDialogue.choice1, currentDialogue.choice2);

            dialogueUI.choiceButton1.onClick.RemoveAllListeners();
            dialogueUI.choiceButton2.onClick.RemoveAllListeners();
            dialogueUI.choiceButton1.onClick.AddListener(() => OnChoiceSelected(1));
            dialogueUI.choiceButton2.onClick.AddListener(() => OnChoiceSelected(2));
        }
        else
        {
            dialogueUI.HideChoices(); // 선택지가 없으면 비활성화
            isChoice = false;
        }
    }

    // 선택지 버튼 클릭시 호출
    public void OnChoiceSelected(int choiceNumber)
    {
        if (currentDialogue == null)
        {
            return;
        }

        string nextId = (choiceNumber == 1) ? currentDialogue.choice1NextId : currentDialogue.choice2NextId;

        dialogueUI.HideChoices();
        isDialogue = true;
        DisplayDialogue(nextId);
    }

    // 다음 대사 출력
    void DisplayNextDialogue()
    {
        // Next ID가 "END"일 경우 대화 종료
        if (currentDialogue.nextId == "END")
        {
            EndDialogue();
            return;
        }

        if (string.IsNullOrEmpty(currentDialogue.nextId))
        {
            int nextIndex = int.Parse(currentDialogue.id) + 1;
            DisplayDialogue(nextIndex.ToString());
        }
        else
        {
            DisplayDialogue(currentDialogue.nextId);
        }
    }

    // 대화 종료
    void EndDialogue()
    {
        isDialogue = false;
        dialogueUI.HideDialogue();
    }
}