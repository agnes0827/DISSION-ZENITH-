using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Item Database", menuName = "Inventory/Item Database")]

/// <summary>
/// ScriptableObject 기반의 아이템 데이터베이스 애셋입니다.
/// 게임에 존재하는 모든 아이템의 원본 정보(ItemDefinition) 목록을 가지고 있으며,
/// 아이템 ID를 통해 이름, 아이콘 등의 상세 정보를 조회하는 기능을 제공합니다.
/// </summary>
public class ItemDatabase : ScriptableObject
{
    public List<ItemDefinition> allItems;

    public ItemDefinition GetItemByID(string id)
    {
        return allItems.FirstOrDefault(item => item.itemId == id);
    }
}