using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    [SerializeField] private GameObject dustCleaningUIPanel;

    private DialogueManager dialogueManager;

    public static bool IsMiniGameActive { get; private set; } = false;

    void Start()
    {
        dialogueManager = DialogueManager.Instance;

        if (dustCleaningUIPanel != null)
        {
            dustCleaningUIPanel.SetActive(false);
        }
    }

    void OnEnable()
    {
        DialogueManager.OnMiniGameRequested += StartDustCleaning;
    }

    void OnDisable()
    {
        DialogueManager.OnMiniGameRequested -= StartDustCleaning;
    }


    public void StartDustCleaning(GameObject dustObject)
    {
        IsMiniGameActive = true;
        dustCleaningUIPanel.SetActive(true);
        var cleaningGame = dustCleaningUIPanel.GetComponent<DustCleaningGame>();
        cleaningGame.BeginGame(dustObject, this.OnDustCleaned);
    }

    // 먼지 하나 제거 성공 시 호출됨
    private void OnDustCleaned(GameObject dustObject)
    {
        dustObject.SetActive(false);
        EndDustCleaning();

        var dustComponent = dustObject.GetComponent<DustObject>();
        if (dustComponent != null)
        {
            GameStateManager.Instance.cleanedDustIds.Add(dustComponent.dustId);
        }
        else
        {
            Debug.LogWarning("DustObject에 DustObject 스크립트가 없습니다!");
            return;
        }

        int currentCleanedCount = GameStateManager.Instance.cleanedDustIds.Count;
        Debug.Log($"먼지 제거 완료 ({currentCleanedCount} / 3)");

        // 먼지 3개 전부 제거 시 → 2층 열쇠 드랍
        if (currentCleanedCount >= 3 && !GameStateManager.Instance.isDustCleaningQuestCompleted)
        {
            Debug.Log("2층 열쇠 획득");
            GameStateManager.Instance.isDustCleaningQuestCompleted = true;

            InventoryManager.Instance.AddItem("Library_Key_Floor2", "도서관 2층 열쇠");

            if (dialogueManager != null)
            {
                dialogueManager.StartDialogue("40005");
            }
        }
    }

    public void EndDustCleaning()
    {
        IsMiniGameActive = false;
        dustCleaningUIPanel.SetActive(false);
    }
}
