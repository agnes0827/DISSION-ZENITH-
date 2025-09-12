using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI 연결")]
    public Transform gridLayout; // GridLayoutGroup 오브젝트
    public GameObject itemSlotPrefab; // Item 슬롯 프리팹

    private Dictionary<string, ItemSlot> itemSlots = new(); // 아이템 ID → 슬롯

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 아이템 추가
    public void AddItem(string itemId, string itemName)
    {
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
