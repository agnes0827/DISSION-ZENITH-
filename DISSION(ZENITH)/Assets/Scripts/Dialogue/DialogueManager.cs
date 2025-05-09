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

        dialogueUI.ShowDialogue(currentDialogue.speaker, currentDialogue.dialogue);
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
