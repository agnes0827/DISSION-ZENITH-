using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetTameTrigger : MonoBehaviour
{
    [Header("필요한 퀘스트 ID")]
    public string requiredQuestId = "Q03";

    private bool alreadyUnlocked = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (alreadyUnlocked) return;
        if (!collision.CompareTag("Player")) return;

        // 퀘스트 완료 여부 확인
        if (QuestManager.Instance == null || PetC.Instance == null)
            return;

        // Q03이 완료된 상태에서만 펫 언락
        if (QuestManager.Instance.HasCompleted(requiredQuestId))
        {
            alreadyUnlocked = true;
            PetC.Instance.UnlockFollow();
        }
        else
        {
            // 아직 퀘스트 안 했으면 그냥 무시하거나,
            // "아직 경계하고 있다..." 같은 연출도 가능
            Debug.Log("[PetTameTrigger] 아직 퀘스트를 완료하지 않았습니다.");
        }
    }
}
