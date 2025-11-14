using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemUsePanel : MonoBehaviour
{
    public static ItemUsePanel Instance;

    [SerializeField] private GameObject panelRoot; // 전체 패널 오브젝트
    [SerializeField] private TextMeshProUGUI messageText;

    private ItemDefinition currentItem; // 지금 사용하려는 아이템

    void Awake()
    {
        Instance = this;
        panelRoot.SetActive(false);
    }

    public void Show(ItemDefinition itemDef)
    {
        currentItem = itemDef;

        // hpRestoreAmount에 맞춰 문구 생성
        if (itemDef.hpRestoreAmount > 0)
        {
            messageText.text =
                $"이 아이템 사용 시\n" +
                $"체력이 {itemDef.hpRestoreAmount} 회복됩니다.\n" +
                $"사용하시겠습니까?";
        }
        else
        {
            // 혹시 다른 타입의 소비 아이템이 생길 걸 대비한 기본 문구
            messageText.text = "이 아이템을 사용하시겠습니까?";
        }

        panelRoot.SetActive(true);
        Time.timeScale = 0f; // 필요하면 일시정지
    }

    public void OnClickYes()
    {
        // 먼저 인벤토리에서 1개 소비 시도
        bool consumed = InventoryManager.Instance.ConsumeItem(currentItem.itemId, 1);

        if (consumed)
        {
            // 소비가 성공했을 때만 HP 변경
            if (currentItem.hpRestoreAmount != 0)
            {
                GameStateManager.Instance.ChangeHP(currentItem.hpRestoreAmount);
            }
        }
        else
        {
            Debug.LogWarning("아이템 소비에 실패했습니다.");
        }
        Close();
    }

    private void Close()
    {
        panelRoot.SetActive(false);
        Time.timeScale = 1f;
        currentItem = null;
    }
}
