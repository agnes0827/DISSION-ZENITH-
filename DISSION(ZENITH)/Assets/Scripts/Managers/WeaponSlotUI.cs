using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    public Image weaponImage; // 각 슬롯 이미지
    public Text usageText; // 사용 횟수 텍스트
    public Button weaponButton; // 버튼
    private int currentUsage;

    public void SetSlot(Sprite sprite, int maxUsage, WeaponData data)
    {
        weaponImage.sprite = sprite;
        weaponImage.color = Color.white;
        weaponButton.interactable = true;

        currentUsage = maxUsage;

        if (maxUsage < 4) // 도끼는 최대 사용 횟수 텍스트 표시
        {
            usageText.gameObject.SetActive(true);
            usageText.text = "X" + currentUsage;
        }
        else
        {
            usageText.gameObject.SetActive(false);
        }
    }

    // 사용 가능한지 여부
    public bool CanUse()
    {
        return currentUsage > 0; // 0 이상이면 사용 가능
    }

    // 무기 사용 시 호출
    public void UseWeapon()
    {
        if (currentUsage > 0)
        {
            currentUsage--;

            usageText.text = "X" + currentUsage; // 사용할 때마다 남은 사용 가능 횟수 표시

            if (currentUsage == 0) // 3번 사용하면 이미지 회색으로, 버튼 비활성화
            {
                weaponImage.color = new Color32(0x6A, 0x6A, 0x6A, 255);
                weaponButton.interactable = false;
            }
        }
    }
}
