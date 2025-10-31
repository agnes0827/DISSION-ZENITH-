using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    [Header("아이템 정보")]
    [Tooltip("ItemDatabase에 정의된 고유 ID")]
    public string itemId;

    [Tooltip("이 '씬 오브젝트'의 고유 ID. 씬 내에서 절대 중복 금지.")]
    public string scenePickupID; // 예: "Library_Potion_01", "Hallway_Key_Pickup"

    [Tooltip("참조할 아이템 데이터베이스")]
    public ItemDatabase itemDatabase;

    public string desiredColor = "ffc200";
    private bool isPlayerInRange = false;

    private void Start()
    {
        if (GameStateManager.Instance == null) return;

        if (GameStateManager.Instance.collectedSceneObjectIDs.Contains(scenePickupID))
        {
            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            PickupItem();
        }
    }

    private void PickupItem()
    {
        if (GameStateManager.Instance == null || itemDatabase == null)
        {
            Debug.LogError("GameStateManager 또는 ItemDatabase가 없습니다!");
            return;
        }

        // 1. 데이터베이스에서 아이템 '이름' 찾기
        ItemDefinition definition = itemDatabase.GetItemByID(itemId);

        // definition.itemName은 "도서관 2층 열쇠"
        string itemName = (definition != null) ? definition.itemName : itemId;
        string highlightedName = $"<color=#{desiredColor}>{itemName}</color>";

        // 2. 인벤토리 매니저에 'ID'로 아이템 추가
        InventoryManager.Instance.AddItem(itemId);
        GameStateManager.Instance.collectedSceneObjectIDs.Add(scenePickupID);

        // 3. 다이얼로그 매니저에 '이름'으로 알림 요청 (UI 표시)
        DialogueManager.Instance.ShowItemGetNotice(highlightedName);

        // 4. 아이템 오브젝트 파괴
        Destroy(gameObject);
    }
}