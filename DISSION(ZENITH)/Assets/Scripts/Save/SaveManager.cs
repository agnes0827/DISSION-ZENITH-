using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public SaveSlotData[] slots = new SaveSlotData[3]; // 파일1~3

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeSlots();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeSlots()
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i] = new SaveSlotData(); // 비어있는 슬롯 생성
    }

    public SaveSlotData GetSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return null;
        return slots[index];
    }

    public void SaveSlot(int index)
    {
        var data = slots[index];
        data.hasSave = true;
        data.playTimeSeconds = Mathf.FloorToInt(GameStateManager.Instance.totalPlayTime);
        // data.faceSprite = /* 현재 플레이어 얼굴 이미지 참조 */;
    }
}
