using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private DialogueLoader dialogueLoader;
    private Dialogue currentDialogue;

    [SerializeField] private DialogueUI dialogueUI;   // UI�� ����
    public bool isDialogue = false;             // ��ȭâ Ȱ��ȭ ����(�⺻: false)
    public bool isChoice = false;                     // ������ Ȱ��ȭ ����(�⺻: false)

    void Start()
    {
        dialogueLoader = FindObjectOfType<DialogueLoader>();

        if (dialogueUI == null)
        {
            Debug.LogError("DialogueUI�� ������� �ʾҽ��ϴ�!");
            return;
        }

        dialogueUI.HideDialogue();
    }

    void Update()
    {
        if (!isChoice && Input.GetKeyDown(KeyCode.Space))
        {
            if (dialogueUI.IsTyping)
                dialogueUI.FinishTyping();  // Ÿ�� ȿ�� ���̸� ��ü ��ȭ ���
            else DisplayNextDialogue();
        }
    }

    // ��ȭ ���
    public void StartDialogue(string startId)
    {
        if (string.IsNullOrEmpty(startId))
        {
            Debug.LogError("���� ID�� �ùٸ��� �ʽ��ϴ�!");
            return;
        }

        isDialogue = true;
        dialogueUI.ShowDialoguePanel();  // ��ȭ ���� �� �г� ǥ��
        DisplayDialogue(startId);
    }

    // Ư�� ID ��ȭ ���
    void DisplayDialogue(string currentId)
    {
        currentDialogue = dialogueLoader.GetDialogueId(currentId);
        if (currentDialogue == null)
        {
            Debug.LogError("��ȭ �����͸� ã�� �� �����ϴ�: " + currentId);
            EndDialogue();
            return;
        }

        dialogueUI.ShowDialogue(currentDialogue.speaker, currentDialogue.dialogue, currentDialogue.portrait);

        // ������ ���� ��� ��ư ǥ��
        if (!string.IsNullOrEmpty(currentDialogue.choice1) && !string.IsNullOrEmpty(currentDialogue.choice2))
        {
            isChoice = true; // ������ ��� �߿��� �����̽��� �Է� ����
            dialogueUI.ShowChoices(currentDialogue.choice1, currentDialogue.choice2);

            dialogueUI.choiceButton1.onClick.RemoveAllListeners();
            dialogueUI.choiceButton2.onClick.RemoveAllListeners();
            dialogueUI.choiceButton1.onClick.AddListener(() => OnChoiceSelected(1));
            dialogueUI.choiceButton2.onClick.AddListener(() => OnChoiceSelected(2));
        }
        else
        {
            dialogueUI.HideChoices(); // �������� ������ ��Ȱ��ȭ
            isChoice = false;
        }
    }

    // ������ ��ư Ŭ���� ȣ��
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

    // ���� ��� ���
    void DisplayNextDialogue()
    {
        // Next ID�� "END"�� ��� ��ȭ ����
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

    // ��ȭ ����
    void EndDialogue()
    {
        isDialogue = false;
        dialogueUI.HideDialogue();
    }
}