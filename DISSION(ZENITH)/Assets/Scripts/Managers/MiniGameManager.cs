using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance;

    [SerializeField] private GameObject dustCleaningUIPanel;

    private int dustCleanedCount = 0;

    public bool IsMiniGameActive { get; private set; } = false; // �̴ϰ��� ���� ����

    void Awake()
    {
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
        Debug.Log("Start �� Active=true");
    }

    // ���� �ϳ� ���� ���� �� ȣ���
    private void OnDustCleaned(GameObject dustObject)
    {
        dustObject.SetActive(false);
        dustCleanedCount++;

        Debug.Log($"���� ���� �Ϸ�! ({dustCleanedCount}/3)");

        EndDustCleaning();

        // ���� 3�� ���� ���� �� �� 2�� ���� ���
        if (dustCleanedCount >= 3)
        {
            Debug.Log("2�� ���� ���!");
        }
    }

    public void EndDustCleaning()
    {
        IsMiniGameActive = false;
        dustCleaningUIPanel.SetActive(false);
        Debug.Log("End �� Active=false");
    }
}
