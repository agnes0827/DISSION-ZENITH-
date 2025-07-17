using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private HashSet<string> acceptedQuests = new HashSet<string>();
    private HashSet<string> completedQuests = new HashSet<string>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AcceptQuest(string questId)
    {
        if (!acceptedQuests.Contains(questId))
            acceptedQuests.Add(questId);
    }

    public bool HasAccepted(string questId)
    {
        return acceptedQuests.Contains(questId);
    }

    public void CompleteQuest(string questId)
    {
        if (acceptedQuests.Contains(questId))
            completedQuests.Add(questId);
    }

    public bool HasCompleted(string questId)
    {
        return completedQuests.Contains(questId);
    }
}
