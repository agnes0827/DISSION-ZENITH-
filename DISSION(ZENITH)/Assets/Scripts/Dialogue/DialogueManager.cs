using UnityEngine;
using System;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private Dialogue currentDialogue;

    [SerializeField] private GameObject dialoguePrefab;       // 프리팹
    [SerializeField] private DialogueLoader dialogueLoader;
    private DialogueUI dialogueInstance;                      // 인스턴스화된 UI

    public bool isDialogue = false;
    public bool isChoice = false;
    public static DialogueManager Instance { get; private set; }

    // 선택지 조작용 필드 추가
    private int selectedChoiceIndex = 1; // 1번 버튼부터 시작 (1 또는 2)
    private bool isInputEnabled = true;  // 쿨다운과 별개로 입력 제어를 위한 플래그
    private float inputCooldown = 0.2f;  // 기존 쿨다운 유지
    private float lastInputTime;         // 기존 쿨다운 유지

    // 미니게임 요청 이벤트 추가
    public static event Action<GameObject> OnMiniGameRequested;
    private GameObject currentDustObject; // 현재 상호작용 중인 먼지 오브젝트

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
            Debug.LogError("DialogueLoader가 연결되지 않았습니다!");
            return;
        }

        if (dialoguePrefab == null)
        {
            Debug.LogError("Dialogue Prefab이 연결되지 않았습니다!");
            return;
        }

        GameObject instance = Instantiate(dialoguePrefab, GameObject.Find("Canvas").transform);
        dialogueInstance = instance.GetComponent<DialogueUI>();

        if (dialogueInstance == null)
        {
            Debug.LogError("DialogueUI 스크립트를 찾을 수 없습니다!");
            return;
        }

        dialogueInstance.HideDialogue();
    }

    // DialogueManager.cs (Update 함수)

    void Update()
    {
        // 1. 쿨다운 체크
        if (Time.time < lastInputTime + inputCooldown) return;

        // 2. 일반 대화 진행 (선택지가 없을 때)
        if (!isChoice && Input.GetKeyDown(KeyCode.Space))
        {
            lastInputTime = Time.time; // 쿨다운 갱신

            if (dialogueInstance.IsTyping)
                dialogueInstance.FinishTyping();
            else
                DisplayNextDialogue();
        }

        // 3. 선택지 키보드 조작 (isChoice가 true일 때만)
        if (isChoice)
        {
            UpdateChoiceHighlight(selectedChoiceIndex); // 1번 버튼을 기본으로 하이라이트

            bool inputProcessed = false;

            // 위/아래 방향키 감지 (1번과 2번 선택지 토글)
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                // 현재 1번이면 2번으로, 2번이면 1번으로 토글
                selectedChoiceIndex = (selectedChoiceIndex == 1) ? 2 : 1;
                // UI 하이라이트 함수를 호출하여 버튼 색상 변경 필요
                UpdateChoiceHighlight(selectedChoiceIndex);
                inputProcessed = true;
            }

            // 엔터 키 감지 (선택 확정)
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnChoiceSelected(selectedChoiceIndex);
                inputProcessed = true;
            }

            if (inputProcessed)
            {
                lastInputTime = Time.time; // 입력 처리 후 쿨다운 갱신
            }
        }
    }

    private void UpdateChoiceHighlight(int index)
    {
        Color highlightColor = Color.yellow;
        Color normalColor = Color.white;

        // 선택지 1번 버튼의 Image 컴포넌트 색상 변경
        Image img1 = dialogueInstance.choiceButton1.GetComponent<Image>();
        if (img1 != null) img1.color = (index == 1) ? highlightColor : normalColor;

        // 선택지 2번 버튼의 Image 컴포넌트 색상 변경
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
            Debug.LogError("대화 데이터를 찾을 수 없습니다: " + currentId);
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

        // 퀘스트 처리
        if (!string.IsNullOrEmpty(currentDialogue.questId))
        {
            if (!QuestManager.Instance.HasAccepted(currentDialogue.questId))
            {
                QuestManager.Instance.AcceptQuest(currentDialogue.questId);
                Debug.Log($"퀘스트 수락됨: {currentDialogue.questId}");
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

        // 미니게임 시작 이벤트 발행
        if (nextId == "MINIGAME")
        {
            Debug.Log("미니게임 시작");
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
