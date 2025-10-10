using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ����Ʈ ���� ����
public enum QuestStatus
{
    NotStarted, // ���� �� ��
    InProgress, // ���� ��
    Completed   // �Ϸ�
}

/// <summary>
/// ������ ��� ���� �����͸� �����ϰ� �����ϴ� �߾� �����ͺ��̽��Դϴ�.
/// ���� ����Ǿ �ı����� ������, �κ��丮, ����Ʈ ���൵, ������Ʈ ���� ��
/// ������ �ݵ�� ����ؾ� �� ��� ������ '����'�� �����մϴ�.
/// �ٸ� �Ŵ������� �� Ŭ������ ���� �����Ϳ� �����ؾ� �մϴ�.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    // HP ���� �̺�Ʈ: (current, max)
    public event Action<float, float> OnPlayerHpChanged;

    // �÷��̾� ����
    [Header("Player Stats")]
    public float playerHP;// ���� ü��
    public float playerMaxHP = 100f;  // �ִ� ü��

    // �κ��丮
    [Header("Inventory")]
    public Dictionary<string, int> inventoryItems = new Dictionary<string, int>();
    public int playerGold;

    // ����Ʈ
    [Header("Quest Status")]
    public Dictionary<string, QuestStatus> questStates = new Dictionary<string, QuestStatus>();

    // ��Ƽ��Ʈ
    [Header("Artifact Status")]
    public List<string> collectedArtifactIDs = new List<string>();

    // ������ �̴ϰ��� ����
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

    // ���� ���¸� �ʱ�ȭ�ϴ� �Լ�
    private void InitializeGameState()
    {
        // �÷��̾� ü���� �ִ�� ����
        playerHP = playerMaxHP;
        playerGold = 0;

        // �κ��丮 �ʱ�ȭ �� �⺻��(���) �߰�
        inventoryItems.Clear();
        inventoryItems.Add("apple", 2); 
        // ��: questStates.Clear();
    }

    // ü�� ������ �� �Լ��� ���ؼ� �ϵ���(Ŭ���� + �̺�Ʈ ����)
    public void ChangeHP(float delta)
    {
        float prev = playerHP;
        playerHP = Mathf.Clamp(playerHP + delta, 0f, playerMaxHP);
        if (!Mathf.Approximately(prev, playerHP))
        {
            OnPlayerHpChanged?.Invoke(playerHP, playerMaxHP);
        }
    }

    // ���� ���ÿ뵵
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

