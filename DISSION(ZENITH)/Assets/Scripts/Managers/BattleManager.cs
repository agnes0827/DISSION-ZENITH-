using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BattleManager : MonoBehaviour
{
    [Header("UI 연결")]
    // [SerializeField] private GameObject battleInventoryMenu; // 인벤토리 메뉴 패널 (전투 시작 시 활성화)
    public WeaponSlotManager weaponSlotManager;              // 플레이어 무기 슬롯 관리 스크립트

    // 전투 정보 표시 UI
    public TextMeshProUGUI DialogText;      // 다이얼로그 (전투 상황 메시지) 출력 텍스트
    public TextMeshProUGUI PlayerHpText;    // 플레이어 현재 체력 표시 텍스트
    public GameObject panel;                // 자막을 띄울 UI 패널

    // 체력바 슬라이더
    public Slider playerHpSlider;
    public Slider enemyHpSlider;

    // 턴 indicator
    public TurnIndicator turnIndicator;

    public Vector3 playerIndicatorOffset = new Vector3(0, 100f, 0); // 플레이어
    public Vector3 enemyIndicatorOffset = new Vector3(0, 200f, 0);  // enemy

    [Header("Dialogue 설정")]
    public float typingSpeed = 0.05f;


    [Header("전투 데이터 및 상태 관리")]
    // 캐릭터 정보
    public Enemy enemy;                     // 현재 전투 중인 적 스크립트 (데미지 전달 대상)

    // 전투 상태
    private bool battleEnded = false;       // 전투가 종료되었는지 여부
    private bool isActionInProgress = false; // 현재 플레이어 또는 적의 행동이 진행 중인지 (중복 입력 방지)

    // 씬 및 몬스터 정보 (GameStateManager에서 초기화됨)
    private string currentMonsterId;        // 현재 싸우는 몬스터의 고유 ID
    private string sceneToReturn;           // 전투 종료 후 돌아갈 이전 씬 이름

    // 적의 공격 패턴
    private string[] enemyWeapons = { "손", "칼" }; // 적이 사용할 수 있는 무기 목록
    private Dictionary<string, int> enemyDamageMap = new Dictionary<string, int>()
    {
        {"손", 10 },
        {"칼", 15 }
    }; // 적 무기 이름별 기본 공격력

    // 무기 리스트
    public List<WeaponData> allWeaponDatabase;
    public WeaponData defaultHandWeapon; // 기본 '손' 데이터 (인스펙터에서 설정)


    [Header("전투 모션 설정")]
    public Transform playerVisual;          // 플레이어 캐릭터의 Transform
    public Transform enemyVisual;           // 적 캐릭터의 Transform

    public Vector3 playerOriginalPos;       // 플레이어의 전투 시작 초기 위치
    public Vector3 enemyOriginalPos;        // 적의 전투 시작 초기 위치

    public float attackMoveDistance = 0.5f; // 공격 시 앞으로 전진하는 거리
    public float hitKnockbackDistance = 0.3f; // 피격 시 뒤로 밀려나는 거리
    public float moveDuration = 0.15f;      // 지속 시간

    void Start()
    {
        // 위치 초기화
        if (playerVisual != null) playerOriginalPos = playerVisual.position;
        if (enemyVisual != null) enemyOriginalPos = enemyVisual.position;

        // GameStateManager 데이터 로드
        if (GameStateManager.Instance != null)
        {
            currentMonsterId = GameStateManager.Instance.currentMonsterId;
            sceneToReturn = GameStateManager.Instance.returnSceneAfterBattle;
            Debug.Log($"BattleManager 시작: 몬스터ID({currentMonsterId}), 복귀씬({sceneToReturn})");

            if (enemy == null) enemy = FindObjectOfType<Enemy>();
            if (enemy == null) { Debug.LogError("BattleManager: Enemy를 찾거나 설정할 수 없습니다!"); return; }

            // 플레이어 HP 초기화
            if (playerHpSlider != null)
            {
                playerHpSlider.maxValue = GameStateManager.Instance.playerMaxHP;
                playerHpSlider.value = GameStateManager.Instance.playerHP;
            }
            UpdatePlayerHPUI();
        }
        else
        {
            Debug.LogError("BattleManager: GameStateManager를 찾을 수 없습니다!"); return;
        }

        // 적 이벤트 연결 및 HP 초기화
        if (enemy != null)
        {
            enemy.OnDamaged += HandleEnemyDamaged;
            enemy.OnDied += HandleEnemyDied;

            if (enemyHpSlider != null)
            {
                enemyHpSlider.maxValue = enemy.hp;
                enemyHpSlider.value = enemy.hp;
            }
        }
        LoadPlayerWeapons(); // 무기 장착

        // 시작 전 indicator 비활성화
        if (turnIndicator != null) turnIndicator.SetTarget(null, Vector3.zero);

        StartCoroutine(StartBattleSequence());
    }

    void LoadPlayerWeapons()
    {
        if (GameStateManager.Instance == null) return;

        List<WeaponData> myWeapons = new List<WeaponData>();

        // 1. 인벤토리(문자열 ID)를 실제 무기 데이터(WeaponData)로 변환
        foreach (var item in GameStateManager.Instance.inventoryItems)
        {
            string itemId = item.Key;

            // 도감에서 이름이 같은 무기를 찾음
            WeaponData foundWeapon = allWeaponDatabase.Find(w => w.name == itemId);

            if (foundWeapon != null)
            {
                myWeapons.Add(foundWeapon);
            }
        }

        // 2. 무기가 없으면 '손' 추가
        if (myWeapons.Count == 0 && defaultHandWeapon != null)
        {
            myWeapons.Add(defaultHandWeapon);
        }

        // 3. WeaponSlotManager에게 전달
        weaponSlotManager.SetupWeaponSlots(myWeapons);

    }
    void UpdatePlayerHPUI()
    {
        // 플레이어 텍스트 & 슬라이더 갱신
        if (GameStateManager.Instance != null)
        {
            if (PlayerHpText != null)
                PlayerHpText.text = $"{GameStateManager.Instance.playerHP}";

            if (playerHpSlider != null)
                playerHpSlider.value = GameStateManager.Instance.playerHP;
        }
    }

    void HandleEnemyDamaged(int currentHP, int damage)
    {
        if (enemyHpSlider != null)
        {
            enemyHpSlider.value = currentHP;
        }
    }

    void HandleEnemyDied()
    {
        SoundManager.Instance.PlaySFX(SfxType.EnemyDeath, 0.5f, false);
        if (battleEnded) return;
        battleEnded = true;

        if (enemyHpSlider != null)
        {
            enemyHpSlider.gameObject.SetActive(false);
        }
    }

    // 전투 시작 연출
    IEnumerator StartBattleSequence()
    {
        panel.SetActive(true);
        isActionInProgress = true;

        // 몬스터 이름 가져오기
        string monsterName = (enemy != null) ? enemy.enemyName : "몬스터";
        yield return StartCoroutine(TypeWriterEffect($"{monsterName}이(가) 공격해왔다!"));
        yield return new WaitForSeconds(1.5f);

        // indicator: 플레이어 턴
        if (turnIndicator != null)
            turnIndicator.SetTarget(playerVisual, playerIndicatorOffset);

        isActionInProgress = false;
    }

    // 타이핑 효과 함수
    IEnumerator TypeWriterEffect(string message)
    {
        DialogText.text = ""; // 초기화
        foreach (char letter in message.ToCharArray())
        {
            DialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    // 무기 선택
    public void OnWeaponSlotClicked(int slotIndex)
    {
        if (isActionInProgress || battleEnded) return;

        WeaponData data = weaponSlotManager.GetWeaponData(slotIndex);
        if (data != null)
        {
            isActionInProgress = true;
            StartCoroutine(ShowWeaponUseAndAttack(data));
        }
    }

    // 플레이어 공격 코루틴
    IEnumerator ShowWeaponUseAndAttack(WeaponData data)
    {
        int damage = Random.Range(data.minDamage, data.maxDamage + 1); // 공격력 랜덤

        string printName = string.IsNullOrEmpty(data.name) ? data.name : data.name;

        // 1. 공격 선언 (타이핑)
        yield return StartCoroutine(TypeWriterEffect($"{data.displayName}(으)로 공격!"));
        yield return new WaitForSeconds(0.2f); // 텍스트 읽을 시간

        // 2. 플레이어 모션 실행
        if (playerVisual != null)
            yield return StartCoroutine(AttackMotion(playerVisual, playerOriginalPos, true));
        else
            yield return new WaitForSeconds(0.5f);

        if (enemy == null) { yield break; }

        // 3. 데미지 적용 및 적 피격 모션
        enemy.TakeDamage(damage);
        enemy.GetComponent<DamageFlash>()?.FlashRed(0.4f);

        if (enemyVisual != null)
            StartCoroutine(HitMotion(enemyVisual, enemyOriginalPos, false));

        // 4. 결과 텍스트 출력 (타이핑)
        yield return StartCoroutine(TypeWriterEffect($"적에게 {damage}의 데미지를 입혔다."));
        yield return new WaitForSeconds(1.0f); // 결과 확인할 시간

        if (battleEnded) // 적이 죽어서 HandleEnemyDied가 실행된 상태라면
        {
            StartCoroutine(VictorySequence()); // 이제 승리 연출 시작!
        }
        else if (enemy != null) // 적이 살아있다면
        {
            StartCoroutine(EnemyCounterAttack()); // 적 반격 시작
        }
    }
    
    // 적 반격 코루틴
    IEnumerator EnemyCounterAttack()
    {
        if (enemy == null || battleEnded || GameStateManager.Instance.playerHP <= 0)
        {
            isActionInProgress = false;
            yield break;
        }

        // indicator: enemy 턴
        if (turnIndicator != null)
            turnIndicator.SetTarget(enemyVisual, enemyIndicatorOffset);

        string selectedWeapon = enemyWeapons[Random.Range(0, enemyWeapons.Length)];
        int damage = enemyDamageMap[selectedWeapon];

        // 1. 적 공격 선언 (타이핑)
        yield return StartCoroutine(TypeWriterEffect($"적의 공격!"));
        yield return new WaitForSeconds(0.2f);

        // 2. 적 공격 모션
        if (enemyVisual != null)
            yield return StartCoroutine(AttackMotion(enemyVisual, enemyOriginalPos, false));
        else
            yield return new WaitForSeconds(0.5f);

        if (enemy == null || battleEnded) yield break;

        // 3. 플레이어 데미지 및 피격 모션
        GameStateManager.Instance.ChangeHP(-damage);
        UpdatePlayerHPUI();
        playerVisual.GetComponent<DamageFlash>().FlashRed(0.4f);

        if (playerVisual != null)
            StartCoroutine(HitMotion(playerVisual, playerOriginalPos, true));
       
        // 4. 결과 텍스트 출력 (타이핑)
        yield return StartCoroutine(TypeWriterEffect($"{damage}의 데미지를 입었다."));
        yield return new WaitForSeconds(1.0f);

        // 패배
        if (GameStateManager.Instance.playerHP <= 0 && !battleEnded)
        {
            battleEnded = true;
            if (PlayerHpText != null) PlayerHpText.enabled = false;

            // indicator 비활성화
            if (turnIndicator != null) turnIndicator.SetTarget(null, Vector3.zero);

            yield return StartCoroutine(TypeWriterEffect("눈앞이 깜깜해진다..."));
            yield return new WaitForSeconds(2.0f);
            EndBattle("DialogueTest");
        }
        else if (!battleEnded) // enemy가 살아있다면
        {
            isActionInProgress = false;

            // indicator: 플레이어 턴
            if (turnIndicator != null)
                turnIndicator.SetTarget(playerVisual, playerIndicatorOffset);
        }
    }

    IEnumerator VictorySequence()
    {
        // indicator 비활성화
        if (turnIndicator != null) turnIndicator.SetTarget(null, Vector3.zero);

        yield return StartCoroutine(TypeWriterEffect("적을 쓰러트렸다!"));
        yield return new WaitForSeconds(1.5f);

        if (GameStateManager.Instance != null && !string.IsNullOrEmpty(currentMonsterId))
        {
            GameStateManager.Instance.defeatedMonsterIds.Add(currentMonsterId);
        }
        EndBattle(sceneToReturn);
    }

    IEnumerator AttackMotion(Transform attacker, Vector3 originalPos, bool isPlayer)
    {
        float direction = isPlayer ? -1f : 1f;
        Vector3 targetPos = originalPos + new Vector3(attackMoveDistance * direction, 0, 0);

        // 1. 앞으로 이동
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            attacker.position = Vector3.Lerp(originalPos, targetPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        attacker.position = targetPos;

        // 2. 잠시 대기 (타격 시점)
        yield return new WaitForSeconds(0.1f);

        // 3. 원래 위치로 복귀
        elapsed = 0f;
        while (elapsed < moveDuration)
        {
            attacker.position = Vector3.Lerp(targetPos, originalPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        attacker.position = originalPos;
    }

    // 피격 모션 코루틴
    IEnumerator HitMotion(Transform target, Vector3 originalPos, bool isPlayer)
    {
        float direction = isPlayer ? 1f : -1f;
        Vector3 knockbackPos = originalPos + new Vector3(hitKnockbackDistance * direction, 0, 0);

        // 1. 뒤로 밀려남
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            target.position = Vector3.Lerp(originalPos, knockbackPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.position = knockbackPos;

        // 2. 원래 위치로 복귀
        elapsed = 0f;
        while (elapsed < moveDuration)
        {
            target.position = Vector3.Lerp(knockbackPos, originalPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.position = originalPos;
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

        StartCoroutine(ProcessBattleEnd(nextScene));
    }

    IEnumerator ProcessBattleEnd(string nextScene)
    {
        // 1. FadeManager에게 페이드 아웃 요청 (1.5초 동안)
        // yield return을 사용하여 페이드가 끝날 때까지 여기서 대기합니다.
        yield return FadeManager.Instance.FadeOut(1.5f);

        // 2. 페이드 아웃이 다 끝나면 씬 로드
        if (!string.IsNullOrEmpty(nextScene))
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.LogError("이동할 씬 이름이 없습니다!");
        }

        // 3. 씬이 로드되면 FadeManager의 OnSceneLoaded가 자동으로 실행되어
        //    새로운 씬에서 FadeIn(화면 밝아짐)이 수행됩니다.
    }
}