using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponItem : MonoBehaviour
{
    public Image weaponImage; // �� ������ �̹��� (������ ���ο� ����)
    private WeaponSlotManager slotManager;

    private void Start()
    {
        // ������ WeaponSlotManager ã��
        slotManager = FindObjectOfType<WeaponSlotManager>();

        // Ŭ�� �̺�Ʈ ���
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
