using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WeaponSlotManager : MonoBehaviour
{
    public Image[] weaponIcons; // 슬롯 이미지

    private List<WeaponData> equippedWeapons = new List<WeaponData>(); // 슬롯에 저장된 무기 정보 저장

    public void AssignWeaponToSlot(Sprite weaponSprite, string name, int minDamage, int maxDamage)
    {
        for (int i = 0; i < weaponIcons.Length; i++)
        {
            if (weaponIcons[i].sprite == null) // 비어있는 슬롯 찾기
            {
                weaponIcons[i].sprite = weaponSprite;
                weaponIcons[i].color = Color.white; // 이미지가 비활성화 상태였다면 표시되도록

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

    public void SetupWeaponSlots(List<WeaponData> weaponsToEquip)
    {
        // 기존 정보 초기화
        equippedWeapons.Clear();

        // 슬롯 개수만큼 반복
        for (int i = 0; i < weaponIcons.Length; i++)
        {
            if (i < weaponsToEquip.Count)
            {
                WeaponData data = weaponsToEquip[i];

                weaponIcons[i].gameObject.SetActive(true); // 슬롯 켜기
                weaponIcons[i].sprite = data.image;        // 이미지 변경

                Color c = weaponIcons[i].color;
                c.a = 1f;
                weaponIcons[i].color = c;

                // 내부 리스트에 저장
                equippedWeapons.Add(data);
            }
            // 데이터가 없으면
            else
            {
                weaponIcons[i].sprite = null; // 이미지 비우기
                weaponIcons[i].gameObject.SetActive(false); // 슬롯 숨기기
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