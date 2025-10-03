using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 퀘스트 상태 저장
public enum QuestStatus
{
    NotStarted, // 시작 안 함
    InProgress, // 진행 중
    Completed   // 완료
}

/// <summary>
/// 게임의 모든 영구 데이터를 저장하고 관리하는 중앙 데이터베이스입니다.
/// 씬이 변경되어도 파괴되지 않으며, 인벤토리, 퀘스트 진행도, 오브젝트 상태 등
/// 게임이 반드시 기억해야 할 모든 정보의 '원본'을 소유합니다.
/// 다른 매니저들은 이 클래스를 통해 데이터에 접근해야 합니다.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    // 플레이어 상태
    [Header("Player Stats")]
    public float playerHP;            // 현재 체력
    public float playerMaxHP = 100f;  // 최대 체력

    // 인벤토리
    [Header("Inventory")]
    public Dictionary<string, int> inventoryItems = new Dictionary<string, int>();
    public int playerGold;

    // 퀘스트
    [Header("Quest Status")]
    public Dictionary<string, QuestStatus> questStates = new Dictionary<string, QuestStatus>();

    // 아티팩트
    [Header("Artifact Status")]
    public List<string> collectedArtifactIDs = new List<string>();

    // 도서관 미니게임 먼지
    [Header("Dust States")]
    public HashSet<string> cleanedDustIds = new HashSet<string>();
    public bool isDustCleaningQuestCompleted = false;

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

    // 게임 상태를 초기화하는 함수
    private void InitializeGameState()
    {
        // 플레이어 체력을 최대로 설정
        playerHP = playerMaxHP;
        playerGold = 0;

        // 다른 데이터들도 필요하다면 여기서 초기화
        // 예: inventoryItems.Clear();
        // 예: questStates.Clear();
    }
}

