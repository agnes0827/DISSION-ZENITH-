using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WeaponData
{
    public string name;
    public string displayName;   // 표시 이름
    public Sprite image;         // UI 이미지
    public int minDamage;        // 최소 공격력
    public int maxDamage;        // 최대 공격력
    public float cooldown = 0f;  // 쿨타임 (기본 0)

    [System.NonSerialized]
    public float lastUseTime = -9999f; // 마지막 사용 시간

    public WeaponData(string name, string displayName, Sprite image, int minDamage, int maxDamage)
    {
        this.name = name;
        this.displayName = displayName;
        this.image = image;
        this.minDamage = minDamage;
        this.maxDamage = maxDamage;
    }
}