using UnityEngine;

public class DialogueInteraction : MonoBehaviour
{
    private DialogueTrigger currentDialogueTarget;
    private DialogueManager dialogueManager;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        DialogueTrigger trigger = other.GetComponent<DialogueTrigger>();

        if (trigger != null)
        {
            currentDialogueTarget = trigger;

            // 접촉 시 자동 실행 옵션이 켜져 있다면
            if (currentDialogueTarget.triggerOnEnter)
            {
                // 현재 대화 중이거나 미니게임 중이 아니라면 실행
                if (!IsSystemLocked())
                {
                    currentDialogueTarget.TriggerDialogue();
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (currentDialogueTarget != null && other.gameObject == currentDialogueTarget.gameObject)
        {
            currentDialogueTarget = null;
        }
    }


    void Update()
    {
        if (currentDialogueTarget == null) return;
        if (IsSystemLocked()) return;

        bool dialogueLocked = (dialogueManager != null && dialogueManager.isDialogue);
        bool miniGameLocked = MiniGameManager.IsMiniGameActive;
        bool cutsceneLocked = (CutsceneManager.Instance != null && CutsceneManager.IsCutscenePlaying);

        if (dialogueLocked || miniGameLocked || cutsceneLocked) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            currentDialogueTarget.TriggerDialogue();
        }
    }

        private bool IsSystemLocked()
    {
        bool dialogueLocked = (dialogueManager != null && dialogueManager.isDialogue);
        bool miniGameLocked = MiniGameManager.IsMiniGameActive;
        // CutsceneManager 체크 등 추가 가능

        return dialogueLocked || miniGameLocked;
    }
}
