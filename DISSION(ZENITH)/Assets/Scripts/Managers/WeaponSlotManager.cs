using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WeaponSlotManager : MonoBehaviour
{
    public WeaponSlotUI[] weaponSlots; // 인스펙터에서 3개 슬롯 오브젝트 연결
    private List<WeaponData> equippedWeapons = new List<WeaponData>(); // 슬롯에 저장된 무기 정보 저장

    public void AssignWeaponToSlot(Sprite weaponSprite, string name, int power, int maxUsage)
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i].weaponImage.sprite == null) // 비어있는 슬롯 찾기
            {

                // 무기 데이터 저장
                WeaponData data = new WeaponData(name, weaponSprite, power, maxUsage);

                // 슬롯 UI 설정
                weaponSlots[i].SetSlot(weaponSprite, maxUsage, data);


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