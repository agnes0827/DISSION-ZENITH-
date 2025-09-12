using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Text itemNameText;
    public Text amountText;

    private ItemData itemData;

    public void SetItem(ItemData data)
    {
        itemData = data;
        itemNameText.text = data.itemName;
        amountText.text = data.amount.ToString();
    }

    // ���� ����
    public void UpdateAmount(int newAmount)
    {
        amountText.text = newAmount.ToString();
    }

    public string GetItemId()
    {
        return itemData.itemId;
    }
}
