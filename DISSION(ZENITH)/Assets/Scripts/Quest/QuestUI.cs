using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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

    private void Start()
    {
        UpdateQuestDisplayBasedOnGameState();

        if (currentQuest == null && questPanel != null)
        {
            questPanel.SetActive(false);
        }
    }

    public void UpdateQuestDisplayBasedOnGameState()
    {
        if (GameStateManager.Instance == null || QuestManager.Instance == null)
        {
            Hide();
            return;
        }

        var acceptedQuests = GameStateManager.Instance.acceptedQuests;

        // 진행 중인 퀘스트가 있다면
        if (acceptedQuests.Count > 0)
        {
            string questIdToShow = acceptedQuests.FirstOrDefault();

            if (!string.IsNullOrEmpty(questIdToShow))
            {
                Quest questData = QuestManager.Instance.GetQuestById(questIdToShow);
                if (questData != null)
                {
                    ShowQuest(questData);
                }
                else
                {
                    Debug.LogWarning($"Quest ID '{questIdToShow}' 데이터를 찾을 수 없어 UI를 숨깁니다.");
                    Hide();
                }
            }
            else
            {
                Hide();
            }
        }
        else
        {
            Hide();
        }
    }

    private void OnDestroy()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.UnregisterQuestUI(this);
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
