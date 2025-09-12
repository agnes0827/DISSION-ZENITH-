using UnityEngine;

public class QuestTarget : MonoBehaviour
{
    public string questId;
    private bool isTriggered = false;

    private void Start()
    {
        Debug.Log($"[QuestTarget 초기화] {gameObject.name} / QuestId = {questId}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[QuestTarget 트리거 감지] {gameObject.name}에 {other.name} 충돌");

        if (isTriggered) return;

        if (other.CompareTag("Player") && QuestManager.Instance.HasAccepted(questId))
        {
            Debug.Log($"[QuestTarget 목표 달성] {questId}");
            QuestManager.Instance.SetObjectiveReached(questId);
            isTriggered = true;
        }

    }
}
