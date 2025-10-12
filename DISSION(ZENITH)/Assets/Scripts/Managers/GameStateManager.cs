using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // 플레이어 상태
    [Header("Player Stats")]
    public float playerHP;// 현재 체력
    public float playerMaxHP = 100f;  // 최대 체력

    // 인벤토리
    [Header("Inventory")]
    public Dictionary<string, int> inventoryItems = new Dictionary<string, int>();
    public int playerGold;

    // 퀘스트
    [Header("Quest Status")]
    public HashSet<string> acceptedQuests = new HashSet<string>();           // 진행 중인 퀘스트 목록
    public HashSet<string> completedQuests = new HashSet<string>();          // 완료한 퀘스트 목록
    public HashSet<string> objectiveReachedQuests = new HashSet<string>();   // 목표 달성한 퀘스트 목록


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

        // 인벤토리 초기화 후 기본템(사과) 추가
        inventoryItems.Clear();
        inventoryItems.Add("apple", 2); 
        // 예: questStates.Clear();
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
}

