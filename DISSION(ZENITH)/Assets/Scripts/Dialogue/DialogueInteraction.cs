using UnityEngine;

public class DialogueInteraction : MonoBehaviour
{
    private DialogueTrigger currentDialogueTarget;

    void OnTriggerEnter2D(Collider2D other)
    {
        currentDialogueTarget = other.GetComponent<DialogueTrigger>();
        if (currentDialogueTarget != null)
            Debug.Log($"{other.name}과 대화 가능 상태");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (currentDialogueTarget != null && other.gameObject == currentDialogueTarget.gameObject)
        {
            Debug.Log($"{other.name}과 대화 불가 상태");
            currentDialogueTarget = null;
        }
    }

    void Update()
    {
        if (currentDialogueTarget != null &&
            Input.GetKeyDown(KeyCode.F) &&
            !FindObjectOfType<DialogueManager>().isDialogue) // 대화 중이 아닐 때만 시작
        {
            Debug.Log("대화 시작!");
            currentDialogueTarget.TriggerDialogue();
        }
    }

}

