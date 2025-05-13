using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private DialogueLoader dialogueLoader;
    [SerializeField] private DialogueUI dialogueUI;  // GameObject로 설정
    private Dialogue currentDialogue;
    private bool isDialogueActive = false;

    void Start()
    {
        dialogueLoader = FindObjectOfType<DialogueLoader>();

        // 대화 시작 시 패널이 꺼져 있으면 활성화
        if (dialogueUI == null)
        {
            Debug.LogError("DialogueUI가 연결되지 않았습니다!");
            return;
        }

        dialogueUI.HideDialogue();
    }

    void Update()
    {
        // 스페이스바로 대화 넘기기
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextDialogue();
        }
    }

    public void StartDialogue(string startId)
    {
        if (string.IsNullOrEmpty(startId))
        {
            Debug.LogError("시작 ID가 올바르지 않습니다!");
            return;
        }

        isDialogueActive = true;
        dialogueUI.ShowDialoguePanel();  // 대화 시작 시 패널 표시
        DisplayDialogue(startId);
    }

    void DisplayDialogue(string currentId)
    {
        currentDialogue = dialogueLoader.GetDialogueById(currentId);
        if (currentDialogue == null)
        {
            Debug.LogError("대화 데이터를 찾을 수 없습니다: " + currentId);
            EndDialogue();
            return;
        }

        dialogueUI.ShowDialogue(currentDialogue.speaker, currentDialogue.dialogue, currentDialogue.portrait);

        // 선택지 있을 경우에만 보여줌
        if (!string.IsNullOrEmpty(currentDialogue.choice1) && !string.IsNullOrEmpty(currentDialogue.choice2))
        {
            isDialogueActive = false;
            dialogueUI.ShowChoices(currentDialogue.choice1, currentDialogue.choice2);

            dialogueUI.choiceButton1.onClick.RemoveAllListeners();
            dialogueUI.choiceButton2.onClick.RemoveAllListeners();
            dialogueUI.choiceButton1.onClick.AddListener(() => OnChoiceSelected(1));
            dialogueUI.choiceButton2.onClick.AddListener(() => OnChoiceSelected(2));
        }
        else
        {
            dialogueUI.HideChoices(); // 선택지가 없으면 무조건 비활성화
            isDialogueActive = true;
        }
    }


    public void OnChoiceSelected(int choiceNumber)
    {
        string nextId = (choiceNumber == 1) ? currentDialogue.choice1NextId : currentDialogue.choice2NextId;

        dialogueUI.HideChoices();
        isDialogueActive = true;
        DisplayDialogue(nextId);
    }


    void DisplayNextDialogue()
    {
        if (currentDialogue.nextId == "END")
        {
            EndDialogue();
            return;
        }

        // 다음 ID가 비어있다면 다음 행을 출력
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

    void EndDialogue()
    {
        isDialogueActive = false;
        dialogueUI.HideDialogue();
    }
}