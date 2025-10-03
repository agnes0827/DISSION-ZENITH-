using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    [SerializeField] private QuestLoader questLoader;

    // 퀘스트 상태
    private HashSet<string> acceptedQuests = new HashSet<string>();           // 진행 중
    private HashSet<string> completedQuests = new HashSet<string>();          // 완료
    private HashSet<string> objectiveReachedQuests = new HashSet<string>();   // 목표 달성 (대화 후)

    // UI 참조
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

    public void UnregisterQuestUI()
    {
        questUI = null;
        Debug.Log("QuestUI가 파괴되어 등록 해제되었습니다.");
    }

    // 퀘스트 수락
    public void AcceptQuest(string questId)
    {
        // 이미 수락했거나 완료한 퀘스트는 다시 수락하지 않습니다.
        if (HasAccepted(questId) || HasCompleted(questId))
        {
            Debug.Log($"퀘스트 '{questId}'는 이미 처리된 퀘스트이므로 다시 수락하지 않습니다.");
            return;
        }

        acceptedQuests.Add(questId);
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
        if (acceptedQuests.Contains(questId) && !completedQuests.Contains(questId))
        {
            acceptedQuests.Remove(questId);
            objectiveReachedQuests.Remove(questId);
            completedQuests.Add(questId);

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
