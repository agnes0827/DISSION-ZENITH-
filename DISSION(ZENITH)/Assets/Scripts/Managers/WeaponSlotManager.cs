using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotManager : MonoBehaviour
{
    public Image[] weaponIcons; // 무기 아이콘들
    public Image[] cooldownOverlays; // 쿨타임 UI 

    private List<WeaponData> equippedWeapons = new List<WeaponData>(); // 장착된 무기 데이터 리스트

    public void SetupWeaponSlots(List<WeaponData> weaponsToEquip)
    {
        equippedWeapons.Clear();

        for (int i = 0; i < weaponIcons.Length; i++)
        {
            if (i < weaponsToEquip.Count)
            {
                WeaponData data = weaponsToEquip[i];
                equippedWeapons.Add(data);

                weaponIcons[i].gameObject.SetActive(true);
                weaponIcons[i].sprite = data.image;
                weaponIcons[i].color = Color.white;

                // 쿨타임 UI 초기화
                if (cooldownOverlays[i] != null)
                {
                    cooldownOverlays[i].fillAmount = 0;
                }
            }
            else
            {
                weaponIcons[i].gameObject.SetActive(false);
                if (cooldownOverlays.Length > i && cooldownOverlays[i] != null) cooldownOverlays[i].gameObject.SetActive(false);
            }
        }
    }

    public WeaponData GetWeaponData(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < equippedWeapons.Count)
            return equippedWeapons[slotIndex];
        return null;
    }

    public void StartCooldown(int slotIndex, float duration)
    {
        if (slotIndex < 0 || slotIndex >= cooldownOverlays.Length) return;

        StartCoroutine(CooldownCoroutine(slotIndex, duration));
    }

    private IEnumerator CooldownCoroutine(int slotIndex, float duration)
    {
        Image overlay = cooldownOverlays[slotIndex];

        if (overlay == null) yield break;

        overlay.gameObject.SetActive(true);

        float timer = duration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            overlay.fillAmount = timer / duration;

            yield return null;
        }

        overlay.fillAmount = 0;
        overlay.gameObject.SetActive(false);
    }

    public void AssignWeaponToSlot(Sprite image, string id, string displayName, int minDamage, int maxDamage)
    {
        // 슬롯이 꽉 찼으면 무시
        if (equippedWeapons.Count >= weaponIcons.Length)
        {
            Debug.Log("더 이상 무기를 장착할 수 없습니다.");
            return;
        }

        // 새 무기 데이터 생성
        WeaponData newWeapon = new WeaponData(id, displayName, image, minDamage, maxDamage);

        // 리스트에 추가
        equippedWeapons.Add(newWeapon);

        // UI 갱신
        int slotIndex = equippedWeapons.Count - 1;
        weaponIcons[slotIndex].gameObject.SetActive(true);
        weaponIcons[slotIndex].sprite = image;
        weaponIcons[slotIndex].color = Color.white;

        if (cooldownOverlays[slotIndex] != null)
        {
            cooldownOverlays[slotIndex].fillAmount = 0;
            cooldownOverlays[slotIndex].gameObject.SetActive(false);
        }
    }
}
