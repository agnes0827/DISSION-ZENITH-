using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BattleManager : MonoBehaviour
{
    [Header("UI 요소")]
    public WeaponSlotManager weaponSlotManager;              // 플레이어 무기 슬롯 관리 스크립트
    public TextMeshProUGUI DialogText;      // 다이얼로그 (전투 상황 메시지) 용 텍스트
    public TextMeshProUGUI PlayerHpText;    // 플레이어 현재 체력 표시 텍스트
    public GameObject panel;                // 전체적인 UI 패널
    public Slider playerHpSlider;
    public Slider enemyHpSlider;
    public TurnIndicator turnIndicator;
    public Vector3 playerIndicatorOffset = new Vector3(0, 100f, 0);
    public Vector3 enemyIndicatorOffset = new Vector3(0, 200f, 0);

    [Header("Dialogue 관련")]
    public float typingSpeed = 0.05f;

    [Header("전투 참여자 및 씬 정보")]
    public Enemy enemy;
    private bool battleEnded = false;
    private bool isActionInProgress = false;
    private string currentMonsterId;
    private string sceneToReturn;

    // 추가: 공격 횟수 카운트 + 펫 도움 발동 여부
    private int playerAttackCount = 0;
    private int enemyAttackCount = 0;
    private bool petAssistUsed = false;

    private string[] enemyWeapons = { "주먹", "칼" };
    private Dictionary<string, int> enemyDamageMap = new Dictionary<string, int>()
    {
        {"주먹", 10 },
        {"칼", 15 }
    };

    public List<WeaponData> allWeaponDatabase;
    public WeaponData defaultHandWeapon;

    [Header("전투 모션 관련")]
    public Transform playerVisual;
    public Transform enemyVisual;
    public Vector3 playerOriginalPos;
    public Vector3 enemyOriginalPos;
    public float attackMoveDistance = 0.5f;
    public float hitKnockbackDistance = 0.3f;
    public float moveDuration = 0.15f;

    [Header("이펙트 프리팹")]
    public GameObject fireballPrefab;
    public Transform firePoint;

    void Start()
    {
        if (playerVisual != null) playerOriginalPos = playerVisual.position;
        if (enemyVisual != null) enemyOriginalPos = enemyVisual.position;

        if (GameStateManager.Instance != null)
        {
            currentMonsterId = GameStateManager.Instance.currentMonsterId;
            sceneToReturn = GameStateManager.Instance.returnSceneAfterBattle;
            if (enemy == null) enemy = FindObjectOfType<Enemy>();
            if (enemy == null) { Debug.LogError("BattleManager: Enemy를 찾거나 생성할 수 없습니다!"); return; }

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
        LoadPlayerWeapons();

        if (turnIndicator != null) turnIndicator.SetTarget(null, Vector3.zero);
        StartCoroutine(StartBattleSequence());
    }

    void LoadPlayerWeapons()
    {
        if (GameStateManager.Instance == null) return;
        List<WeaponData> myWeapons = new List<WeaponData>();
        foreach (var item in GameStateManager.Instance.inventoryItems)
        {
            WeaponData foundWeapon = allWeaponDatabase.Find(w => w.name == item.Key);
            if (foundWeapon != null)
            {
                myWeapons.Add(foundWeapon);
            }
        }
        if (myWeapons.Count == 0 && defaultHandWeapon != null)
        {
            myWeapons.Add(defaultHandWeapon);
        }
        weaponSlotManager.SetupWeaponSlots(myWeapons);
    }

    void UpdatePlayerHPUI()
    {
        if (GameStateManager.Instance != null)
        {
            if (PlayerHpText != null) PlayerHpText.text = $"{GameStateManager.Instance.playerHP}";
            if (playerHpSlider != null) playerHpSlider.value = GameStateManager.Instance.playerHP;
        }
    }

    void HandleEnemyDamaged(int currentHP, int damage)
    {
        if (enemyHpSlider != null) enemyHpSlider.value = currentHP;
    }

    void HandleEnemyDied()
    {
        if (battleEnded) return;
        battleEnded = true;
        if (enemyHpSlider != null) enemyHpSlider.gameObject.SetActive(false);
    }

    IEnumerator StartBattleSequence()
    {
        panel.SetActive(true);
        isActionInProgress = true;
        string monsterName = (enemy != null) ? enemy.enemyName : "적";
        yield return StartCoroutine(TypeWriterEffect($"{monsterName}이(가) 나타났다!"));
        yield return new WaitForSeconds(1.5f);
        if (turnIndicator != null) turnIndicator.SetTarget(playerVisual, playerIndicatorOffset);
        isActionInProgress = false;
    }

    IEnumerator TypeWriterEffect(string message)
    {
        DialogText.text = "";
        foreach (char letter in message.ToCharArray())
        {
            DialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void OnWeaponSlotClicked(int slotIndex)
    {
        if (isActionInProgress || battleEnded) return;

        WeaponData data = weaponSlotManager.GetWeaponData(slotIndex);
        if (data == null) return;

        // 쿨타임 확인
        if (Time.time - data.lastUseTime < data.cooldown)
        {
            Debug.Log($"{data.displayName}은(는) 아직 쿨타임 중입니다.");
            // 여기에 쿨타임 중일 때의 사운드나 UI 피드백을 추가할 수 있습니다.
            return;
        }

        isActionInProgress = true;
        StartCoroutine(ShowWeaponUseAndAttack(data, slotIndex));
    }

    IEnumerator ShowWeaponUseAndAttack(WeaponData data, int slotIndex)
    {
        // 쿨타임 적용
        data.lastUseTime = Time.time;
        if (data.cooldown > 0)
        {
            weaponSlotManager.StartCooldown(slotIndex, data.cooldown);
        }

        int damage = Random.Range(data.minDamage, data.maxDamage + 1);
        yield return StartCoroutine(TypeWriterEffect($"{data.displayName}(으)로 공격!"));
        yield return new WaitForSeconds(0.2f);

        (SfxType soundToPlay, float volume) = GetWeaponSound(data.name);
        SoundManager.Instance.PlaySFX(soundToPlay, volume);

        if (data.name == "Fireball_Book" || data.name == "fireball")
        {
            yield return StartCoroutine(FireballAttackSequence(damage));
        }
        else
        {
            if (playerVisual != null) yield return StartCoroutine(AttackMotion(playerVisual, playerOriginalPos, true));
            else yield return new WaitForSeconds(0.5f);
            ApplyDamageToEnemy(damage);
        }

        yield return StartCoroutine(TypeWriterEffect($"적에게 {damage}의 피해를 입혔다."));
        yield return new WaitForSeconds(1.0f);

        // 플레이어 공격 카운트 증가
        playerAttackCount++;

        if (battleEnded)
        {
            StartCoroutine(VictorySequence());
        }
        else if (enemy != null)
        {
            StartCoroutine(EnemyCounterAttack());
        }
    }

    IEnumerator FireballAttackSequence(int damage)
    {
        Vector3 startPos = (firePoint != null) ? firePoint.position : playerVisual.position;
        if (fireballPrefab != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, startPos, fireballPrefab.transform.rotation);
            if (playerVisual != null) fireball.transform.SetParent(playerVisual.parent, true);

            FireballMove mover = fireball.GetComponent<FireballMove>();
            if (mover != null)
            {
                bool hitFinished = false;
                mover.Setup(enemyVisual.position, () => {
                    ApplyDamageToEnemy(damage);
                    hitFinished = true;
                });
                while (!hitFinished)
                {
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                ApplyDamageToEnemy(damage);
            }
        }
        else
        {
            Debug.LogError("파이어볼 프리팹이 설정되지 않았습니다!");
            ApplyDamageToEnemy(damage);
        }
    }

    void ApplyDamageToEnemy(int damage)
    {
        if (enemy == null) return;
        enemy.TakeDamage(damage);
        enemy.GetComponent<DamageFlash>()?.FlashRed(0.4f);
        if (enemyVisual != null) StartCoroutine(HitMotion(enemyVisual, enemyOriginalPos, false));
    }

    IEnumerator EnemyCounterAttack()
    {
        if (enemy == null || battleEnded || GameStateManager.Instance.playerHP <= 0)
        {
            isActionInProgress = false;
            yield break;
        }

        // 적 공격 카운트 증가
        enemyAttackCount++;

        if (turnIndicator != null) turnIndicator.SetTarget(enemyVisual, enemyIndicatorOffset);
        string selectedWeapon = enemyWeapons[Random.Range(0, enemyWeapons.Length)];
        int damage = enemyDamageMap[selectedWeapon];

        yield return StartCoroutine(TypeWriterEffect($"적의 반격!"));
        yield return new WaitForSeconds(0.2f);

        SoundManager.Instance.PlaySFX(SfxType.Attack3, 0.4f);
        if (enemyVisual != null) yield return StartCoroutine(AttackMotion(enemyVisual, enemyOriginalPos, false));
        else yield return new WaitForSeconds(0.5f);

        if (enemy == null || battleEnded) yield break;

        SoundManager.Instance.PlaySFX(SfxType.Hit, 0.4f);
        GameStateManager.Instance.ChangeHP(-damage);
        UpdatePlayerHPUI();
        playerVisual.GetComponent<DamageFlash>().FlashRed(0.4f);
        if (playerVisual != null) StartCoroutine(HitMotion(playerVisual, playerOriginalPos, true));

        yield return StartCoroutine(TypeWriterEffect($"{damage}의 피해를 입었다."));
        yield return new WaitForSeconds(1.0f);

        // 여기서 펫 도움 개입 여부 체크
        yield return StartCoroutine(CheckPetAssist());

        // 펫이 물어서 바로 승리해버렸을 수도 있으니 한 번 더 확인
        if (battleEnded)
        {
            isActionInProgress = false;
            yield break;
        }

        if (GameStateManager.Instance.playerHP <= 0 && !battleEnded)
        {
            battleEnded = true;
            if (PlayerHpText != null) PlayerHpText.enabled = false;
            if (turnIndicator != null) turnIndicator.SetTarget(null, Vector3.zero);
            yield return StartCoroutine(TypeWriterEffect("패배했다..."));
            yield return new WaitForSeconds(2.0f);
            EndBattle("DialogueTest");
        }
        else if (!battleEnded)
        {
            isActionInProgress = false;
            if (turnIndicator != null) turnIndicator.SetTarget(playerVisual, playerIndicatorOffset);
        }
    }

    IEnumerator CheckPetAssist()
    {
        // 이미 한 번 발동했으면 다시 안 함
        if (petAssistUsed) yield break;

        // 서로 3번씩 공격한 이후에만
        if (playerAttackCount < 3 || enemyAttackCount < 3) yield break;

        if (battleEnded || enemy == null || GameStateManager.Instance == null)
            yield break;

        float playerHP = GameStateManager.Instance.playerHP;
        int enemyHP = enemy.hp;

        // 적 HP가 내 HP보다 많을 때만 발동
        if (enemyHP <= playerHP) yield break;

        // 여기서 펫 개입 발동
        petAssistUsed = true;

        // 펫이 도와주는 연출
        yield return StartCoroutine(TypeWriterEffect("펫이 먼지괴물을 물었다!"));
        yield return new WaitForSeconds(0.3f);

        int extraDamage = 15;

        // 적에게 추가 피해
        enemy.TakeDamage(extraDamage);
        enemy.GetComponent<DamageFlash>()?.FlashRed(0.4f);
        if (enemyVisual != null)
            yield return StartCoroutine(HitMotion(enemyVisual, enemyOriginalPos, false));

        // 혹시 이 한 방에 적이 죽었으면 바로 승리 처리
        if (enemy.hp <= 0 && !battleEnded)
        {
            battleEnded = true;
            StartCoroutine(VictorySequence());
        }
    }

    IEnumerator VictorySequence()
    {
        if (turnIndicator != null) turnIndicator.SetTarget(null, Vector3.zero);
        yield return StartCoroutine(TypeWriterEffect("적을 물리쳤다!"));
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
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            attacker.position = Vector3.Lerp(originalPos, targetPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        attacker.position = targetPos;
        yield return new WaitForSeconds(0.1f);
        elapsed = 0f;
        while (elapsed < moveDuration)
        {
            attacker.position = Vector3.Lerp(targetPos, originalPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        attacker.position = originalPos;
    }

    IEnumerator HitMotion(Transform target, Vector3 originalPos, bool isPlayer)
    {
        float direction = isPlayer ? 1f : -1f;
        Vector3 knockbackPos = originalPos + new Vector3(hitKnockbackDistance * direction, 0, 0);
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            target.position = Vector3.Lerp(originalPos, knockbackPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.position = knockbackPos;
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
        if (enemy != null)
        {
            enemy.OnDamaged -= HandleEnemyDamaged;
            enemy.OnDied -= HandleEnemyDied;
        }
        StartCoroutine(ProcessBattleEnd(nextScene));
    }

    IEnumerator ProcessBattleEnd(string nextScene)
    {
        yield return FadeManager.Instance.FadeOut(1.5f);
        if (!string.IsNullOrEmpty(nextScene))
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.LogError("이동할 씬 이름이 없습니다!");
        }
    }

    (SfxType, float) GetWeaponSound(string weaponName)
    {
        switch (weaponName)
        {
            case "axe": return (SfxType.Attack4, 1.0f);
            case "Fireball_Book": return (SfxType.AttackFire, 1.0f);
            default: return (SfxType.Attack1, 0.7f);
        }
    }
}
