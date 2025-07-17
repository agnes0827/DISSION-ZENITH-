using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryMenu; // 인벤토리 메뉴 인스펙터에 연결

    void Start()
    {
        StartBattle();
    }

    void StartBattle()
    {
        inventoryMenu.SetActive(true); // 전투 시작 시 인벤토리 자동 오픈
    }
}
