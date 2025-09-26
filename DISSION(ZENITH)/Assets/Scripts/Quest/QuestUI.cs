using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 퀘스트 창의 모든 시각적 요소를 제어하고, Manager에게 자신을 등록합니다.
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
            Debug.LogError("QuestManager가 씬에 없습니다! UI를 등록할 수 없습니다.");
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
