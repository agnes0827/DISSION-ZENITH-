using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private HashSet<string> acceptedQuests = new HashSet<string>();          // 진행 중인 퀘스트
    private HashSet<string> completedQuests = new HashSet<string>();         // 완료한 퀘스트
    private HashSet<string> objectiveReachedQuests = new HashSet<string>();  // 목표 달성했지만 완료 전 상태

    private QuestLoader questLoader;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            questLoader = FindObjectOfType<QuestLoader>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 퀘스트 수락
    public void AcceptQuest(string questId)
    {
        if (!acceptedQuests.Contains(questId))
        {
            acceptedQuests.Add(questId);

            // UI 표시
            var quest = GetQuestById(questId);
            FindObjectOfType<QuestUI>()?.ShowQuest(quest);

            Debug.Log($"[퀘스트 수락] {quest.quest_title}");
        }

        // 퀘스트 아이콘 갱신
        foreach (var icon in FindObjectsOfType<QuestIconUI>())
        {
            icon.UpdateIcon();
        }
    }

    // 목표 달성

    public void SetObjectiveReached(string questId)
    {
        if (acceptedQuests.Contains(questId) && !objectiveReachedQuests.Contains(questId))
        {
            objectiveReachedQuests.Add(questId);
            Debug.Log($"[퀘스트 목표 달성] {questId}");
        }
    }
    
    // NPC 대화 시 퀘스트 완료 여부 확인
    public void TryCompleteTalkToNPC(string npcId)
    {
        foreach (var questId in acceptedQuests)
        {
            Quest quest = GetQuestById(questId);

            if (quest == null) continue;

            // 퀘스트 타입이 TalkToNPC이고 대상이 npcId인 경우만 처리
            if (quest.type == "TalkToNPC" && quest.target_id == npcId)
            {
                // 목표가 달성된 상태여야만 완료 처리
                if (objectiveReachedQuests.Contains(questId))
                {
                    Debug.Log($"[퀘스트 완료] {quest.quest_title} - NPC에게 보고 완료");
                    CompleteQuest(questId);
                }
                else
                {
                    Debug.Log($"[퀘스트 진행 중] {quest.quest_title} - 목표를 아직 달성하지 않음");
                }
                break;
            }
        }
    }


    // 퀘스트 완료 처리
    public void CompleteQuest(string questId)
    {
        if (acceptedQuests.Contains(questId) && !completedQuests.Contains(questId))
        {
            acceptedQuests.Remove(questId);
            objectiveReachedQuests.Remove(questId);
            completedQuests.Add(questId);

            var quest = GetQuestById(questId);
            GiveReward(quest.reward);

            // UI 닫기
            var questUI = FindObjectOfType<QuestUI>();
            if (questUI != null && questUI.GetCurrentQuestId() == questId)
                questUI.Hide();
        }
    }

    // 상태 체크 함수
    public bool HasAccepted(string questId)
    {
        return acceptedQuests.Contains(questId);
    }

    public bool HasCompleted(string questId)
    {
        return completedQuests.Contains(questId);
    }

    public bool IsObjectiveReached(string questId)
    {
        return objectiveReachedQuests.Contains(questId);
    }

    // 퀘스트 데이터 로드
    public Quest GetQuestById(string questId)
    {
        return questLoader?.GetQuestById(questId);
    }

    // 보상 지급
    private void GiveReward(string reward)
    {
        Debug.Log($"[퀘스트 보상 지급] {reward}");
    }
}
