using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryMenu; // �κ��丮 �޴� �ν����Ϳ� ����
    public WeaponSlotManager weaponSlotManager;
    public Enemy enemy; // �������� ���� ��� (��ũ��Ʈ ����)
    public int playerHP = 100; // �÷��̾� �⺻ ü��
    public GameObject panel; // �ڸ��� ��� �г�
    public Text DialogText; // ���̾�α� �ؽ�Ʈ
    private string[] enemyWeapons = { "��", "Į" }; // ���ʹ̰� ����ϴ� ���� ����
    private Dictionary<string, int> enemyDamageMap = new Dictionary<string, int>()
    {
        {"��", 10 },
        {"Į", 15 }
    }; // �� ���� �� ���ݷ� ����

    void Start()
    {
        enemy.OnDamaged += HandleEnemyDamaged;
        enemy.OnDied += HandleEnemyDied;

        StartBattle();
    }

    void HandleEnemyDamaged(int currentHP)
    {
        DialogText.text = $"���� ���ظ� �Ծ���! ���� ü��: {currentHP}";
    }

    void HandleEnemyDied()
    {
        DialogText.text = "���� ��������!";
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
            panel.SetActive(true); // �г� Ȱ��ȭ
            int damage = Random.Range(data.minDamage, data.maxDamage + 1); // ���ݷ� ���� ���

            DialogText.text = $"���� {data.name} ���! ���ݷ�: {damage}";

            // ���⼭ ������ ������ �ֱ�
            enemy.TakeDamage(damage);

            // �� �ݰ� �ڷ�ƾ ����
            OnPlayerAttack(); 
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
        DialogText.text = $"���� {selectedWeapon}(��)�� ����! �÷��̾ {damage} ���ظ� �Ծ���. ���� ü��: {playerHP}";

        if (playerHP <= 0)
        {
            DialogText.text = "�÷��̾ ��������!"; // ���� ���� ó��
        }
    }
}