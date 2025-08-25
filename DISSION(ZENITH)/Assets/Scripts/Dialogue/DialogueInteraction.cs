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
        if (currentDialogueTarget != null &&
            Input.GetKeyDown(KeyCode.F) &&
            !dialogueManager.isDialogue) // ��ȭ ���� �ƴ� ���� ����
        {
            currentDialogueTarget.TriggerDialogue();
        }
    }
}

