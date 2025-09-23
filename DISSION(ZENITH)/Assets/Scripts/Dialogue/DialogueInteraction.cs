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
        currentDialogueTarget = other.GetComponent<DialogueTrigger>();
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

        bool dialogueLocked = (dialogueManager != null && dialogueManager.isDialogue);
        bool miniGameLocked = (MiniGameManager.Instance != null && MiniGameManager.IsMiniGameActive);
        bool cutsceneLocked = (CutsceneManager.Instance != null && CutsceneManager.IsCutscenePlaying);

        if (dialogueLocked || miniGameLocked || cutsceneLocked) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            // DialogueTrigger가 붙어 있으면 대화 시작 (기존 시스템)
            if (currentDialogueTarget.GetComponent<DialogueTrigger>() != null)
            {
                currentDialogueTarget.GetComponent<DialogueTrigger>().TriggerDialogue();
            }
        }
    }
}

