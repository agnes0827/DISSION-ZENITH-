using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestUI : MonoBehaviour
{
    public GameObject questPanel;
    public TMP_Text titleText;
    public TMP_Text descriptionText;

    private Quest currentQuest;

    private void Start()
    {
        questPanel.SetActive(false);
    }

    public void ShowQuest(Quest quest)
    {
        currentQuest = quest;
        titleText.text = quest.quest_title;
        descriptionText.text = quest.description;
        questPanel.SetActive(true);
    }

    public void Hide()
    {
        currentQuest = null;
        questPanel.SetActive(false);
    }

    public string GetCurrentQuestId()
    {
        return currentQuest?.quest_id;
    }
}
