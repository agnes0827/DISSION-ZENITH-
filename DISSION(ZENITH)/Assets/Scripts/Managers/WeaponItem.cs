using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class WeaponItem : MonoBehaviour
{
    public Image weaponImage; // �� ������ �̹��� (������ ���ο� ����)
    public Text weaponNameText; // ������ �̸�
    public Text weaponPowerText; // ������ ���ݷ�

    private void Start()
    {
        // Ŭ�� �̺�Ʈ ���
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        var slotManager = FindObjectOfType<WeaponSlotManager>();

        string weaponName = weaponNameText.text;
        int power = int.Parse(weaponPowerText.text);

        slotManager.AssignWeaponToSlot(weaponImage.sprite, weaponName, power);
    }
}