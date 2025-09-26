using UnityEngine;
using System.Collections.Generic;

public class QuestLoader : MonoBehaviour
{
    private Dictionary<string, Quest> quests = new Dictionary<string, Quest>();

    public void LoadQuestData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/quest");

        foreach (var entry in data)
        {
            string id = entry["Quest ID"].ToString();
            string title = entry["Quest Title"].ToString();
            string description = entry["Description"].ToString();
            string type = entry["Type"].ToString();
            string targetId = entry["Target ID"].ToString();
            int requiredCount = int.TryParse(entry["Required Count"].ToString(), out var count) ? count : 1;
            string reward = entry["Reward"].ToString();

            quests[id] = new Quest(id, title, description, type, targetId, requiredCount, reward);
        }
        Debug.Log("CSV 퀘스트 데이터 로드 완료");
    }

    public Quest GetQuestById(string id)
    {
        if (quests.TryGetValue(id, out var quest))
        {
            return quest;
        }
        return null;
    }
}
