using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Item Database", menuName = "Inventory/Item Database")]

/// <summary>
/// ScriptableObject ����� ������ �����ͺ��̽� �ּ��Դϴ�.
/// ���ӿ� �����ϴ� ��� �������� ���� ����(ItemDefinition) ����� ������ ������,
/// ������ ID�� ���� �̸�, ������ ���� �� ������ ��ȸ�ϴ� ����� �����մϴ�.
/// </summary>
public class ItemDatabase : ScriptableObject
{
    public List<ItemDefinition> allItems;

    public ItemDefinition GetItemByID(string id)
    {
        return allItems.FirstOrDefault(item => item.itemId == id);
    }
}