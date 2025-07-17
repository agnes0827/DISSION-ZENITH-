using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryMenu; // 인벤토리 메뉴 인스펙터에 연결
    public WeaponSlotManager weaponSlotManager;
    public Enemy enemy; // 데미지를 받을 대상 (스크립트 연결)

    void Start()
    {
        StartBattle();
    }

    void StartBattle()
    {
        inventoryMenu.SetActive(true); // 전투 시작 시 인벤토리 자동 오픈
    }

    public void OnWeaponSlotClicked(int slotIndex)
    {
        WeaponData data = weaponSlotManager.GetWeaponData(slotIndex);
        if (data != null)
        {
            Debug.Log($"무기 {data.name} 사용! 공격력: {data.power}");
            enemy.TakeDamage(data.power);
        }
    }
}
