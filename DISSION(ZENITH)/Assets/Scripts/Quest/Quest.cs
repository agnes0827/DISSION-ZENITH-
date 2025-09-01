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
    public string quest_id;         // ����Ʈ id
    public string quest_title;      // ����Ʈ �̸�
    public string description;      // ����Ʈ ����
    public string type;             // ����Ʈ Ÿ�� (��: TalkToNPC, KillTarget, DeliverItem)
    public string target_id;        // Ÿ�� id
    public int required_count;      // �Ϸ� ���� ��ġ
    public string reward;           // ����

    public QuestType Type
    {
        get
        {
            if (System.Enum.TryParse(type, out QuestType parsedType))
                return parsedType;
            else
            {
                Debug.LogWarning($"[Quest] Unknown QuestType: {type}, �⺻������ ó����");
                return QuestType.TalkToNPC; // �⺻��
            }
        }
    }

    // ������
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

