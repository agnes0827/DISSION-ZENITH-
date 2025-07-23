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
    public int maxUsage = -1; // ������ 3�̰� ������ ������ ó��

    public WeaponData(string name, Sprite image, int power, int maxUsage)
    {
        this.name = name;
        this.image = image;
        this.power = power;
        this.maxUsage = maxUsage;
    }
}
