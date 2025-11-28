using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    // 기본 상태들
    public float playerHP;
    public float playerMaxHP;
    public int playerGold;
    public float totalPlayTime;

    public string nextSpawnPointId;

    // 필요하면 현재 씬 이름도
    public string currentSceneName;

    // --- 인벤토리 (Dictionary는 JsonUtility가 못 써서 리스트 2개로 분리) ---
    public List<string> inventoryItemKeys = new List<string>();
    public List<int> inventoryItemValues = new List<int>();

    // 씬 오브젝트 상태 (HashSet → List)
    public List<string> collectedSceneObjectIDs = new List<string>();

    // 퀘스트
    public List<string> acceptedQuests = new List<string>();
    public List<string> completedQuests = new List<string>();
    public List<string> objectiveReachedQuests = new List<string>();

    // 전투 관련, 먼지 상태 등등 필요하면 계속 추가
    public List<string> defeatedMonsterIds = new List<string>();
    public List<string> cleanedDustIds = new List<string>();

    public bool isDustCleaningQuestCompleted;
    public bool collectedLibraryBossReward;
}


