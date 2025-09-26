using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("������ ���� ������")]
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
        Debug.Log("Inventory UI�� InventoryManager�� ���������� ��ϵǾ����ϴ�.");
    }

    // ������ �߰�
    public void AddItem(string itemId, string itemName)
    {
        if (gridLayout == null)
        {
            Debug.LogError("Inventory UI�� ��ϵ��� �ʾҽ��ϴ�! �������� �߰��� �� �����ϴ�.");
            return;
        }

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
