using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Tooltip("기본 대화 ID")]
    public string defaultId;

    [Header("퀘스트 대화 설정")]

    [Tooltip("퀘스트 진행 중")]
    public string acceptedId;
    [Tooltip("퀘스트 완료 후")]
    public string completedId;

    public string QuestId;


    public void TriggerDialogue()
    {
        string selectedId = defaultId;

        if (!string.IsNullOrEmpty(QuestId))
        {
            if (QuestManager.Instance.HasCompleted(QuestId))
            {
                selectedId = completedId;
            }
            else if (QuestManager.Instance.HasAccepted(QuestId))
            {
                selectedId = acceptedId;
            }
        }

        FindObjectOfType<DialogueManager>().StartDialogue(selectedId);
    }
}
