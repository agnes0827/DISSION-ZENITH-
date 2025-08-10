using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private HashSet<string> acceptedQuests = new HashSet<string>();
    private HashSet<string> completedQuests = new HashSet<string>();

    private QuestLoader questLoader;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환에도 유지
            questLoader = FindObjectOfType<QuestLoader>();  // QuestLoader 연결
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AcceptQuest(string questId)
    {
        if (!acceptedQuests.Contains(questId))
        {
            acceptedQuests.Add(questId);

            // UI에 퀘스트 표시
            var quest = GetQuestById(questId);
            FindObjectOfType<QuestUI>()?.ShowQuest(quest);
        }

        // 퀘스트 아이콘 갱신
        foreach (var icon in FindObjectsOfType<QuestIconUI>())
        {
            icon.UpdateIcon();
        }
    }

    public bool HasAccepted(string questId)
    {
        return acceptedQuests.Contains(questId);
    }

    // 완료 처리
    public void TryCompleteTalkToNPC(string npcId)
    {
        foreach (var questId in acceptedQuests)
        {
            Quest quest = GetQuestById(questId);
            if (quest != null && quest.type == "TalkToNPC" && quest.target_id == npcId)
            {
                Debug.Log($"[퀘스트 완료] {quest.quest_title} - NPC와 대화 완료");
                CompleteQuest(questId);
                break; // 하나만 완료
            }
        }
    }

    public void CompleteQuest(string questId)
    {
        if (acceptedQuests.Contains(questId) && !completedQuests.Contains(questId))
        {
            acceptedQuests.Remove(questId);
            completedQuests.Add(questId);

            var quest = GetQuestById(questId);
            GiveReward(quest.reward); // 나중에 구현할 보상 처리

            // UI 닫기
            var questUI = FindObjectOfType<QuestUI>();
            if (questUI != null && questUI.GetCurrentQuestId() == questId)
                questUI.Hide();
        }
    }

    public bool HasCompleted(string questId)
    {
        return completedQuests.Contains(questId);
    }

    public Quest GetQuestById(string questId)
    {
        return questLoader?.GetQuestById(questId);
    }

    private void GiveReward(string reward)
    {
        Debug.Log($"보상 지급: {reward}");
    }
}
