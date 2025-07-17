using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryMenu; // �κ��丮 �޴� �ν����Ϳ� ����

    void Start()
    {
        StartBattle();
    }

    void StartBattle()
    {
        inventoryMenu.SetActive(true); // ���� ���� �� �κ��丮 �ڵ� ����
    }
}
