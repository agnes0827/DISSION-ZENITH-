using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인벤토리의 모든 로직을 관리하는 중앙 컨트롤러입니다.
/// 아이템 데이터는 GameStateManager에 저장하며, 이 매니저는 데이터에 접근하여
/// 아이템을 추가/제거하고 UI를 새로고침하는 등의 기능을 담당합니다.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("아이템 슬롯 프리팹")]
    public GameObject itemSlotPrefab;

    [Header("데이터베이스 연결")]
    public ItemDatabase itemDatabase;

    [Header("아이템이 추가될 그리드 레이아웃")]
    private Transform gridLayout;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterInventoryUI(Transform grid)
    {
        gridLayout = grid;
        Debug.Log("Inventory UI가 InventoryManager에 성공적으로 등록되었습니다.");
        RedrawInventoryUI();
    }

    // 아이템 추가
    public void AddItem(string itemId, string itemName)
    {
        // item을 GameStateManager에 등록
        var inventory = GameStateManager.Instance.inventoryItems;
        if (inventory.ContainsKey(itemId))
        {
            inventory[itemId]++; // 수량 증가
        }
        else
        {
            inventory.Add(itemId, 1); // 새 아이템 등록
        }

        // 2. 데이터가 변경되었으니 UI를 새로고침합니다.
        RedrawInventoryUI();

        // 3. 퀘스트 매니저에 알림
        QuestManager.Instance?.CheckQuestItem(itemId);
    }
    public bool HasItem(string itemId)
    {
        // GameStateManager의 데이터를 확인
        return GameStateManager.Instance.inventoryItems.ContainsKey(itemId);
    }

    public void RedrawInventoryUI()
    {
        if (gridLayout == null) return;

        // 1. 기존에 생성된 모든 슬롯 UI를 삭제
        foreach (Transform child in gridLayout)
        {
            Destroy(child.gameObject);
        }

        // 2. GameStateManager에서 최신 아이템 목록 불러오기
        var inventory = GameStateManager.Instance.inventoryItems;

        // 3. 목록의 모든 아이템에 대해 UI 슬롯을 처음부터 새로 생성
        foreach (var itemPair in inventory)
        {
            string itemId = itemPair.Key;
            int amount = itemPair.Value;

            ItemDefinition definition = itemDatabase.GetItemByID(itemId);

            if (definition == null)
            {
                Debug.LogWarning($"{itemId}에 해당하는 ItemDefinition을 데이터베이스에서 찾을 수 없습니다.");
                continue;
            }

            GameObject slotObj = Instantiate(itemSlotPrefab, gridLayout);
            ItemSlot newSlot = slotObj.GetComponent<ItemSlot>();

            ItemData newData = new ItemData
            {
                itemId = itemId,
                itemName = definition.itemName, // 데이터베이스에서 찾은 진짜 이름 사용
                amount = amount
            };

            newSlot.SetItem(newData);
        }
    }
}
