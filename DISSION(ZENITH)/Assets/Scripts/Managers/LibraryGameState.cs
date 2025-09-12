using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibraryGameState : MonoBehaviour
{
    public static LibraryGameState Instance { get; private set; }

    // 최근 전투를 시작한 몬스터의 ID
    public string lastMonsterId;

    // 이미 처치한 몬스터들
    private HashSet<string> defeatedMonsters = new HashSet<string>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void MarkDefeated(string monsterId)
    {
        if (!string.IsNullOrEmpty(monsterId))
            defeatedMonsters.Add(monsterId);
    }

    public bool IsDefeated(string monsterId)
    {
        return !string.IsNullOrEmpty(monsterId) && defeatedMonsters.Contains(monsterId);
    }
}
