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
    public void AddItem(string itemId)
    {
        AddItem(itemId, null); // 이름은 DB에서 가져옴
    }

    public void AddItem(string itemId, string itemName)
    {
        // GameStateManager의 원본 인벤토리 사전에 접근
        var inventory = GameStateManager.Instance.inventoryItems;
        // 이미 있으면 수량만 증가, 없으면 신규 키로 1개 등록
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
        if (gridLayout == null)
        {
            UIManager ui = FindObjectOfType<UIManager>();
            if (ui != null)
            {
                ui.ForceRegisterInventory();
            }
            if (gridLayout == null)
            {
                Debug.LogWarning("Inventory UI 연결 실패로 인해 Redraw 중단");
                return;
            }
        }

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

    // 아이템 소비
    public bool ConsumeItem(string itemId, int amount = 1)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("ConsumeItem 호출 시 amount는 1 이상이어야 합니다.");
            return false;
        }

        var inventory = GameStateManager.Instance.inventoryItems;

        // 해당 아이템이 없으면 실패
        if (!inventory.ContainsKey(itemId))
        {
            Debug.LogWarning($"ConsumeItem 실패: 인벤토리에 {itemId}가 없습니다.");
            return false;
        }

        int before = inventory[itemId];
        // 수량이 부족한 경우
        if (before < amount)
        {
            Debug.LogWarning($"[Inventory] Consume FAIL: not enough '{itemId}' ({before} < {amount})");
            return false;
        }

        // 수량 감소
        inventory[itemId] -= amount;
        int after = inventory.ContainsKey(itemId) ? inventory[itemId] : 0;

        // 0개가 되면 인벤토리에서 삭제
        if (inventory[itemId] <= 0)
        {
            inventory.Remove(itemId);
            Debug.Log($"[Inventory] Removed '{itemId}' (went from {before} -> 0)");
        }
        else
        {
            Debug.Log($"[Inventory] Consumed '{itemId}' ({before} -> {inventory[itemId]})");
        }

        // UI 갱신
        RedrawInventoryUI();
        return true;
    }
}
