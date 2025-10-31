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

    [Header("전투 설정")]
    public string fightSceneName = "Fight"; // 로드할 전투 씬 이름
    public Image fadePanel;                 // 페이드 효과에 사용할 이미지
    public float fadeDuration = 1f;

    [Header("보상 설정")]
    public GameObject rewardArtifactPrefab; // 처치 후 생성될 아티팩트 프리팹 (ArtifactProximityPickup 붙어있어야 함)
    public Transform rewardSpawnPoint;      // 생성 위치 (없으면 몬스터 위치)

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
            else // 아직 처치 안 됨
            {
                Debug.Log($"몬스터 '{monsterId}'가 전투 대기 중입니다.");
            }
        }
        else
        {
            Debug.LogError("FightTrigger: GameStateManager를 찾을 수 없습니다!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 로딩 중이거나 플레이어가 아니면 무시
        if (_isLoading || !other.CompareTag("Player")) return;
        _isLoading = true;

        // 이미 전투 진입 로직이 시작되었다면 중복 실행 방지
        _isLoading = true;

        Debug.Log($"플레이어가 '{monsterId}'와 전투를 시작합니다.");

        // GameStateManager에 전투 정보 기록
        if (PlayerController.Instance != null && GameStateManager.Instance != null)
        {
            GameStateManager.Instance.playerPositionBeforeBattle = PlayerController.Instance.transform.position;
            GameStateManager.Instance.returnSceneAfterBattle = SceneManager.GetActiveScene().name;
            GameStateManager.Instance.currentMonsterId = monsterId; // 현재 싸울 몬스터 ID 저장
            Debug.Log($"전투 정보 저장: 위치({GameStateManager.Instance.playerPositionBeforeBattle}), 복귀씬({GameStateManager.Instance.returnSceneAfterBattle}), 몬스터ID({monsterId})");

            PlayerController.Instance.StopMovement();
        }
        else
        {
            Debug.LogError("FightTrigger: PlayerController 또는 GameStateManager를 찾을 수 없어 정보 저장을 못했습니다!");
            _isLoading = false; // 로딩 취소
            return;
        }

        // 페이드 후 전투 씬으로 이동
        if (fadePanel != null)
            StartCoroutine(FadeAndLoad());
        else
            SceneManager.LoadScene(fightSceneName);
    }

    private IEnumerator FadeAndLoad()
    {
        float t = 0f;
        Color c = fadePanel.color;

        if (fadePanel.gameObject != null) fadePanel.gameObject.SetActive(true);

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }

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