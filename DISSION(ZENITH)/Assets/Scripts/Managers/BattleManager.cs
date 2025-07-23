using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryMenu; // 인벤토리 메뉴 인스펙터에 연결
    public WeaponSlotManager weaponSlotManager;
    public Enemy enemy; // 데미지를 받을 대상 (스크립트 연결)
    public int playerHP = 100; // 플레이어 기본 체력
    private string[] enemyWeapons = { "손", "칼" }; // 에너미가 사용하는 무기 나열
    private Dictionary<string, int> enemyDamageMap = new Dictionary<string, int>()
    {
        {"손", 10 },
        {"칼", 15 }
    }; // 적 무기 별 공격력 연결

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
        WeaponSlotUI slotUI = weaponSlotManager.weaponSlots[slotIndex];

        if (data != null && slotUI.CanUse())
        {
            Debug.Log($"무기 {data.name} 사용! 공격력: {data.power}");
            enemy.TakeDamage(data.power);

            slotUI.UseWeapon(); // 사용 횟수 차감 및 버튼 비활성화 처리

            OnPlayerAttack(); // 적 반격 코루틴 시작
        }
        else
        {
            Debug.Log("이 무기는 더 이상 사용할 수 없습니다!");
        }
    }

    // 플레이어가 공격했을 때 호출되는 함수
    public void OnPlayerAttack()
    {
        // 잠깐 대기 후 적의 반격
        StartCoroutine(EnemyCounterAttack());
    }

    IEnumerator EnemyCounterAttack()
    {
        yield return new WaitForSeconds(1f); // 1초 대기 (연출용)

        string selectedWeapon = enemyWeapons[Random.Range(0, enemyWeapons.Length)]; // 무기 둘 중 하나 사용
        int damage = enemyDamageMap[selectedWeapon];

        playerHP -= damage;
        Debug.Log($"적이 {selectedWeapon}으로 공격! 플레이어가 {damage} 피해를 입었다. 남은 체력: {playerHP}");

        if (playerHP <= 0)
        {
            Debug.Log("플레이어가 쓰러졌다!");
            // 게임 오버 처리
        }
    }
}
