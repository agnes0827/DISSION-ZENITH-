using UnityEngine;

[CreateAssetMenu(fileName = "New Item Definition", menuName = "Inventory/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    [Header("기본 정보")]
    public string itemId;
    public string itemName;
    public Sprite itemIcon;

    [Header("사용 관련 설정")]
    public bool canUseFromInventory;
    public int hpRestoreAmount;

    [TextArea]
    public string itemDescription;
}