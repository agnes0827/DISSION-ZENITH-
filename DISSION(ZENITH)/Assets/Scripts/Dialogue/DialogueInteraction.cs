using UnityEngine;

public class DialogueInteraction : MonoBehaviour
{
    private DialogueTrigger currentDialogueTarget;

    void OnTriggerEnter2D(Collider2D other)
    {
        currentDialogueTarget = other.GetComponent<DialogueTrigger>();
        if (currentDialogueTarget != null)
            Debug.Log($"{other.name}�� ��ȭ ���� ����");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (currentDialogueTarget != null && other.gameObject == currentDialogueTarget.gameObject)
        {
            Debug.Log($"{other.name}�� ��ȭ �Ұ� ����");
            currentDialogueTarget = null;
        }
    }

    void Update()
    {
        if (currentDialogueTarget != null &&
            Input.GetKeyDown(KeyCode.F) &&
            !FindObjectOfType<DialogueManager>().isDialogue) // ��ȭ ���� �ƴ� ���� ����
        {
            Debug.Log("��ȭ ����!");
            currentDialogueTarget.TriggerDialogue();
        }
    }

}

