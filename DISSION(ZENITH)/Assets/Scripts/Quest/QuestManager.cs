using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    [SerializeField] private QuestLoader questLoader;
    private QuestUI questUI;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (questLoader != null)
            {
                questLoader.LoadQuestData();
            }
            else
            {
                Debug.LogError("QuestLoader가 연결되지 않았습니다!");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterQuestUI(QuestUI ui)
    {
        questUI = ui;
        Debug.Log("QuestUI가 QuestManager에 성공적으로 등록되었습니다.");
    }

    public void UnregisterQuestUI(QuestUI uiToRemove)
    {
        if (questUI == uiToRemove)
        {
            questUI = null;
            Debug.Log($"QuestUI ({uiToRemove.gameObject.name})가 파괴되어 등록 해제되었습니다.");
        }
    }

    // 퀘스트 수락
    public void AcceptQuest(string questId)
    {
        // 이미 수락했거나 완료한 퀘스트는 다시 수락하지 않습니다.
        if (HasAccepted(questId) || HasCompleted(questId)) return;

        GameStateManager.Instance.acceptedQuests.Add(questId);
        Debug.Log($"[퀘스트 수락] ID: {questId}");

        // 등록된 UI가 있다면, UI를 표시합니다.
        var quest = GetQuestById(questId);
        if (questUI != null && quest != null)
        {
            questUI.ShowQuest(quest);
        }
        UpdateQuestIcons();
    }

    // 퀘스트 완료 처리
    public void CompleteQuest(string questId)
    {
        if (GameStateManager.Instance.acceptedQuests.Contains(questId) && !GameStateManager.Instance.completedQuests.Contains(questId))
        {
            GameStateManager.Instance.acceptedQuests.Remove(questId);
            GameStateManager.Instance.objectiveReachedQuests.Remove(questId);
            GameStateManager.Instance.completedQuests.Add(questId);

            var quest = GetQuestById(questId);
            GiveReward(quest.reward);

            if (questUI != null && questUI.GetCurrentQuestId() == questId)
            {
                questUI.Hide();
            }

            UpdateQuestIcons();
            Debug.Log($"[퀘스트 완료] ID: {questId}");
        }
    }

    // 목표 달성
    public void SetObjectiveReached(string questId)
    {
        if (GameStateManager.Instance.acceptedQuests.Contains(questId) && !GameStateManager.Instance.objectiveReachedQuests.Contains(questId))
        {
            GameStateManager.Instance.objectiveReachedQuests.Add(questId);
            Debug.Log($"[목표 달성] {questId}");
        }
    }

    // NPC에게 보고 → TalkToNPC 퀘스트 완료 시도
    //public void TryCompleteTalkToNPC(string npcId)
    //{
    //    foreach (var questId in acceptedQuests)
    //    {
    //        Quest quest = GetQuestById(questId);
    //        if (quest == null) continue;

    //        // TalkToNPC 타입이고 목표 대상이면 완료 처리
    //        if (quest.Type == QuestType.HaveItem && quest.target_id == npcId)
    //        {
    //            if (objectiveReachedQuests.Contains(questId))
    //            {
    //                CompleteQuest(questId);
    //                Debug.Log($"[퀘스트 완료] {quest.quest_title}");
    //            }
    //            break;
    //        }
    //    }
    //}

    public void CheckQuestItem(string itemId)
    {
        foreach (var questId in new List<string>(GameStateManager.Instance.acceptedQuests))
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

    // 상태 체크
    public bool HasAccepted(string questId) => GameStateManager.Instance.acceptedQuests.Contains(questId);
    public bool HasCompleted(string questId) => GameStateManager.Instance.completedQuests.Contains(questId);
    public bool IsObjectiveReached(string questId) => GameStateManager.Instance.objectiveReachedQuests.Contains(questId);


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
