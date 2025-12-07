using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class FightTrigger : MonoBehaviour
{
    [Header("몬스터 정보")]
    public string monsterId = "LibraryBoss";

    [Header("전투 Scene 불러오기")]
    public string fightSceneName = "Fight"; // 로드할 전투 씬 이름

    [Header("보상 설정")]
    public GameObject rewardArtifactPrefab; // 처치 후 생성될 아티팩트 프리팹 (ArtifactProximityPickup 붙어있어야 함)
    public Transform rewardSpawnPoint;      // 생성 위치 (없으면 몬스터 위치)

    [Header("전투 시작 연출")]
    public float bounceScale = 0.7f;
    public float bounceDuration = 0.15f;
    public float suckDuration = 0.6f;

    private bool _isLoading = false;
    private Collider2D triggerCollider;

    void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        if (GameStateManager.Instance != null)
        {
            bool isDefeated = GameStateManager.Instance.defeatedMonsterIds.Contains(monsterId);

            // 만약 이미 처치된 몬스터라면
            if (isDefeated)
            {
                HandleRewardState();
                if (triggerCollider != null) triggerCollider.enabled = false;
                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 로딩 중이거나 플레이어가 아니면 무시
        if (_isLoading || !other.CompareTag("Player")) return;
        _isLoading = true;

        Debug.Log($"'{monsterId}'와 전투 시작");

        // GameStateManager에 전투 정보 기록
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.playerPositionBeforeBattle = PlayerController.Instance.transform.position;
            GameStateManager.Instance.returnSceneAfterBattle = SceneManager.GetActiveScene().name;
            GameStateManager.Instance.currentMonsterId = monsterId; // 현재 싸울 몬스터 ID 저장
            PlayerController.Instance.StopMovement();

            Debug.Log($"전투 정보 저장: 위치({GameStateManager.Instance.playerPositionBeforeBattle}), 복귀씬({GameStateManager.Instance.returnSceneAfterBattle}), 몬스터ID({monsterId})");
        }
        else
        {
            _isLoading = false; // 로딩 취소
            return;
        }

        // 페이드 후 전투 씬으로 이동
        StartCoroutine(EncounterSequence());
    }

    // 전투 시작 연출 코루틴
    private IEnumerator EncounterSequence()
    {
        Camera cam = Camera.main;
        float originalSize = cam.orthographicSize;
        SoundManager.Instance.PlaySFX(SfxType.BattleEncounter);

        // 1-1. 하얗게 세팅
        FadeManager.Instance.SetFadeColor(Color.white);

        // 1-2. "쿵!" (순간 확대) + 하얀색 팍!
        // 병렬로 실행: 카메라는 줄어들고(줌인), 화면은 하얘짐
        FadeManager.Instance.FadeOut(bounceDuration);
        yield return StartCoroutine(ChangeCameraSize(cam, originalSize, originalSize * bounceScale, bounceDuration));

        // 1-3. "탁!" (원상 복구) + 하얀색 빠짐
        FadeManager.Instance.FadeIn(bounceDuration);
        yield return StartCoroutine(ChangeCameraSize(cam, originalSize * bounceScale, originalSize, bounceDuration));

        // 잠깐 대기 (리듬감)
        yield return new WaitForSeconds(0.1f);

        // 2-1. 검은색 세팅
        FadeManager.Instance.SetFadeColor(Color.black);

        FadeManager.Instance.FadeOut(suckDuration);

        yield return StartCoroutine(ChangeCameraSize(cam, originalSize, 0.1f, suckDuration));

        SceneManager.LoadScene(fightSceneName);
    }

    private IEnumerator ChangeCameraSize(Camera cam, float startSize, float endSize, float time)
    {
        float elapsed = 0f;
        while (elapsed < time)
        {
            elapsed += Time.unscaledDeltaTime;
            cam.orthographicSize = Mathf.Lerp(startSize, endSize, elapsed / time);
            yield return null;
        }
        cam.orthographicSize = endSize;
    }

    private IEnumerator ProcessBattleStart()
    {
        yield return FadeManager.Instance.WipeOut(0.5f);
        SceneManager.LoadScene(fightSceneName);
    }

    private void HandleRewardState()
    {
        Debug.Log($"몬스터 '{monsterId}'는 이미 처치됨. 보상 상태 확인.");
        gameObject.SetActive(false);

        // 보상 아이템 획득 여부 확인
        bool rewardCollected = false;
        if (monsterId == "LibraryBoss")
        {
            rewardCollected = GameStateManager.Instance.collectedLibraryBossReward;
        }

        // 아직 보상을 획득하지 않았고, 보상 프리팹이 설정되어 있다면
        if (!rewardCollected && rewardArtifactPrefab != null)
        {
            Debug.Log($"보상({rewardArtifactPrefab.name})을 생성합니다.");
            Vector3 pos = rewardSpawnPoint ? rewardSpawnPoint.position : transform.position;
            Quaternion rot = rewardSpawnPoint ? rewardSpawnPoint.rotation : Quaternion.identity;
            Instantiate(rewardArtifactPrefab, pos, rot); // 생성
        }
        else
        {
            Debug.Log("보상은 이미 획득했거나 설정되지 않았습니다.");
        }
    }
}