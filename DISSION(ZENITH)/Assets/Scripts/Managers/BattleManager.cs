using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryMenu; // 인벤토리 메뉴 인스펙터에 연결
    public WeaponSlotManager weaponSlotManager;
    public Enemy enemy; // 데미지를 받을 대상 (스크립트 연결)

    public int playerHP = 100; // 플레이어 기본 체력
    public HpBar playerHealthBar; // 플레이어 체력바

    public GameObject panel; // 자막을 띄울 패널
    public Text DialogText; // 다이얼로그 텍스트

    public Image fadePanel; // 화면 페이드아웃
    public float fadeDuration = 1.5f; // 페이드아웃 시간

    private bool battleEnded = false;

    private string[] enemyWeapons = { "손", "칼" }; // 에너미가 사용하는 무기 나열
    private Dictionary<string, int> enemyDamageMap = new Dictionary<string, int>()
    {
        {"손", 10 },
        {"칼", 15 }
    }; // 적 무기 별 공격력 연결

    void Start()
    {
        playerHealthBar.Initialize(playerHP); // 시작 시 체력바 초기화
        enemy.OnDamaged += HandleEnemyDamaged;
        enemy.OnDied += HandleEnemyDied;

        StartBattle();
    }

    void HandleEnemyDamaged(int currentHP)
    {
        DialogText.text = $"적이 피해를 입었다! 남은 체력: {currentHP}";

        if (currentHP <= 0 && !battleEnded)
        {
            EndBattle();
        }
    }

    void HandleEnemyDied()
    {
        DialogText.text = "적이 쓰러졌다!";
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
            panel.SetActive(true); // 패널 활성화
            int damage = Random.Range(data.minDamage, data.maxDamage + 1); // 공격력 랜덤 계산

            DialogText.text = $"무기 {data.name} 사용! 공격력: {damage}";

            // 여기서 적에게 데미지 주기
            enemy.TakeDamage(damage);

            // 적 체력바 갱신 (Enemy의 체력바 참조 필요)
            if (enemy.healthBar != null)
                enemy.healthBar.UpdateHealth(enemy.hp);

            // 적 반격 코루틴 시작
            OnPlayerAttack(); 
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
        yield return new WaitForSeconds(2f); // 2초 대기 (연출용)

        string selectedWeapon = enemyWeapons[Random.Range(0, enemyWeapons.Length)]; // 무기 둘 중 하나 사용
        int damage = enemyDamageMap[selectedWeapon];

        playerHP -= damage;
        if (playerHP < 0) playerHP = 0;

        playerHealthBar.UpdateHealth(playerHP); // 체력바 갱신

        DialogText.text = $"적이 {selectedWeapon}(으)로 공격! 플레이어가 {damage} 피해를 입었다. 남은 체력: {playerHP}";

        if (playerHP <= 0 && !battleEnded)
        {
            DialogText.text = "플레이어가 쓰러졌다!"; // 게임 오버 처리
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

        // 페이드 아웃
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }

        // 전투 시스템 종료 → 프리팹 비활성화
        // gameObject.SetActive(false);

        // 씬 전환(만약 플레이어가 지게 되면 플레이어 말하는 화면 나온 뒤 집으로)
        // SceneManager.LoadScene("마을씬");
    }
}