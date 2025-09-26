using UnityEngine;
using System;
using System.Collections.Generic;

// 씬이 바뀌어도 파괴되지 않고, 모든 대화의 흐름을 총괄하는 '뇌' 역할을 합니다.
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("핵심 연결 요소")]
    [SerializeField] private GameObject dialogueUIPrefab;
    [SerializeField] private DialogueLoader dialogueLoader;

    public bool isDialogue { get; private set; } = false;
    public bool isChoice { get; private set; } = false;
    private DialogueUI dialogueUI;
    private Dialogue currentDialogue;

    // 입력 및 선택지 관련 변수
    private int selectedChoiceIndex = 1;
    private float inputCooldown = 0.2f;
    private float lastInputTime;

    // 이벤트
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

    #region UI 등록 시스템
    public void RegisterDialogueUI(DialogueUI ui)
    {
        dialogueUI = ui;
        Debug.Log("DialogueUI가 DialogueManager에 성공적으로 등록되었습니다.");
    }

    // DialogueUI가 파괴될 때 참조를 깨끗하게 정리합니다.
    public void UnregisterDialogueUI()
    {
        dialogueUI = null;
        Debug.Log("DialogueUI가 파괴되어 등록 해제되었습니다.");
    }
    #endregion

    #region 입력 처리
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

    #region 대화 흐름 제어
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
                    Debug.LogError("현재 씬에 UI를 표시할 Canvas가 없습니다!");
                    return;
                }
                GameObject uiObject = Instantiate(dialogueUIPrefab, sceneCanvas.transform);
            }
            else
            {
                Debug.LogError("DialogueUIPrefab이 할당되지 않았습니다!");
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
            Debug.LogError($"ID '{id}'에 해당하는 대화 데이터를 찾을 수 없습니다.");
            EndDialogue();
            return;
        }

        dialogueUI.ShowDialogue(currentDialogue.speaker, currentDialogue.dialogue, currentDialogue.portrait);

        if (!string.IsNullOrEmpty(currentDialogue.choice1))
        {
            isChoice = true;
            selectedChoiceIndex = 1;
            dialogueUI.ShowChoices(currentDialogue.choice1, currentDialogue.choice2);
            dialogueUI.HighlightChoice(selectedChoiceIndex); // 첫 선택지 하이라이트

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
            // QuestManager가 존재하고, 아직 수락하지 않은 퀘스트라면
            if (QuestManager.Instance != null && !QuestManager.Instance.HasAccepted(currentDialogue.questId))
            {
                QuestManager.Instance.AcceptQuest(currentDialogue.questId);
                Debug.Log($"[DialogueManager] 퀘스트 수락됨: {currentDialogue.questId}");
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

