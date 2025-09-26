using UnityEngine;

// �� UIManager�� �ڽ��� ���� ���� UI�� �����մϴ�.
// Canvas ���� ������Ʈ�� �ٿ��� ����ϴ� ���� �����մϴ�.
public class UIManager_SceneSpecific : MonoBehaviour // ��ũ��Ʈ �̸� ������ ����
{
    [Header("�� ���� UI �гε�")]
    [SerializeField] private GameObject inventoryPanel;
    // ... �� ������ ������ �ٸ� UI �гε� ...

    [Header("�κ��丮 UI�� Grid Layout")]
    // �κ��丮 �����͸� ǥ���ϱ� ���� InventoryManager�� ����� Grid Layout
    [SerializeField] private Transform inventoryGridLayout;

    void Awake()
    {
        // Boot������ �Ѿ�� InventoryManager�� �����Ѵٸ�,
        // �� ���� �κ��丮 UI�� ����� �� �ֵ��� ����մϴ�.
        if (InventoryManager.Instance != null && inventoryGridLayout != null)
        {
            InventoryManager.Instance.RegisterInventoryUI(inventoryGridLayout);
        }
        else
        {
            Debug.LogWarning("InventoryManager�� ã�� �� ���ų�, inventoryGridLayout�� ������� �ʾҽ��ϴ�.");
        }
    }

    void Start()
    {
        // ���� ���۵� �� ��� UI �г��� ��Ȱ��ȭ ���·� �ʱ�ȭ�մϴ�.
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
    }

    void Update()
    {
        // Tab Ű�� �κ��丮 ���� �ݱ�
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePanel(inventoryPanel);
        }
    }

    // UI �г��� �Ѱ� ���� �Լ�
    public void TogglePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }
}