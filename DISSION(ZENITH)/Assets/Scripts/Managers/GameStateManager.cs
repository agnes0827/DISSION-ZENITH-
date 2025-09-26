using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� �ٲ� �ı����� �ʰ�, ������ ��� �ٽ� ���¸� ���
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

