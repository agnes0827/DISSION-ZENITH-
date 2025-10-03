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

    // ���� �ϳ� ���� ���� �� ȣ���
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
            Debug.LogWarning("DustObject�� DustObject ��ũ��Ʈ�� �����ϴ�!");
            return;
        }

        int currentCleanedCount = GameStateManager.Instance.cleanedDustIds.Count;
        Debug.Log($"���� ���� �Ϸ� ({currentCleanedCount} / 3)");

        // ���� 3�� ���� ���� �� �� 2�� ���� ���
        if (currentCleanedCount >= 3 && !GameStateManager.Instance.isDustCleaningQuestCompleted)
        {
            Debug.Log("2�� ���� ȹ��");
            GameStateManager.Instance.isDustCleaningQuestCompleted = true;

            InventoryManager.Instance.AddItem("Library_Key_Floor2", "������ 2�� ����");

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
