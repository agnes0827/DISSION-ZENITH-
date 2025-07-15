using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public string startDialogueId;  // 이 오브젝트의 대화 시작 ID

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(startDialogueId);
    }
}
