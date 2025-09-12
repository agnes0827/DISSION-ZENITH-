using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private HashSet<string> acceptedQuests = new HashSet<string>();           // 진행 중
    private HashSet<string> completedQuests = new HashSet<string>();          // 완료
    private HashSet<string> objectiveReachedQuests = new HashSet<string>();   // 목표 달성 (대화 후)

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
            UpdateQuestIcons();
        }
    }

    // 목표 달성 (DialogueTrigger에서 직접 호출)
    public void SetObjectiveReached(string questId)
    {
        if (acceptedQuests.Contains(questId) && !objectiveReachedQuests.Contains(questId))
        {
            objectiveReachedQuests.Add(questId);
            Debug.Log($"[목표 달성] {questId}");
        }
    }

    // NPC에게 보고 → TalkToNPC 퀘스트 완료 시도
    public void TryCompleteTalkToNPC(string npcId)
    {
        foreach (var questId in acceptedQuests)
        {
            Quest quest = GetQuestById(questId);
            if (quest == null) continue;

            // TalkToNPC 타입이고 목표 대상이면 완료 처리
            if (quest.Type == QuestType.HaveItem && quest.target_id == npcId)
            {
                if (objectiveReachedQuests.Contains(questId))
                {
                    CompleteQuest(questId);
                    Debug.Log($"[퀘스트 완료] {quest.quest_title}");
                }
                break;
            }
        }
    }

    public void CheckQuestItem(string itemId)
    {
        foreach (var questId in new List<string>(acceptedQuests))
        {
            var quest = GetQuestById(questId);
            if (quest == null) continue;

            if (quest.Type == QuestType.HaveItem && quest.target_id == itemId)
            {
                SetObjectiveReached(questId);  
                CompleteQuest(questId);        
                Debug.Log($"[아이템 조건 만족→완료] {quest.quest_id} - {quest.quest_title}");
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

            UpdateQuestIcons();
        }
    }

    // 상태 체크
    public bool HasAccepted(string questId) => acceptedQuests.Contains(questId);
    public bool HasCompleted(string questId) => completedQuests.Contains(questId);
    public bool IsObjectiveReached(string questId) => objectiveReachedQuests.Contains(questId);

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

    // 아이콘 갱신
    private void UpdateQuestIcons()
    {
        foreach (var icon in FindObjectsOfType<QuestIconUI>())
        {
            icon.UpdateIcon();
        }
    }
}
