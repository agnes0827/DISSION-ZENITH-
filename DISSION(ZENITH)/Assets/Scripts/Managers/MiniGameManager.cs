using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance;

    [SerializeField] private GameObject dustCleaningUIPanel;
    [SerializeField] private GameObject dustKeyPrefab; // 2층 열쇠 프리팹

    private int dustCleanedCount = 0;

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
        dustCleaningUIPanel.SetActive(true);
        var cleaningGame = dustCleaningUIPanel.GetComponent<DustCleaningGame>();
        cleaningGame.BeginGame(dustObject, OnDustCleaned);
    }

    // 먼지 하나 제거 성공 시 호출됨
    private void OnDustCleaned(GameObject dustObject)
    {
        dustObject.SetActive(false);
        dustCleanedCount++;

        Debug.Log($"먼지 제거 완료! ({dustCleanedCount}/3)");

        // 먼지 3개 전부 제거 시 → 2층 열쇠 드랍
        if (dustCleanedCount >= 3)
        {
            DropSecondFloorKey();
        }
    }

    private void DropSecondFloorKey()
    {
        if (dustKeyPrefab != null)
        {
            Vector3 dropPosition = new Vector3(5f, 2f, 0f); // 원하는 좌표로 변경 가능
            Instantiate(dustKeyPrefab, dropPosition, Quaternion.identity);
            Debug.Log("2층 열쇠 드랍!");
        }
    }
}
