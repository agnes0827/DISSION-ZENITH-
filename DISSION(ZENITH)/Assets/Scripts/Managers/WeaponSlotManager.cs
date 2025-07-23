using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WeaponSlotManager : MonoBehaviour
{
    public WeaponSlotUI[] weaponSlots; // �ν����Ϳ��� 3�� ���� ������Ʈ ����
    private List<WeaponData> equippedWeapons = new List<WeaponData>(); // ���Կ� ����� ���� ���� ����

    public void AssignWeaponToSlot(Sprite weaponSprite, string name, int power, int maxUsage)
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i].weaponImage.sprite == null) // ����ִ� ���� ã��
            {

                // ���� ������ ����
                WeaponData data = new WeaponData(name, weaponSprite, power, maxUsage);

                // ���� UI ����
                weaponSlots[i].SetSlot(weaponSprite, maxUsage, data);


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