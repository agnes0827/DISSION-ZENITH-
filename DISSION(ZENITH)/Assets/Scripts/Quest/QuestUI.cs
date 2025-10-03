using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ����Ʈ â�� ��� �ð��� ��Ҹ� �����ϰ�, Manager���� �ڽ��� ����մϴ�.
public class QuestUI : MonoBehaviour
{
    [SerializeField] private GameObject questPanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    private Quest currentQuest;

    private void Awake()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.RegisterQuestUI(this);
        }
        else
        {
            Debug.LogError("QuestManager�� ���� �����ϴ�! UI�� ����� �� �����ϴ�.");
        }
    }

    private void OnDestroy()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.UnregisterQuestUI();
        }
    }

    private void Start()
    {
        if (questPanel != null)
        {
            questPanel.SetActive(false);
        }
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
