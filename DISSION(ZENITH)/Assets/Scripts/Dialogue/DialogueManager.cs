using UnityEngine;
using System;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private Dialogue currentDialogue;

    [SerializeField] private GameObject dialoguePrefab;       // ������
    [SerializeField] private DialogueLoader dialogueLoader;
    private DialogueUI dialogueInstance;                      // �ν��Ͻ�ȭ�� UI

    public bool isDialogue = false;
    public bool isChoice = false;
    public static DialogueManager Instance { get; private set; }

    // ������ ���ۿ� �ʵ� �߰�
    private int selectedChoiceIndex = 1; // 1�� ��ư���� ���� (1 �Ǵ� 2)
    private bool isInputEnabled = true;  // ��ٿ�� ������ �Է� ��� ���� �÷���
    private float inputCooldown = 0.2f;  // ���� ��ٿ� ����
    private float lastInputTime;         // ���� ��ٿ� ����

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

    // DialogueManager.cs (Update �Լ�)

    void Update()
    {
        // 1. ��ٿ� üũ
        if (Time.time < lastInputTime + inputCooldown) return;

        // 2. �Ϲ� ��ȭ ���� (�������� ���� ��)
        if (!isChoice && Input.GetKeyDown(KeyCode.Space))
        {
            lastInputTime = Time.time; // ��ٿ� ����

            if (dialogueInstance.IsTyping)
                dialogueInstance.FinishTyping();
            else
                DisplayNextDialogue();
        }

        // 3. ������ Ű���� ���� (isChoice�� true�� ����)
        if (isChoice)
        {
            UpdateChoiceHighlight(selectedChoiceIndex); // 1�� ��ư�� �⺻���� ���̶���Ʈ

            bool inputProcessed = false;

            // ��/�Ʒ� ����Ű ���� (1���� 2�� ������ ���)
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                // ���� 1���̸� 2������, 2���̸� 1������ ���
                selectedChoiceIndex = (selectedChoiceIndex == 1) ? 2 : 1;
                // UI ���̶���Ʈ �Լ��� ȣ���Ͽ� ��ư ���� ���� �ʿ�
                UpdateChoiceHighlight(selectedChoiceIndex);
                inputProcessed = true;
            }

            // ���� Ű ���� (���� Ȯ��)
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnChoiceSelected(selectedChoiceIndex);
                inputProcessed = true;
            }

            if (inputProcessed)
            {
                lastInputTime = Time.time; // �Է� ó�� �� ��ٿ� ����
            }
        }
    }

    private void UpdateChoiceHighlight(int index)
    {
        Color highlightColor = Color.yellow;
        Color normalColor = Color.white;

        // ������ 1�� ��ư�� Image ������Ʈ ���� ����
        Image img1 = dialogueInstance.choiceButton1.GetComponent<Image>();
        if (img1 != null) img1.color = (index == 1) ? highlightColor : normalColor;

        // ������ 2�� ��ư�� Image ������Ʈ ���� ����
        Image img2 = dialogueInstance.choiceButton2.GetComponent<Image>();
        if (img2 != null) img2.color = (index == 2) ? highlightColor : normalColor;
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
            selectedChoiceIndex = 1;
            UpdateChoiceHighlight(selectedChoiceIndex);

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
            EndDialogue();
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
