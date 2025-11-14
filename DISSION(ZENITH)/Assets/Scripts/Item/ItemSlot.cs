using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    [Header("UI 표시 요소")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI amountText;

    [Header("슬롯 버튼 & 배경")]
    [SerializeField] private Button slotButton; // 이 슬롯을 클릭하는 버튼
    [SerializeField] private Image backgroundImage; // 배경 이미지 (회색 처리용)


    private ItemData itemData;
    private ItemDefinition definition;
    private bool isUsable; // 인벤토리에서 바로 사용 가능한가?

    public void SetItem(ItemData data)
    {
        itemData = data;
        itemNameText.text = data.itemName;
        amountText.text = data.amount.ToString();

        // itemId로부터 Definition 가져오기
        definition = InventoryManager.Instance.itemDatabase.GetItemByID(data.itemId);

        if (definition != null)
        {
            isUsable = definition.canUseFromInventory;
        }
        else
        {
            isUsable = false;
        }

        ApplyUsableState();
    }

    // 수량 갱신
    public void UpdateAmount(int newAmount)
    {
        amountText.text = newAmount.ToString();
    }

    public string GetItemId()
    {
        return itemData.itemId;
    }

    // 슬롯 클릭 시 (Button OnClick에 연결)
    public void OnClickSlot()
    {
        if (!isUsable)
        {
            // 사용 불가능한 아이템이면 아무 일도 하지 않음
            return;
        }

        if (definition == null) return;

        // 사용 가능한 아이템이면 확인 패널 띄우기
        ItemUsePanel.Instance.Show(definition);
    }

    private void ApplyUsableState()
    {
        // 1) 버튼 인터랙션 막기 / 풀기
        if (slotButton != null)
        {
            slotButton.interactable = isUsable;  // false면 클릭 자체가 비활성화됨
        }

        // 2) 배경 색 변경 (사용 불가능 = 회색)
        if (backgroundImage != null)
        {
            if (isUsable)
            {
                // HEX 코드 → Color로 변환
                Color customColor;
                ColorUtility.TryParseHtmlString("#49FFEB", out customColor);
                backgroundImage.color = customColor;
            }
            else
            {
                backgroundImage.color = new Color(0.6f, 0.6f, 0.6f, 1f); // 회색
            }
        }
    }
}
