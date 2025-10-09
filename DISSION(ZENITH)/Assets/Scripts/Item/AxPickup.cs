using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxPickup : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject uiTextObject;   // Canvas 안의 "F 키 누르고 아이템 수집" Text 오브젝트
    [SerializeField] string playerTag = "Player";

    [Header("Item")]
    [SerializeField] private string itemId = "axe"; // 도끼의 ItemDefinition.itemId
    [SerializeField] private int amount = 1; // 필요 시 수량(현재 1개만)

    private bool isPlayerInRange = false;
    private bool picked = false;

    void Start()
    {
        if (uiTextObject) uiTextObject.SetActive(false);
        var col = GetComponent<Collider2D>();
        if (col && !col.isTrigger)
            Debug.LogWarning("[AxPickup] Collider2D는 IsTrigger 체크가 필요합니다.");
    }
    void Update()
    {
        // 플레이어가 범위 안에 있고 아직 안 주웠을 때만 F키 감지
        if (!isPlayerInRange || picked) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            Pickup();
        }
    }

    private void Pickup()
    {
        picked = true;
        Debug.Log("F 키 입력 감지: 도끼를 수집합니다!");

        // UI 끄기 & 다시 못 줍도록 콜라이더 비활성화
        if (uiTextObject) uiTextObject.SetActive(false);
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        // 인벤토리에 추가
        if (InventoryManager.Instance != null)
        {
            // 프로젝트에서 쓰는 AddItem 시그니처에 맞춰 한 줄만 선택해 남기기

            // (1) AddItem(string itemId) 버전일 때:
            // InventoryManager.Instance.AddItem(itemId);

            // (2) AddItem(string itemId, string itemName) 버전일 때:
            InventoryManager.Instance.AddItem(itemId, null); // itemName은 DB에서 채우므로 null 전달
            Debug.Log($"AddItem 호출 완료: {itemId}");

            // amount > 1을 지원하려면 for 루프 사용
            // for (int i = 0; i < Mathf.Max(1, amount); i++)
            // InventoryManager.Instance.AddItem(itemId);
        }
        else
        {
            Debug.LogWarning("[AxPickup] InventoryManager.Instance가 없습니다.");
        }

        // 픽업 오브젝트 제거(이펙트/사운드가 있으면 여기서 재생)
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (picked) return;
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = true;
            if (uiTextObject) uiTextObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = false;
            Debug.Log("플레이어가 도끼 범위 안에 들어옴");
            if (uiTextObject) uiTextObject.SetActive(false);
        }
    }
}
