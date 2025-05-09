using UnityEngine;
using System.Collections.Generic;

public class DialogueLoader : MonoBehaviour
{
    public Dictionary<string, Dialogue> dialogues = new Dictionary<string, Dialogue>();

    void Start()
    {
        LoadDialogue("CSV/dialogue");
    }

    void LoadDialogue(string fileName)
    {
        List<Dictionary<string, object>> data = CSVReader.Read(fileName);

        foreach (var entry in data)
        {
            string id = entry["ID"].ToString();
            string speaker = entry["Speaker"].ToString();
            string dialogue = entry["Dialogue"].ToString();
            string nextId = entry.ContainsKey("Next ID") ? entry["Next ID"].ToString() : "END";

            dialogues[id] = new Dialogue(id, speaker, dialogue, nextId);
        }

        Debug.Log("CSV 대화 데이터 로드 완료!");
    }

    public Dialogue GetDialogueById(string id)
    {
        dialogues.TryGetValue(id, out var dialogue);
        return dialogue;
    }
}
