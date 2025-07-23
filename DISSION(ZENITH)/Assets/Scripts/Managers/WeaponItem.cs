using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class WeaponItem : MonoBehaviour
{
    public Image weaponImage; // 이 무기의 이미지 (프리팹 내부에 연결)
    public Text weaponNameText; // 무기의 이름
    public Text weaponPowerText; // 무기의 공격력
    public Text weaponMaxUsageText; // 무기의 최대 공격 횟수

    private void Start()
    {
        // 클릭 이벤트 등록
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        var slotManager = FindObjectOfType<WeaponSlotManager>();

        string weaponName = weaponNameText.text;
        int power = int.Parse(weaponPowerText.text);
        int maxUsage = int.Parse(weaponMaxUsageText.text);

        slotManager.AssignWeaponToSlot(weaponImage.sprite, weaponName, power, maxUsage);
    }
}