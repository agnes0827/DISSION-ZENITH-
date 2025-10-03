using UnityEngine;

[CreateAssetMenu(fileName = "New Item Definition", menuName = "Inventory/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    [Header("기본 정보")]
    public string itemId;
    public string itemName;
    public Sprite itemIcon;

    [TextArea]
    public string itemDescription;
}