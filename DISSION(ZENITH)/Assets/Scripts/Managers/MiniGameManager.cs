using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance;

    [SerializeField] private GameObject dustCleaningUIPanel;
    [SerializeField] private GameObject dustKeyPrefab; // 2�� ���� ������

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

    // ���� �ϳ� ���� ���� �� ȣ���
    private void OnDustCleaned(GameObject dustObject)
    {
        dustObject.SetActive(false);
        dustCleanedCount++;

        Debug.Log($"���� ���� �Ϸ�! ({dustCleanedCount}/3)");

        // ���� 3�� ���� ���� �� �� 2�� ���� ���
        if (dustCleanedCount >= 3)
        {
            DropSecondFloorKey();
        }
    }

    private void DropSecondFloorKey()
    {
        if (dustKeyPrefab != null)
        {
            Vector3 dropPosition = new Vector3(5f, 2f, 0f); // ���ϴ� ��ǥ�� ���� ����
            Instantiate(dustKeyPrefab, dropPosition, Quaternion.identity);
            Debug.Log("2�� ���� ���!");
        }
    }
}
