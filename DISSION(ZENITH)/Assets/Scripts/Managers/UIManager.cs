using UnityEngine;

// 이 UIManager는 자신이 속한 씬의 UI만 제어합니다.
// Canvas 게임 오브젝트에 붙여서 사용하는 것을 권장합니다.
public class UIManager_SceneSpecific : MonoBehaviour // 스크립트 이름 변경을 권장
{
    [Header("이 씬의 UI 패널들")]
    [SerializeField] private GameObject inventoryPanel;
    // ... 이 씬에서 제어할 다른 UI 패널들 ...

    [Header("인벤토리 UI의 Grid Layout")]
    // 인벤토리 데이터를 표시하기 위해 InventoryManager에 등록할 Grid Layout
    [SerializeField] private Transform inventoryGridLayout;

    void Awake()
    {
        // Boot씬에서 넘어온 InventoryManager가 존재한다면,
        // 이 씬의 인벤토리 UI를 사용할 수 있도록 등록합니다.
        if (InventoryManager.Instance != null && inventoryGridLayout != null)
        {
            InventoryManager.Instance.RegisterInventoryUI(inventoryGridLayout);
        }
        else
        {
            Debug.LogWarning("InventoryManager를 찾을 수 없거나, inventoryGridLayout이 연결되지 않았습니다.");
        }
    }

    void Start()
    {
        // 씬이 시작될 때 모든 UI 패널을 비활성화 상태로 초기화합니다.
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
    }

    void Update()
    {
        // Tab 키로 인벤토리 열고 닫기
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePanel(inventoryPanel);
        }
    }

    // UI 패널을 켜고 끄는 함수
    public void TogglePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }
}