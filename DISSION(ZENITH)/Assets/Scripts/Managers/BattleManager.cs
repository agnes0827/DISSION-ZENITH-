using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryMenu; // �κ��丮 �޴� �ν����Ϳ� ����
    public WeaponSlotManager weaponSlotManager;
    public Enemy enemy; // �������� ���� ��� (��ũ��Ʈ ����)

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
        if (data != null)
        {
            Debug.Log($"���� {data.name} ���! ���ݷ�: {data.power}");
            enemy.TakeDamage(data.power);
        }
    }
}
