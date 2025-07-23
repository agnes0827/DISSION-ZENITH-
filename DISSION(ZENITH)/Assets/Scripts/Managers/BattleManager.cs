using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryMenu; // �κ��丮 �޴� �ν����Ϳ� ����
    public WeaponSlotManager weaponSlotManager;
    public Enemy enemy; // �������� ���� ��� (��ũ��Ʈ ����)
    public int playerHP = 100; // �÷��̾� �⺻ ü��
    private string[] enemyWeapons = { "��", "Į" }; // ���ʹ̰� ����ϴ� ���� ����
    private Dictionary<string, int> enemyDamageMap = new Dictionary<string, int>()
    {
        {"��", 10 },
        {"Į", 15 }
    }; // �� ���� �� ���ݷ� ����

    void Start()
    {
        StartBattle();
    }

    void StartBattle()
    {
        inventoryMenu.SetActive(true); // ���� ���� �� �κ��丮 �ڵ� ����
    }

    public void OnWeaponSlotClicked(int slotIndex)
    {
        WeaponData data = weaponSlotManager.GetWeaponData(slotIndex);
        WeaponSlotUI slotUI = weaponSlotManager.weaponSlots[slotIndex];

        if (data != null && slotUI.CanUse())
        {
            Debug.Log($"���� {data.name} ���! ���ݷ�: {data.power}");
            enemy.TakeDamage(data.power);

            slotUI.UseWeapon(); // ��� Ƚ�� ���� �� ��ư ��Ȱ��ȭ ó��

            OnPlayerAttack(); // �� �ݰ� �ڷ�ƾ ����
        }
        else
        {
            Debug.Log("�� ����� �� �̻� ����� �� �����ϴ�!");
        }
    }

    // �÷��̾ �������� �� ȣ��Ǵ� �Լ�
    public void OnPlayerAttack()
    {
        // ��� ��� �� ���� �ݰ�
        StartCoroutine(EnemyCounterAttack());
    }

    IEnumerator EnemyCounterAttack()
    {
        yield return new WaitForSeconds(1f); // 1�� ��� (�����)

        string selectedWeapon = enemyWeapons[Random.Range(0, enemyWeapons.Length)]; // ���� �� �� �ϳ� ���
        int damage = enemyDamageMap[selectedWeapon];

        playerHP -= damage;
        Debug.Log($"���� {selectedWeapon}���� ����! �÷��̾ {damage} ���ظ� �Ծ���. ���� ü��: {playerHP}");

        if (playerHP <= 0)
        {
            Debug.Log("�÷��̾ ��������!");
            // ���� ���� ó��
        }
    }
}
