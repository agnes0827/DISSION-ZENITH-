using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveSlotData
{
    public bool hasSave;           // 세이브가 있는 슬롯인지
    public int playTimeSeconds;    // 총 플레이 시간 (초 단위)
}
