using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // ToList 쓰려면 필요
using UnityEngine.SceneManagement;

/// <summary>
/// 게임의 모든 영구 데이터를 저장하고 관리하는 중앙 데이터베이스입니다.
/// 씬이 변경되어도 파괴되지 않으며, 인벤토리, 퀘스트 진행도, 오브젝트 상태 등
/// 게임이 반드시 기억해야 할 모든 정보의 '원본'을 소유합니다.
/// 다른 매니저들은 이 클래스를 통해 데이터에 접근해야 합니다.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    // HP 변경 이벤트: (current, max)
    public event Action<float, float> OnPlayerHpChanged;

    // 스폰포인트 설정
    [Header("Scene Management")]
    public string nextSpawnPointId;

    // 플레이어 상태
    [Header("Player Stats")]
    public float playerHP;// 현재 체력
    public float playerMaxHP = 100f;  // 최대 체력

    // 인벤토리
    [Header("Inventory")]
    public Dictionary<string, int> inventoryItems = new Dictionary<string, int>();
    public int playerGold;

    // 씬 오브젝트 상태
    [Header("Scene Object States")]
    public HashSet<string> collectedSceneObjectIDs = new HashSet<string>();

    // 퀘스트
    [Header("Quest Status")]
    public HashSet<string> acceptedQuests = new HashSet<string>();           // 진행 중인 퀘스트 목록
    public HashSet<string> completedQuests = new HashSet<string>();          // 완료한 퀘스트 목록
    public HashSet<string> objectiveReachedQuests = new HashSet<string>();   // 목표 달성한 퀘스트 목록

    // 아티팩트
    [Header("Artifact Status")]
    public List<string> collectedArtifactIDs = new List<string>();

    // 전투
    [HideInInspector] public Vector3 playerPositionBeforeBattle; // 전투 전 플레이어 위치
    [HideInInspector] public string returnSceneAfterBattle;      // 전투 후 돌아갈 씬 이름
    [HideInInspector] public string currentMonsterId;            // 현재 전투 중인 몬스터 ID

    // 진행 상황 플래그
    [Header("Event Flags")]
    public HashSet<string> triggeredNoticeIds = new HashSet<string>();       // NoticeUI

    public bool collectedLibraryBossReward = false;                          // 도서관 보상 아이템 획득 여부
    public bool isLibraryPurified = false;                                   // 도서관 정화 여부

    [Header("Combat States")]
    public HashSet<string> defeatedMonsterIds = new HashSet<string>();

    // 도서관 미니게임 먼지
    [Header("Dust States")]
    public HashSet<string> cleanedDustIds = new HashSet<string>();
    public bool isDustCleaningQuestCompleted = false;

    // 플레이 타임 측정
    [Header("Play Time")]
    public float totalPlayTime;   // 초 단위 누적 플레이 시간
    public bool isCountingTime = true;   // 일시정지 등일 때 멈출 용

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeGameState();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (!isCountingTime) return;

        // Time.deltaTime = 프레임 간 경과 시간(초)
        totalPlayTime += Time.deltaTime;
    }

    // 게임 상태를 초기화하는 함수
    private void InitializeGameState()
    {
        // 플레이어 체력을 최대로 설정
        playerHP = playerMaxHP;
        playerGold = 0;

        // 인벤토리 초기화 후 기본템(사과) 추가
        inventoryItems.Clear();
        inventoryItems.Add("apple", 2);
        // 예: questStates.Clear();

        collectedSceneObjectIDs.Clear();
        defeatedMonsterIds.Clear();
    }

    public bool IsMonsterDefeated(string monsterId)
    {
        return defeatedMonsterIds.Contains(monsterId);
    }

    // 체력 변경은 이 함수만 통해서 하도록(클램프 + 이벤트 발행)
    public void ChangeHP(float delta)
    {
        float prev = playerHP;
        playerHP = Mathf.Clamp(playerHP + delta, 0f, playerMaxHP);
        if (!Mathf.Approximately(prev, playerHP))
        {
            OnPlayerHpChanged?.Invoke(playerHP, playerMaxHP);
        }
    }

    // 직접 세팅용도
    public void SetHP(float value)
    {
        float clamped = Mathf.Clamp(value, 0f, playerMaxHP);
        if (!Mathf.Approximately(playerHP, clamped))
        {
            playerHP = clamped;
            OnPlayerHpChanged?.Invoke(playerHP, playerMaxHP);
        }
    }

    // 세이브 시 텍스트로 쓸 플레이타임 포맷 함수
    public string GetFormattedPlayTime()
    {
        int totalSec = Mathf.FloorToInt(totalPlayTime);
        int h = totalSec / 3600;
        int m = (totalSec % 3600) / 60;
        int s = totalSec % 60;
        return $"{h}:{m:00}:{s:00}";   // 1:01:35 이렇게
    }

    // 해쉬셋 저장 리스트로 변환 
    public SaveData CreateSaveData() // 해쉬셋 리스트로 변환
    {
        SaveData data = new SaveData();

        // 기본 스탯
        data.playerHP = playerHP;
        data.playerMaxHP = playerMaxHP;
        data.playerGold = playerGold;
        data.totalPlayTime = totalPlayTime;

        data.nextSpawnPointId = nextSpawnPointId;
        data.currentSceneName = SceneManager.GetActiveScene().name;

        // 인벤토리 (Dictionary → 두 개의 List로 분해)
        data.inventoryItemKeys = inventoryItems.Keys.ToList();
        data.inventoryItemValues = inventoryItems.Values.ToList();

        // HashSet → List
        data.collectedSceneObjectIDs = collectedSceneObjectIDs.ToList();
        data.acceptedQuests = acceptedQuests.ToList();
        data.completedQuests = completedQuests.ToList();
        data.objectiveReachedQuests = objectiveReachedQuests.ToList();
        data.defeatedMonsterIds = defeatedMonsterIds.ToList();
        data.cleanedDustIds = cleanedDustIds.ToList();

        data.isDustCleaningQuestCompleted = isDustCleaningQuestCompleted;
        data.collectedLibraryBossReward = collectedLibraryBossReward;

        return data;
    }
}

