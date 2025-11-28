using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class WeaponItem : MonoBehaviour
{
    public string weaponId;
    public Image weaponImage; // 이 무기의 이미지 (프리팹 내부에 연결)
    public Text weaponNameText; // 무기의 이름

    public int minDamage; // 최소 공격력
    public int maxDamage; // 최대 공격력

    private void Start()
    {
        // 클릭 이벤트 등록
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        var slotManager = FindObjectOfType<WeaponSlotManager>();
        if (slotManager == null) return;

        string displayName = weaponNameText.text;
        slotManager.AssignWeaponToSlot(weaponImage.sprite, weaponId, displayName, minDamage, maxDamage);
    }
}