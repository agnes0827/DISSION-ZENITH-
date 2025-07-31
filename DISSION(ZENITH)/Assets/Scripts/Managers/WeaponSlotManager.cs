using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WeaponSlotManager : MonoBehaviour
{
    public Image[] weaponSlots; // 무기 이미지 슬롯 3개를 인스펙터에서 연결
    private List<WeaponData> equippedWeapons = new List<WeaponData>(); // 슬롯에 저장된 무기 정보 저장

    public void AssignWeaponToSlot(Sprite weaponSprite, string name, int minDamage, int maxDamage)
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i].sprite == null) // 비어있는 슬롯 찾기
            {
                weaponSlots[i].sprite = weaponSprite;
                weaponSlots[i].color = Color.white; // 이미지가 비활성화 상태였다면 표시되도록

                WeaponData data = new WeaponData(name, weaponSprite, minDamage, maxDamage);

                // 리스트 크기 보정
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