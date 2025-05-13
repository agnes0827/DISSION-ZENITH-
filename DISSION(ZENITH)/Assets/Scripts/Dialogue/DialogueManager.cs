using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private DialogueLoader dialogueLoader;
    [SerializeField] private DialogueUI dialogueUI;  // GameObject�� ����
    private Dialogue currentDialogue;
    private bool isDialogueActive = false;

    void Start()
    {
        dialogueLoader = FindObjectOfType<DialogueLoader>();

        // ��ȭ ���� �� �г��� ���� ������ Ȱ��ȭ
        if (dialogueUI == null)
        {
            Debug.LogError("DialogueUI�� ������� �ʾҽ��ϴ�!");
            return;
        }

        dialogueUI.HideDialogue();
    }

    void Update()
    {
        // �����̽��ٷ� ��ȭ �ѱ��
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextDialogue();
        }
    }

    public void StartDialogue(string startId)
    {
        if (string.IsNullOrEmpty(startId))
        {
            Debug.LogError("���� ID�� �ùٸ��� �ʽ��ϴ�!");
            return;
        }

        isDialogueActive = true;
        dialogueUI.ShowDialoguePanel();  // ��ȭ ���� �� �г� ǥ��
        DisplayDialogue(startId);
    }

    void DisplayDialogue(string currentId)
    {
        currentDialogue = dialogueLoader.GetDialogueById(currentId);
        if (currentDialogue == null)
        {
            Debug.LogError("��ȭ �����͸� ã�� �� �����ϴ�: " + currentId);
            EndDialogue();
            return;
        }

        dialogueUI.ShowDialogue(currentDialogue.speaker, currentDialogue.dialogue, currentDialogue.portrait);

        // ������ ���� ��쿡�� ������
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
            dialogueUI.HideChoices(); // �������� ������ ������ ��Ȱ��ȭ
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

        // ���� ID�� ����ִٸ� ���� ���� ���
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