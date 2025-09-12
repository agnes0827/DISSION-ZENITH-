using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance;

    [SerializeField] private GameObject dustCleaningUIPanel;

    private DialogueManager dialogueManager;

    private int dustCleanedCount = 0;

    public bool IsMiniGameActive { get; private set; } = false; // 미니게임 진행 여부

    void Awake()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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
        Debug.Log("Start → Active=true");
    }

    // 먼지 하나 제거 성공 시 호출됨
    private void OnDustCleaned(GameObject dustObject)
    {
        dustObject.SetActive(false);
        dustCleanedCount++;

        Debug.Log($"먼지 제거 완료 ({dustCleanedCount}/3)");

        EndDustCleaning();

        // 먼지 3개 전부 제거 시 → 2층 열쇠 드랍
        if (dustCleanedCount >= 3 && !InventoryManager.Instance.HasItem("Library_Key_Floor2"))
        {
            Debug.Log("2층 열쇠 획득");
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
        Debug.Log("End → Active=false");
    }
}
