using UnityEngine;

/// <summary>
/// Ư�� ���� ���ӵ� UI ��ҵ��� �����ϰ� �����մϴ�.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("�� ���� UI �гε�")]
    [SerializeField] private GameObject inventoryPanel;
    // ... �� ������ ������ �ٸ� UI �гε� ...

    [Header("�κ��丮 UI�� Grid Layout")]
    [SerializeField] private Transform inventoryGridLayout;

    void Start()
    {
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
    }

    void Update()
    {
        // Tab Ű�� �κ��丮 ���� �ݱ�
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

        // �г��� Ȱ��ȭ�Ǵ� ������ �κ��丮 ������ ���� �޾ƿ�
        if (isActive)
        {
            if (InventoryManager.Instance != null && inventoryGridLayout != null)
            {
                InventoryManager.Instance.RegisterInventoryUI(inventoryGridLayout);
            }
            else
            {
                Debug.LogWarning("InventoryManager�� ã�� �� ���ų�, inventoryGridLayout�� ������� �ʾҽ��ϴ�.");
            }
        }
    }

    public void TogglePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }
}