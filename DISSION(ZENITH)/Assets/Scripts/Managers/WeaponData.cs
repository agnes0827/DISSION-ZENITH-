using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WeaponData
{
    public string name; // 무기 이름 저장
    public Sprite image; // 무기 이미지 저장
    public int power; // 무기 공격력 저장

    public WeaponData(string name, Sprite image, int power)
    {
        this.name = name;
        this.image = image;
        this.power = power;
    }
}
