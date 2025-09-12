using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI ����")]
    public Transform gridLayout; // GridLayoutGroup ������Ʈ
    public GameObject itemSlotPrefab; // Item ���� ������

    private Dictionary<string, ItemSlot> itemSlots = new(); // ������ ID �� ����

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ������ �߰�
    public void AddItem(string itemId, string itemName)
    {
        if (itemSlots.ContainsKey(itemId))
        {
            // �̹� ������ ���� ����
            ItemSlot slot = itemSlots[itemId];
            int current = int.Parse(slot.amountText.text);
            int updated = current + 1;
            slot.UpdateAmount(updated);
        }
        else
        {
            // ������ �� ���� ����
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
