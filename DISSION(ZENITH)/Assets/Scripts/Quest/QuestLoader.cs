using UnityEngine;
using System.Collections.Generic;

public class QuestLoader : MonoBehaviour
{
    public Dictionary<string, Quest> quests = new Dictionary<string, Quest>();

    void Awake()
    {
        LoadQuest("CSV/quest");
    }

    void LoadQuest(string fileName)
    {
        List<Dictionary<string, object>> data = CSVReader.Read(fileName);

        foreach (var entry in data)
        {
            string id = entry["Quest ID"].ToString();
            string title = entry["Quest Title"].ToString();
            string description = entry["Description"].ToString();
            string type = entry["Type"].ToString();
            string targetId = entry["Target ID"].ToString();
            string requiredCountStr = entry["Required Count"].ToString();
            int requiredCount = int.TryParse(requiredCountStr, out var count) ? count : 1;
            string reward = entry["Reward"].ToString();

            Quest quest = new Quest(id, title, description, type, targetId, requiredCount, reward);
            quests[id] = quest;
        }

        Debug.Log("퀘스트 데이터 로드 완료");
    }

    // ID로 대화 데이터 로드
    public Quest GetQuestById(string id)
    {
        quests.TryGetValue(id, out var quest);
        return quest;
    }
}
