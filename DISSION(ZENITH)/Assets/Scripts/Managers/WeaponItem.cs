using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponItem : MonoBehaviour
{
    public Image weaponImage; // 이 무기의 이미지 (프리팹 내부에 연결)
    private WeaponSlotManager slotManager;

    private void Start()
    {
        // 씬에서 WeaponSlotManager 찾기
        slotManager = FindObjectOfType<WeaponSlotManager>();

        // 클릭 이벤트 등록
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (slotManager != null)
        {
            slotManager.AssignWeaponToSlot(weaponImage.sprite);
        }
    }
}
