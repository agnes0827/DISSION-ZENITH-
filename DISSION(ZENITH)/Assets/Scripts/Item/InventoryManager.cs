using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �κ��丮�� ��� ������ �����ϴ� �߾� ��Ʈ�ѷ��Դϴ�.
/// ������ �����ʹ� GameStateManager�� �����ϸ�, �� �Ŵ����� �����Ϳ� �����Ͽ�
/// �������� �߰�/�����ϰ� UI�� ���ΰ�ħ�ϴ� ���� ����� ����մϴ�.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("������ ���� ������")]
    public GameObject itemSlotPrefab;

    [Header("�����ͺ��̽� ����")]
    public ItemDatabase itemDatabase;

    [Header("�������� �߰��� �׸��� ���̾ƿ�")]
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
        Debug.Log("Inventory UI�� InventoryManager�� ���������� ��ϵǾ����ϴ�.");
        RedrawInventoryUI();
    }

    // ������ �߰�
    public void AddItem(string itemId, string itemName)
    {
        // item�� GameStateManager�� ���
        var inventory = GameStateManager.Instance.inventoryItems;
        if (inventory.ContainsKey(itemId))
        {
            inventory[itemId]++; // ���� ����
        }
        else
        {
            inventory.Add(itemId, 1); // �� ������ ���
        }

        // 2. �����Ͱ� ����Ǿ����� UI�� ���ΰ�ħ�մϴ�.
        RedrawInventoryUI();

        // 3. ����Ʈ �Ŵ����� �˸�
        QuestManager.Instance?.CheckQuestItem(itemId);
    }
    public bool HasItem(string itemId)
    {
        // GameStateManager�� �����͸� Ȯ��
        return GameStateManager.Instance.inventoryItems.ContainsKey(itemId);
    }

    public void RedrawInventoryUI()
    {
        if (gridLayout == null) return;

        // 1. ������ ������ ��� ���� UI�� ����
        foreach (Transform child in gridLayout)
        {
            Destroy(child.gameObject);
        }

        // 2. GameStateManager���� �ֽ� ������ ��� �ҷ�����
        var inventory = GameStateManager.Instance.inventoryItems;

        // 3. ����� ��� �����ۿ� ���� UI ������ ó������ ���� ����
        foreach (var itemPair in inventory)
        {
            string itemId = itemPair.Key;
            int amount = itemPair.Value;

            ItemDefinition definition = itemDatabase.GetItemByID(itemId);

            if (definition == null)
            {
                Debug.LogWarning($"{itemId}�� �ش��ϴ� ItemDefinition�� �����ͺ��̽����� ã�� �� �����ϴ�.");
                continue;
            }

            GameObject slotObj = Instantiate(itemSlotPrefab, gridLayout);
            ItemSlot newSlot = slotObj.GetComponent<ItemSlot>();

            ItemData newData = new ItemData
            {
                itemId = itemId,
                itemName = definition.itemName, // �����ͺ��̽����� ã�� ��¥ �̸� ���
                amount = amount
            };

            newSlot.SetItem(newData);
        }
    }
}
