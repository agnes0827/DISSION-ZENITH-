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
    public HpBar playerHealthBar; // �÷��̾� ü�¹�

    public GameObject panel; // �ڸ��� ��� �г�
    public Text DialogText; // ���̾�α� �ؽ�Ʈ

    public Image fadePanel; // ȭ�� ���̵�ƿ�
    public float fadeDuration = 1.5f; // ���̵�ƿ� �ð�

    private bool battleEnded = false;

    private string[] enemyWeapons = { "��", "Į" }; // ���ʹ̰� ����ϴ� ���� ����
    private Dictionary<string, int> enemyDamageMap = new Dictionary<string, int>()
    {
        {"��", 10 },
        {"Į", 15 }
    }; // �� ���� �� ���ݷ� ����

    void Start()
    {
        playerHealthBar.Initialize(playerHP); // ���� �� ü�¹� �ʱ�ȭ
        enemy.OnDamaged += HandleEnemyDamaged;
        enemy.OnDied += HandleEnemyDied;

        StartBattle();
    }

    void HandleEnemyDamaged(int currentHP)
    {
        DialogText.text = $"���� ���ظ� �Ծ���! ���� ü��: {currentHP}";

        if (currentHP <= 0 && !battleEnded)
        {
            EndBattle();
        }
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

            // �� ü�¹� ���� (Enemy�� ü�¹� ���� �ʿ�)
            if (enemy.healthBar != null)
                enemy.healthBar.UpdateHealth(enemy.hp);

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
        yield return new WaitForSeconds(2f); // 2�� ��� (�����)

        string selectedWeapon = enemyWeapons[Random.Range(0, enemyWeapons.Length)]; // ���� �� �� �ϳ� ���
        int damage = enemyDamageMap[selectedWeapon];

        playerHP -= damage;
        if (playerHP < 0) playerHP = 0;

        playerHealthBar.UpdateHealth(playerHP); // ü�¹� ����

        DialogText.text = $"���� {selectedWeapon}(��)�� ����! �÷��̾ {damage} ���ظ� �Ծ���. ���� ü��: {playerHP}";

        if (playerHP <= 0 && !battleEnded)
        {
            DialogText.text = "�÷��̾ ��������!"; // ���� ���� ó��
            EndBattle();
        }
    }

    void EndBattle()
    {
        battleEnded = true;
        fadePanel.gameObject.SetActive(true); 
        StartCoroutine(FadeOutAndClose());
    }

    IEnumerator FadeOutAndClose()
    {
        float elapsed = 0f;
        Color c = fadePanel.color;

        // ���̵� �ƿ�
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }

        // ���� �ý��� ���� �� ������ ��Ȱ��ȭ
        // gameObject.SetActive(false);

        // �� ��ȯ(���� �÷��̾ ���� �Ǹ� �÷��̾� ���ϴ� ȭ�� ���� �� ������)
        // SceneManager.LoadScene("������");
    }
}