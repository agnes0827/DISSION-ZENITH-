using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WeaponSlotManager : MonoBehaviour
{
    public Image[] weaponSlots; // ���� �̹��� ���� 3���� �ν����Ϳ��� ����
    private List<WeaponData> equippedWeapons = new List<WeaponData>(); // ���Կ� ����� ���� ���� ����

    public void AssignWeaponToSlot(Sprite weaponSprite, string name, int minDamage, int maxDamage)
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i].sprite == null) // ����ִ� ���� ã��
            {
                weaponSlots[i].sprite = weaponSprite;
                weaponSlots[i].color = Color.white; // �̹����� ��Ȱ��ȭ ���¿��ٸ� ǥ�õǵ���

                WeaponData data = new WeaponData(name, weaponSprite, minDamage, maxDamage);

                // ����Ʈ ũ�� ����
                while (equippedWeapons.Count <= i)
                {
                    equippedWeapons.Add(null);
                }

                equippedWeapons[i] = data;

                break;
            }
        }
    }

    public WeaponData GetWeaponData(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < equippedWeapons.Count)
            return equippedWeapons[slotIndex];
        return null;
    }
}