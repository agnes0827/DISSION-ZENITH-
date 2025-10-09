using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxPickup : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject uiTextObject;   // Canvas ���� "F Ű ������ ������ ����" Text ������Ʈ
    [SerializeField] string playerTag = "Player";

    [Header("Item")]
    [SerializeField] private string itemId = "axe"; // ������ ItemDefinition.itemId
    [SerializeField] private int amount = 1; // �ʿ� �� ����(���� 1����)

    private bool isPlayerInRange = false;
    private bool picked = false;

    void Start()
    {
        if (uiTextObject) uiTextObject.SetActive(false);
        var col = GetComponent<Collider2D>();
        if (col && !col.isTrigger)
            Debug.LogWarning("[AxPickup] Collider2D�� IsTrigger üũ�� �ʿ��մϴ�.");
    }
    void Update()
    {
        // �÷��̾ ���� �ȿ� �ְ� ���� �� �ֿ��� ���� FŰ ����
        if (!isPlayerInRange || picked) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            Pickup();
        }
    }

    private void Pickup()
    {
        picked = true;
        Debug.Log("F Ű �Է� ����: ������ �����մϴ�!");

        // UI ���� & �ٽ� �� �ݵ��� �ݶ��̴� ��Ȱ��ȭ
        if (uiTextObject) uiTextObject.SetActive(false);
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        // �κ��丮�� �߰�
        if (InventoryManager.Instance != null)
        {
            // ������Ʈ���� ���� AddItem �ñ״�ó�� ���� �� �ٸ� ������ �����

            // (1) AddItem(string itemId) ������ ��:
            // InventoryManager.Instance.AddItem(itemId);

            // (2) AddItem(string itemId, string itemName) ������ ��:
            InventoryManager.Instance.AddItem(itemId, null); // itemName�� DB���� ä��Ƿ� null ����
            Debug.Log($"AddItem ȣ�� �Ϸ�: {itemId}");

            // amount > 1�� �����Ϸ��� for ���� ���
            // for (int i = 0; i < Mathf.Max(1, amount); i++)
            // InventoryManager.Instance.AddItem(itemId);
        }
        else
        {
            Debug.LogWarning("[AxPickup] InventoryManager.Instance�� �����ϴ�.");
        }

        // �Ⱦ� ������Ʈ ����(����Ʈ/���尡 ������ ���⼭ ���)
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
            Debug.Log("�÷��̾ ���� ���� �ȿ� ����");
            if (uiTextObject) uiTextObject.SetActive(false);
        }
    }
}
