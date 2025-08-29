using UnityEngine;
using System;

public class DialogueManager : MonoBehaviour
{
    private DialogueLoader dialogueLoader;
    private Dialogue currentDialogue;

    [SerializeField] private GameObject dialoguePrefab; // 프리팹
    private DialogueUI dialogueInstance;                // 인스턴스화된 UI

    public bool isDialogue = false;
    public bool isChoice = false;

    // 미니게임 요청 이벤트 추가
    public static event Action<GameObject> OnMiniGameRequested;
    private GameObject currentDustObject; // 현재 상호작용 중인 먼지 오브젝트

    void Start()
    {
        dialogueLoader = FindObjectOfType<DialogueLoader>();

        if (dialoguePrefab == null)
        {
            Debug.LogError("Dialogue Prefab이 연결되지 않았습니다!");
            return;
        }

        // 프리팹 인스턴스화 + 캔버스 자식으로 넣기
        GameObject instance = Instantiate(dialoguePrefab, GameObject.Find("Canvas").transform);
        dialogueInstance = instance.GetComponent<DialogueUI>();

        if (dialogueInstance == null)
        {
            Debug.LogError("DialogueUI 스크립트를 찾을 수 없습니다!");
            return;
        }

        dialogueInstance.HideDialogue(); // 시작 시 숨기기
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

    void DisplayDialogue(string currentId)
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
