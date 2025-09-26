using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("아이템 슬롯 프리팹")]
    public GameObject itemSlotPrefab;

    private Transform gridLayout;
    private Dictionary<string, ItemSlot> itemSlots = new();

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
    }

    // 아이템 추가
    public void AddItem(string itemId, string itemName)
    {
        if (gridLayout == null)
        {
            Debug.LogError("Inventory UI가 등록되지 않았습니다! 아이템을 추가할 수 없습니다.");
            return;
        }

        if (itemSlots.ContainsKey(itemId))
        {
            // 이미 있으면 수량 증가
            ItemSlot slot = itemSlots[itemId];
            int current = int.Parse(slot.amountText.text);
            int updated = current + 1;
            slot.UpdateAmount(updated);
        }
        else
        {
            // 없으면 새 슬롯 생성
            GameObject slotObj = Instantiate(itemSlotPrefab, gridLayout);
            ItemSlot newSlot = slotObj.GetComponent<ItemSlot>();

            ItemData newData = new ItemData
            {
                itemId = itemId,
                itemName = itemName,
                amount = 1
            };

            newSlot.SetItem(newData);
            itemSlots.Add(itemId, newSlot);
        }

        QuestManager.Instance?.CheckQuestItem(itemId);
    }

    public bool HasItem(string itemId)
    {
        return itemSlots.ContainsKey(itemId);
    }
}
