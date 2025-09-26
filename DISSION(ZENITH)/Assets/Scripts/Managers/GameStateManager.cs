using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 씬이 바뀌어도 파괴되지 않고, 게임의 모든 핵심 상태를 기억
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public List<string> collectedArtifactIDs = new List<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

