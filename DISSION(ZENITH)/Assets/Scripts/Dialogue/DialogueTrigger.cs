using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public string startDialogueId;  // �� ������Ʈ�� ��ȭ ���� ID

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(startDialogueId);
    }
}
