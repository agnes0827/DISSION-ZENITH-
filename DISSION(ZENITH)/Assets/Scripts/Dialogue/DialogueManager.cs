using UnityEngine;
using System;
using System.Collections.Generic;

// ���� �ٲ� �ı����� �ʰ�, ��� ��ȭ�� �帧�� �Ѱ��ϴ� '��' ������ �մϴ�.
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("�ٽ� ���� ���")]
    [SerializeField] private GameObject dialogueUIPrefab;
    [SerializeField] private DialogueLoader dialogueLoader;

    public bool isDialogue { get; private set; } = false;
    public bool isChoice { get; private set; } = false;
    private DialogueUI dialogueUI;
    private Dialogue currentDialogue;

    // �Է� �� ������ ���� ����
    private int selectedChoiceIndex = 1;
    private float inputCooldown = 0.2f;
    private float lastInputTime;

    // �̺�Ʈ
    public static event Action<GameObject> OnMiniGameRequested;
    private GameObject currentDustObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (dialogueLoader != null)
            {
                dialogueLoader.LoadDialogueData();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region UI ��� �ý���
    public void RegisterDialogueUI(DialogueUI ui)
    {
        dialogueUI = ui;
        Debug.Log("DialogueUI�� DialogueManager�� ���������� ��ϵǾ����ϴ�.");
    }

    // DialogueUI�� �ı��� �� ������ �����ϰ� �����մϴ�.
    public void UnregisterDialogueUI()
    {
        dialogueUI = null;
        Debug.Log("DialogueUI�� �ı��Ǿ� ��� �����Ǿ����ϴ�.");
    }
    #endregion

    #region �Է� ó��
    void Update()
    {
        if (!isDialogue || Time.time < lastInputTime + inputCooldown) return;

        if (isChoice)
        {
            HandleChoiceInput();
        }
        else
        {
            HandleNormalInput();
        }
    }

    private void HandleNormalInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            lastInputTime = Time.time;
            if (dialogueUI.IsTyping)
            {
                dialogueUI.FinishTyping();
            }
            else
            {
                DisplayNextDialogue();
            }
        }
    }

    private void HandleChoiceInput()
    {
        bool inputProcessed = false;
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedChoiceIndex = (selectedChoiceIndex == 1) ? 2 : 1;
            dialogueUI.HighlightChoice(selectedChoiceIndex);
            inputProcessed = true;
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnChoiceSelected(selectedChoiceIndex);
            inputProcessed = true;
        }

        if (inputProcessed)
        {
            lastInputTime = Time.time;
        }
    }
    #endregion

    #region ��ȭ �帧 ����
    public void StartDialogue(string startId, GameObject dustObject = null)
    {
        currentDustObject = dustObject;

        if (dialogueUI == null)
        {
            if (dialogueUIPrefab != null)
            {
                Canvas sceneCanvas = FindObjectOfType<Canvas>();
                if (sceneCanvas == null)
                {
                    Debug.LogError("���� ���� UI�� ǥ���� Canvas�� �����ϴ�!");
                    return;
                }
                GameObject uiObject = Instantiate(dialogueUIPrefab, sceneCanvas.transform);
            }
            else
            {
                Debug.LogError("DialogueUIPrefab�� �Ҵ���� �ʾҽ��ϴ�!");
                return;
            }
        }

        isDialogue = true;
        dialogueUI.ShowDialoguePanel();
        DisplayDialogue(startId);
    }

    private void DisplayDialogue(string id)
    {
        currentDialogue = dialogueLoader.GetDialogueId(id);
        if (currentDialogue == null)
        {
            Debug.LogError($"ID '{id}'�� �ش��ϴ� ��ȭ �����͸� ã�� �� �����ϴ�.");
            EndDialogue();
            return;
        }

        dialogueUI.ShowDialogue(currentDialogue.speaker, currentDialogue.dialogue, currentDialogue.portrait);

        if (!string.IsNullOrEmpty(currentDialogue.choice1))
        {
            isChoice = true;
            selectedChoiceIndex = 1;
            dialogueUI.ShowChoices(currentDialogue.choice1, currentDialogue.choice2);
            dialogueUI.HighlightChoice(selectedChoiceIndex); // ù ������ ���̶���Ʈ

            dialogueUI.choiceButton1.onClick.RemoveAllListeners();
            dialogueUI.choiceButton2.onClick.RemoveAllListeners();
            dialogueUI.choiceButton1.onClick.AddListener(() => OnChoiceSelected(1));
            dialogueUI.choiceButton2.onClick.AddListener(() => OnChoiceSelected(2));
        }
        else
        {
            isChoice = false;
            dialogueUI.HideChoices();
        }

        if (!string.IsNullOrEmpty(currentDialogue.questId))
        {
            // QuestManager�� �����ϰ�, ���� �������� ���� ����Ʈ���
            if (QuestManager.Instance != null && !QuestManager.Instance.HasAccepted(currentDialogue.questId))
            {
                QuestManager.Instance.AcceptQuest(currentDialogue.questId);
                Debug.Log($"[DialogueManager] ����Ʈ ������: {currentDialogue.questId}");
            }
        }
    }

    private void OnChoiceSelected(int choiceNumber)
    {
        isChoice = false;
        dialogueUI.HideChoices();
        string nextId = (choiceNumber == 1) ? currentDialogue.choice1NextId : currentDialogue.choice2NextId;

        if (nextId == "MINIGAME")
        {
            OnMiniGameRequested?.Invoke(currentDustObject);
            EndDialogue();
        }
        else
        {
            DisplayDialogue(nextId);
        }
    }

    private void DisplayNextDialogue()
    {
        if (currentDialogue.nextId == "END" || string.IsNullOrEmpty(currentDialogue.nextId))
        {
            EndDialogue();
        }
        else
        {
            DisplayDialogue(currentDialogue.nextId);
        }
    }

    private void EndDialogue()
    {
        isDialogue = false;
        if (dialogueUI != null)
        {
            dialogueUI.HideDialoguePanel();
        }
    }
    #endregion
}

