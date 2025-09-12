using UnityEngine;
using System;

public class DialogueManager : MonoBehaviour
{
    private Dialogue currentDialogue;

    [SerializeField] private GameObject dialoguePrefab;       // ������
    [SerializeField] private DialogueLoader dialogueLoader;
    private DialogueUI dialogueInstance;                      // �ν��Ͻ�ȭ�� UI

    public bool isDialogue = false;
    public bool isChoice = false;

    public static DialogueManager Instance { get; private set; }

    // �̴ϰ��� ��û �̺�Ʈ �߰�
    public static event Action<GameObject> OnMiniGameRequested;
    private GameObject currentDustObject; // ���� ��ȣ�ۿ� ���� ���� ������Ʈ

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RequestMiniGame(GameObject targetDust)
    {
        OnMiniGameRequested?.Invoke(targetDust);
    }

    void Start()
    {
        if (dialogueLoader == null)
        {
            Debug.LogError("DialogueLoader�� ������� �ʾҽ��ϴ�!");
            return;
        }

        if (dialoguePrefab == null)
        {
            Debug.LogError("Dialogue Prefab�� ������� �ʾҽ��ϴ�!");
            return;
        }

        GameObject instance = Instantiate(dialoguePrefab, GameObject.Find("Canvas").transform);
        dialogueInstance = instance.GetComponent<DialogueUI>();

        if (dialogueInstance == null)
        {
            Debug.LogError("DialogueUI ��ũ��Ʈ�� ã�� �� �����ϴ�!");
            return;
        }

        dialogueInstance.HideDialogue();
    }

    void Update()
    {
        if (!isChoice && Input.GetKeyDown(KeyCode.Space))
        {
            if (dialogueInstance.IsTyping)
                dialogueInstance.FinishTyping();
            else DisplayNextDialogue();
        }
    }

    public void StartDialogue(string startId, GameObject dustObject = null)
    {
        currentDustObject = dustObject;
        isDialogue = true;
        dialogueInstance.ShowDialoguePanel();
        DisplayDialogue(startId);
    }

    public void DisplayDialogue(string currentId)
    {
        currentDialogue = dialogueLoader.GetDialogueId(currentId);
        if (currentDialogue == null)
        {
            Debug.LogError("��ȭ �����͸� ã�� �� �����ϴ�: " + currentId);
            EndDialogue();
            return;
        }

        dialogueInstance.ShowDialogue(currentDialogue.speaker, currentDialogue.dialogue, currentDialogue.portrait);

        if (!string.IsNullOrEmpty(currentDialogue.choice1) && !string.IsNullOrEmpty(currentDialogue.choice2))
        {
            isChoice = true;
            dialogueInstance.ShowChoices(currentDialogue.choice1, currentDialogue.choice2);

            dialogueInstance.choiceButton1.onClick.RemoveAllListeners();
            dialogueInstance.choiceButton2.onClick.RemoveAllListeners();
            dialogueInstance.choiceButton1.onClick.AddListener(() => OnChoiceSelected(1));
            dialogueInstance.choiceButton2.onClick.AddListener(() => OnChoiceSelected(2));
        }
        else
        {
            dialogueInstance.HideChoices();
            isChoice = false;
        }

        // ����Ʈ ó��
        if (!string.IsNullOrEmpty(currentDialogue.questId))
        {
            if (!QuestManager.Instance.HasAccepted(currentDialogue.questId))
            {
                QuestManager.Instance.AcceptQuest(currentDialogue.questId);
                Debug.Log($"����Ʈ ������: {currentDialogue.questId}");
            }
        }
    }

    public void OnChoiceSelected(int choiceNumber)
    {
        if (currentDialogue == null)
            return;

        string nextId = (choiceNumber == 1) ? currentDialogue.choice1NextId : currentDialogue.choice2NextId;

        dialogueInstance.HideChoices();
        isDialogue = true;

        // �̴ϰ��� ���� �̺�Ʈ ����
        if (nextId == "MINIGAME")
        {
            Debug.Log("�̴ϰ��� ����");
            EndDialogue();
            OnMiniGameRequested?.Invoke(currentDustObject);
            return;
        }

        DisplayDialogue(nextId);
    }

    void DisplayNextDialogue()
    {
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

    void EndDialogue()
    {
        dialogueInstance.HideDialogue();
        isDialogue = false;
    }
}
