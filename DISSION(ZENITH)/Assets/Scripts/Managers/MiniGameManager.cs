using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance;

    [SerializeField] private GameObject dustCleaningUIPanel;

    private DialogueManager dialogueManager;

    private int dustCleanedCount = 0;

    public static bool IsMiniGameActive { get; private set; } = false;

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
        Debug.Log("Start ¡æ Active=true");
    }

    // ¸ÕÁö ÇÏ³ª Á¦°Å ¼º°ø ½Ã È£ÃâµÊ
    private void OnDustCleaned(GameObject dustObject)
    {
        dustObject.SetActive(false);
        dustCleanedCount++;

        Debug.Log($"¸ÕÁö Á¦°Å ¿Ï·á ({dustCleanedCount}/3)");

        EndDustCleaning();

        // ¸ÕÁö 3°³ ÀüºÎ Á¦°Å ½Ã ¡æ 2Ãþ ¿­¼è µå¶ø
        if (dustCleanedCount >= 3 && !InventoryManager.Instance.HasItem("Library_Key_Floor2"))
        {
            Debug.Log("2Ãþ ¿­¼è È¹µæ");
            InventoryManager.Instance.AddItem("Library_Key_Floor2", "µµ¼­°ü 2Ãþ ¿­¼è");

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
        Debug.Log("End ¡æ Active=false");
    }
}
