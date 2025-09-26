using UnityEngine;
using System.Collections.Generic;

// CSV ������ �а� Dialogue �����͸� Dictionary�� ����
public class DialogueLoader : MonoBehaviour
{
    public Dictionary<string, Dialogue> dialogues = new Dictionary<string, Dialogue>();

    // CSVReder�� ���� CSV ���� �Ľ�, ��ȭ ������ �ε�
    public void LoadDialogueData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/dialogue");

        foreach (var entry in data)
        {
            string id = entry["ID"].ToString();
            string speaker = entry["Speaker"].ToString();
            string dialogue = entry["Dialogue"].ToString();
            string nextId = entry.ContainsKey("Next ID") ? entry["Next ID"].ToString() : "END";
            string portrait = entry.ContainsKey("Portrait") ? entry["Portrait"].ToString() : "";

            string choice1 = entry.ContainsKey("Choice1") ? entry["Choice1"].ToString() : "";
            string choice1NextId = entry.ContainsKey("Choice1 Next ID") ? entry["Choice1 Next ID"].ToString() : "";
            string choice2 = entry.ContainsKey("Choice2") ? entry["Choice2"].ToString() : "";
            string choice2NextId = entry.ContainsKey("Choice2 Next ID") ? entry["Choice2 Next ID"].ToString() : "";

            string questId = entry.ContainsKey("Quest ID") ? entry["Quest ID"].ToString() : "";

            dialogues[id] = new Dialogue(id, speaker, dialogue, nextId, portrait, choice1, choice1NextId, choice2, choice2NextId, questId);

        }
        Debug.Log("CSV ��ȭ ������ �ε� �Ϸ�");
    }

    // ID�� ��ȭ ������ �ε�
    public Dialogue GetDialogueId(string id)
    {
        dialogues.TryGetValue(id, out var dialogue);
        return dialogue;
    }
}
