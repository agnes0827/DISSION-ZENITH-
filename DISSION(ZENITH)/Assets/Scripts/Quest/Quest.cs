using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public enum QuestType
{
    TalkToNPC,
    HaveItem
    //KillTarget,
    //DeliverItem,
    //CollectItem,
    //Clean
}

public class Quest
{
    public string quest_id;         // 퀘스트 id
    public string quest_title;      // 퀘스트 이름
    public string description;      // 퀘스트 설명
    public string type;             // 퀘스트 타입 (예: TalkToNPC, KillTarget, DeliverItem)
    public string target_id;        // 타겟 id
    public int required_count;      // 완료 조건 수치
    public string reward;           // 보상

    public QuestType Type
    {
        get
        {
            if (System.Enum.TryParse(type, out QuestType parsedType))
                return parsedType;
            else
            {
                Debug.LogWarning($"[Quest] Unknown QuestType: {type}, 기본값으로 처리됨");
                return QuestType.TalkToNPC; // 기본값
            }
        }
    }

    // 생성자
    public Quest(string id, string title, string description,
        string type, string target_id, int required_count, string reward)
    {
        this.quest_id = id;
        this.quest_title = title;
        this.description = description;
        this.type = type;
        this.target_id = target_id;
        this.required_count = required_count;
        this.reward = reward;
    }
}

