using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject battleInventoryMenu; // ��Ʋ �κ��丮 �޴� �ν����Ϳ� ����
    public WeaponSlotManager weaponSlotManager;
    public Enemy enemy; // �������� ���� ��� (��ũ��Ʈ ����)

    public int playerHP = 100; // �÷��̾� �⺻ ü��

    public GameObject panel; // �ڸ��� ��� �г�
    public Text DialogText; // ���̾�α� �ؽ�Ʈ

    public Text EnemyHpText; // �� ü�� �ؽ�Ʈ
    public Text PlayerHpText; // �÷��̾� ü�� �ؽ�Ʈ

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
        enemy.OnDamaged += HandleEnemyDamaged;
        enemy.OnDied += HandleEnemyDied;

        StartBattle();
    }

    void HandleEnemyDamaged(int currentHP, int damage)
    {
        DialogText.text = $"���� {damage} ���ظ� �Ծ���!";
        EnemyHpText.text = $"{damage} ����, ���� ü��: {currentHP}";
    }

    void HandleEnemyDied()
    {
        EnemyHpText.enabled = false;
        PlayerHpText.enabled = false;
        DialogText.text = "���� ��������!";
        // ���������� �ο� ���͸� óġ �Ϸ�� ���
        if (LibraryGameState.Instance != null)
            LibraryGameState.Instance.MarkDefeated(LibraryGameState.Instance.lastMonsterId);

        // Library�� ���� (���̵�ƿ� �ڷ�ƾ �ȿ��� ȣ���ص� OK)
        EndBattle("Library"); // �ռ� ���� nextScene ���� �޴� ���� ���
    }

    void StartBattle()
    {
        battleInventoryMenu.SetActive(true); // ���� ���� �� �κ��丮 �ڵ� ����
    }

    public void OnWeaponSlotClicked(int slotIndex)
    {
        WeaponData data = weaponSlotManager.GetWeaponData(slotIndex);
        if (data != null)
        {
            panel.SetActive(true); // �г� Ȱ��ȭ
            StartCoroutine(ShowWeaponUseAndAttack(data));
        }
    }

    // �÷��̾ �������� �� ȣ��Ǵ� �Լ�
    public void OnPlayerAttack()
    {
        // ��� ��� �� ���� �ݰ�
        StartCoroutine(EnemyCounterAttack());
    }

    IEnumerator ShowWeaponUseAndAttack(WeaponData data)
    {
        int damage = Random.Range(data.minDamage, data.maxDamage + 1); // ���ݷ� ����

        DialogText.text = $"���� {data.name} ���!";

        yield return new WaitForSeconds(1.2f);
        enemy.TakeDamage(damage); // �� ü�� ����
        enemy.GetComponent<DamageFlash>().FlashRed(1f); // �� �̹��� ������
        yield return new WaitForSeconds(1.5f);
        OnPlayerAttack(); // �� �ݰ� ����
    }

    IEnumerator EnemyCounterAttack()
    {
        yield return new WaitForSeconds(1.5f); // 2�� ��� (�����)

        string selectedWeapon = enemyWeapons[Random.Range(0, enemyWeapons.Length)]; // ���� �� �� �ϳ� ���
        int damage = enemyDamageMap[selectedWeapon];

        playerHP -= damage;
        if (playerHP < 0) playerHP = 0;

        DialogText.text = $"���� {selectedWeapon}(��)�� ����!";
        PlayerHpText.text = $"{damage} ����, ���� ü��: {playerHP}";

        GameObject.Find("Player").GetComponent<DamageFlash>().FlashRed(1f); // �÷��̾� �̹��� ������

        if (playerHP <= 0 && !battleEnded)
        {
            EnemyHpText.enabled = false;
            PlayerHpText.enabled = false;
            DialogText.text = "�÷��̾ ��������!"; // ���� ���� ó��
            EndBattle("DialogueTest");
        }
    }

    void EndBattle(string nextScene)
    {
        if (battleEnded) return;
        battleEnded = true;

        fadePanel.gameObject.SetActive(true);
        StartCoroutine(FadeOutAndClose(nextScene));
    }

    IEnumerator FadeOutAndClose(string nextScene)
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
        gameObject.SetActive(false);

        // �� ��ȯ
        // ������ ������ �̵�
        SceneManager.LoadScene(nextScene);
    }
}