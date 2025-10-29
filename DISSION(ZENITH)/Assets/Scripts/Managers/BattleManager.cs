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

    private string currentMonsterId;
    private string sceneToReturn;

    private bool battleEnded = false;

    private string[] enemyWeapons = { "손", "칼" }; // 에너미가 사용하는 무기 나열
    private Dictionary<string, int> enemyDamageMap = new Dictionary<string, int>()
    {
        {"손", 10 },
        {"칼", 15 }
    }; // 적 무기 별 공격력 연결

    void Start()
    {
        if (GameStateManager.Instance != null)
        {
            currentMonsterId = GameStateManager.Instance.currentMonsterId;
            sceneToReturn = GameStateManager.Instance.returnSceneAfterBattle;
            Debug.Log($"BattleManager 시작: 몬스터ID({currentMonsterId}), 복귀씬({sceneToReturn})");

            if (enemy == null) enemy = FindObjectOfType<Enemy>();
            if (enemy == null) { Debug.LogError("BattleManager: Enemy를 찾거나 설정할 수 없습니다!"); return; }

            // 플레이어 HP UI 초기화
            UpdatePlayerHPUI();
        }
        else
        {
            Debug.LogError("BattleManager: GameStateManager를 찾을 수 없습니다!"); return;
        }

        if (enemy != null)
        {
            enemy.OnDamaged += HandleEnemyDamaged;
            enemy.OnDied += HandleEnemyDied; // OnDied 이벤트 구독
            EnemyHpText.text = $"남은 체력: {enemy.hp}"; // 적 HP 초기 표시
        }
        StartBattle();
    }

    void UpdatePlayerHPUI()
    {
        if (PlayerHpText != null && GameStateManager.Instance != null)
        {
            PlayerHpText.text = $"남은 체력: {GameStateManager.Instance.playerHP}";
        }
    }

    void HandleEnemyDamaged(int currentHP, int damage)
    {
        DialogText.text = $"적이 {damage} 피해를 입었다!";
        EnemyHpText.text = $"{damage} 피해, 남은 체력: {currentHP}";
    }

    void HandleEnemyDied()
    {
        if (battleEnded) return;
        battleEnded = true;

        EnemyHpText.enabled = false;
        PlayerHpText.enabled = false;
        DialogText.text = "적이 쓰러졌다!";

        if (GameStateManager.Instance != null && !string.IsNullOrEmpty(currentMonsterId))
        {
            GameStateManager.Instance.defeatedMonsterIds.Add(currentMonsterId);
            Debug.Log($"GameStateManager 업데이트: 몬스터 '{currentMonsterId}' 처치 기록됨.");
        }

        EndBattle(sceneToReturn);
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

        if (enemy == null || battleEnded) // 이미 전투 끝났거나 적 없으면 공격 금지
        {
            yield break; // 코루틴 즉시 종료
        }

        enemy.TakeDamage(damage); // 적 체력 감소

        if (enemy == null || battleEnded)
        {
            Debug.Log("ShowWeaponUseAndAttack: 이번 공격으로 적이 쓰러져 추가 처리 중단.");
            yield break; // 코루틴 즉시 종료
        }

        enemy.GetComponent<DamageFlash>()?.FlashRed(1f); // ?. 연산자로 null 체크 추가
        yield return new WaitForSeconds(1.5f);

        if (enemy != null && !battleEnded)
        {
            OnPlayerAttack(); // 적 반격
        }
    }

    IEnumerator EnemyCounterAttack()
    {
        if (enemy == null || battleEnded || GameStateManager.Instance == null || GameStateManager.Instance.playerHP <= 0)
        {
            Debug.Log("EnemyCounterAttack: 반격 조건 미충족으로 중단.");
            yield break; // 조건 안 맞으면 반격 금지
        }

        yield return new WaitForSeconds(1.5f);

        if (enemy == null || battleEnded || GameStateManager.Instance.playerHP <= 0)
        {
            Debug.Log("EnemyCounterAttack: 반격 직전 조건 미충족으로 중단.");
            yield break;
        }


        string selectedWeapon = enemyWeapons[Random.Range(0, enemyWeapons.Length)]; // 무기 둘 중 하나 사용
        int damage = enemyDamageMap[selectedWeapon];

        GameStateManager.Instance.ChangeHP(-damage);
        UpdatePlayerHPUI();

        DialogText.text = $"적이 {selectedWeapon}(으)로 공격!";
        PlayerController.Instance?.GetComponent<DamageFlash>()?.FlashRed(1f);


        if (GameStateManager.Instance != null && GameStateManager.Instance.playerHP <= 0 && !battleEnded)
        {
            EnemyHpText.enabled = false;
            PlayerHpText.enabled = false;
            DialogText.text = "플레이어가 쓰러졌다!";
            EndBattle("DialogueTest");
        }
    }

    void EndBattle(string nextScene)
    {
        Debug.Log($"EndBattle 호출됨. 다음 씬: {nextScene}");

        // 적 이벤트 구독 해제 (메모리 누수 방지)
        if (enemy != null)
        {
            enemy.OnDamaged -= HandleEnemyDamaged;
            enemy.OnDied -= HandleEnemyDied;
        }
        else
        {
            Debug.Log("Enemy 참조가 null이므로 이벤트 해제 건너<0xEB><0><0x84>. (이미 파괴된 것으로 예상)");
        }


        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            StartCoroutine(FadeOutAndClose(nextScene));
        }
        else
        {
            Debug.LogWarning("Fade Panel이 연결되지 않아 페이드 효과 없이 즉시 씬 전환 시도.");
            if (!string.IsNullOrEmpty(nextScene))
            {
                SceneManager.LoadScene(nextScene);
            }
            else
            {
                Debug.LogError("BattleManager: 돌아갈 씬 이름이 없습니다!");
            }
        }
    }

    IEnumerator FadeOutAndClose(string nextScene)
    {
        Debug.Log($"FadeOutAndClose 시작. TimeScale: {Time.timeScale}");

        float elapsed = 0f;
        Color c = fadePanel.color;

        // 페이드 아웃
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }

        Debug.Log("페이드 아웃 완료. 씬 로드 시도...");

        if (!string.IsNullOrEmpty(nextScene))
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.LogError("BattleManager: 돌아갈 씬 이름이 없습니다!");
            // 기본 씬으로 이동하거나 에러 처리
            // SceneManager.LoadScene("DefaultMap");
        }

    }
}