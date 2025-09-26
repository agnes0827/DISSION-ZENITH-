using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject battleInventoryMenu; // 배틀 인벤토리 메뉴 인스펙터에 연결
    public WeaponSlotManager weaponSlotManager;
    public Enemy enemy; // 데미지를 받을 대상 (스크립트 연결)

    public int playerHP = 100; // 플레이어 기본 체력

    public GameObject panel; // 자막을 띄울 패널
    public Text DialogText; // 다이얼로그 텍스트

    public Text EnemyHpText; // 적 체력 텍스트
    public Text PlayerHpText; // 플레이어 체력 텍스트

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
        enemy.OnDamaged += HandleEnemyDamaged;
        enemy.OnDied += HandleEnemyDied;

        StartBattle();
    }

    void HandleEnemyDamaged(int currentHP, int damage)
    {
        DialogText.text = $"적이 {damage} 피해를 입었다!";
        EnemyHpText.text = $"{damage} 피해, 남은 체력: {currentHP}";
    }

    void HandleEnemyDied()
    {
        EnemyHpText.enabled = false;
        PlayerHpText.enabled = false;
        DialogText.text = "적이 쓰러졌다!";
        // 마지막으로 싸운 몬스터를 처치 완료로 기록
        if (LibraryGameState.Instance != null)
            LibraryGameState.Instance.MarkDefeated(LibraryGameState.Instance.lastMonsterId);

        // Library로 복귀 (페이드아웃 코루틴 안에서 호출해도 OK)
        EndBattle("Library"); // 앞서 만든 nextScene 인자 받는 버전 사용
    }

    void StartBattle()
    {
        battleInventoryMenu.SetActive(true); // 전투 시작 시 인벤토리 자동 오픈
    }

    public void OnWeaponSlotClicked(int slotIndex)
    {
        WeaponData data = weaponSlotManager.GetWeaponData(slotIndex);
        if (data != null)
        {
            panel.SetActive(true); // 패널 활성화
            StartCoroutine(ShowWeaponUseAndAttack(data));
        }
    }

    // 플레이어가 공격했을 때 호출되는 함수
    public void OnPlayerAttack()
    {
        // 잠깐 대기 후 적의 반격
        StartCoroutine(EnemyCounterAttack());
    }

    IEnumerator ShowWeaponUseAndAttack(WeaponData data)
    {
        int damage = Random.Range(data.minDamage, data.maxDamage + 1); // 공격력 랜덤

        DialogText.text = $"무기 {data.name} 사용!";

        yield return new WaitForSeconds(1.2f);
        enemy.TakeDamage(damage); // 적 체력 감소
        enemy.GetComponent<DamageFlash>().FlashRed(1f); // 적 이미지 빨갛게
        yield return new WaitForSeconds(1.5f);
        OnPlayerAttack(); // 적 반격 시작
    }

    IEnumerator EnemyCounterAttack()
    {
        yield return new WaitForSeconds(1.5f); // 2초 대기 (연출용)

        string selectedWeapon = enemyWeapons[Random.Range(0, enemyWeapons.Length)]; // 무기 둘 중 하나 사용
        int damage = enemyDamageMap[selectedWeapon];

        playerHP -= damage;
        if (playerHP < 0) playerHP = 0;

        DialogText.text = $"적이 {selectedWeapon}(으)로 공격!";
        PlayerHpText.text = $"{damage} 피해, 남은 체력: {playerHP}";

        GameObject.Find("Player").GetComponent<DamageFlash>().FlashRed(1f); // 플레이어 이미지 빨갛게

        if (playerHP <= 0 && !battleEnded)
        {
            EnemyHpText.enabled = false;
            PlayerHpText.enabled = false;
            DialogText.text = "플레이어가 쓰러졌다!"; // 게임 오버 처리
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

        // 페이드 아웃
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }

        // 전투 시스템 종료 → 프리팹 비활성화
        gameObject.SetActive(false);

        // 씬 전환
        // 지정된 씬으로 이동
        SceneManager.LoadScene(nextScene);
    }
}