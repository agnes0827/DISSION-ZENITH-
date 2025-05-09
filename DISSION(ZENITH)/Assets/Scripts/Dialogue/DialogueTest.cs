using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    public string startDialogueId;

    public void OnClickStartDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(startDialogueId);
    }
}
