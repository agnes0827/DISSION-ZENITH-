using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    [SerializeField] private GameObject dustCleaningUIPanel;

    private DialogueManager dialogueManager;
    private int dustCleanedCount = 0;

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
        Debug.Log("���� ���� �̴ϰ��� ����");
    }

    // ���� �ϳ� ���� ���� �� ȣ���
    private void OnDustCleaned(GameObject dustObject)
    {
        dustObject.SetActive(false);
        dustCleanedCount++;

        Debug.Log($"���� ���� �Ϸ� ({dustCleanedCount}/3)");

        EndDustCleaning();

        // ���� 3�� ���� ���� �� �� 2�� ���� ���
        if (dustCleanedCount >= 3 && !InventoryManager.Instance.HasItem("Library_Key_Floor2"))
        {
            Debug.Log("2�� ���� ȹ��");
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
        Debug.Log("���� ���� �̴ϰ��� ����");
    }
}
