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
    public Text weaponMaxUsageText; // ������ �ִ� ���� Ƚ��

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
        int maxUsage = int.Parse(weaponMaxUsageText.text);

        slotManager.AssignWeaponToSlot(weaponImage.sprite, weaponName, power, maxUsage);
    }
}