using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WeaponData
{
    public string name; // ���� �̸� ����
    public Sprite image; // ���� �̹��� ����
    public int power; // ���� ���ݷ� ����

    public WeaponData(string name, Sprite image, int power)
    {
        this.name = name;
        this.image = image;
        this.power = power;
    }
}
