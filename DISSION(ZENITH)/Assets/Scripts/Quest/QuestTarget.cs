using UnityEngine;

public class QuestTarget : MonoBehaviour
{
    public string questId;
    private bool isTriggered = false;

    private void Start()
    {
        Debug.Log($"[QuestTarget �ʱ�ȭ] {gameObject.name} / QuestId = {questId}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[QuestTarget Ʈ���� ����] {gameObject.name}�� {other.name} �浹");

        if (isTriggered) return;

        if (other.CompareTag("Player") && QuestManager.Instance.HasAccepted(questId))
        {
            Debug.Log($"[QuestTarget ��ǥ �޼�] {questId}");
            QuestManager.Instance.SetObjectiveReached(questId);
            isTriggered = true;
        }

    }
}
