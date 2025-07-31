using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WeaponData
{
    public string name; // ���� �̸� ����
    public Sprite image; // ���� �̹��� ����
    public int minDamage; // �ּ� ���ݷ�
    public int maxDamage; // �ִ� ���ݷ�

    public WeaponData(string name, Sprite image, int minDamage, int maxDamage)
    {
        this.name = name;
        this.image = image;
        this.minDamage = minDamage;
        this.maxDamage = maxDamage;
    }
}
