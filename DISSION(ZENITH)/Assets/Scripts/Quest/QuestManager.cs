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
            Debug.Log($"[퀘스트 수락] {questId}");
            acceptedQuests.Add(questId);

            // UI에 퀘스트 표시
            var quest = GetQuestById(questId);
            Debug.Log($"[퀘스트 데이터 확인] 제목: {quest?.quest_title}");

            FindObjectOfType<QuestUI>()?.ShowQuest(quest);
        }
    }

    public bool HasAccepted(string questId)
    {
        return acceptedQuests.Contains(questId);
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
        Debug.Log($"보상 지급: {reward}");  // 나중에 인벤토리나 골드 시스템 연동
    }
}
