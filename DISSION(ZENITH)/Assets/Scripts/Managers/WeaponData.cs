using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WeaponData
{
    public string name; // 무기 이름 저장
    public Sprite image; // 무기 이미지 저장
    public int minDamage; // 최소 공격력
    public int maxDamage; // 최대 공격력

    public WeaponData(string name, Sprite image, int minDamage, int maxDamage)
    {
        this.name = name;
        this.image = image;
        this.minDamage = minDamage;
        this.maxDamage = maxDamage;
    }
}
