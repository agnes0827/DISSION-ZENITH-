using UnityEngine;

/// <summary>
/// 특정 씬에 종속된 UI 요소들을 관리하고 제어합니다.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("씬 전용 UI 패널들")]
    [SerializeField] private GameObject inventoryPanel;

    [Header("인벤토리 UI의 Grid Layout")]
    [SerializeField] private Transform inventoryGridLayout;

    void Start()
    {
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
    }

    void Update()
    {
        // Tab 키로 인벤토리 열고 닫기
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventoryPanel();
        }
    }

    public void ToggleInventoryPanel()
    {
        if (inventoryPanel == null) return;

        bool isActive = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isActive);

        SoundManager.Instance.PlaySFX(SfxType.UISelect, 0.7f, false);

        // 패널이 활성화되는 시점에 인벤토리 데이터를 다시 받아옴
        if (isActive)
        {
            Time.timeScale = 0f; // 게임 일시정지
            PlayerController.Instance.StopMovement();
            if (InventoryManager.Instance != null && inventoryGridLayout != null)
            {
                InventoryManager.Instance.RegisterInventoryUI(inventoryGridLayout);
            }
            else
            {
                Debug.LogWarning("InventoryManager를 찾을 수 없거나, inventoryGridLayout이 설정되지 않았습니다.");
            }
        }
        else
        {
            Time.timeScale = 1f; // 게임 시간을 원래 속도로 되돌림
            PlayerController.Instance.ResumeMovement();
        }
    }

    public void TogglePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }

    public void ForceRegisterInventory()
    {
        InventoryManager.Instance.RegisterInventoryUI(inventoryGridLayout);
    }
}
