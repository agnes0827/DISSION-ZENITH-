using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    public Image weaponImage; // �� ���� �̹���
    public Text usageText; // ��� Ƚ�� �ؽ�Ʈ
    public Button weaponButton; // ��ư
    private int currentUsage;

    public void SetSlot(Sprite sprite, int maxUsage, WeaponData data)
    {
        weaponImage.sprite = sprite;
        weaponImage.color = Color.white;
        weaponButton.interactable = true;

        currentUsage = maxUsage;

        if (maxUsage < 4) // ������ �ִ� ��� Ƚ�� �ؽ�Ʈ ǥ��
        {
            usageText.gameObject.SetActive(true);
            usageText.text = "X" + currentUsage;
        }
        else
        {
            usageText.gameObject.SetActive(false);
        }
    }

    // ��� �������� ����
    public bool CanUse()
    {
        return currentUsage > 0; // 0 �̻��̸� ��� ����
    }

    // ���� ��� �� ȣ��
    public void UseWeapon()
    {
        if (currentUsage > 0)
        {
            currentUsage--;

            usageText.text = "X" + currentUsage; // ����� ������ ���� ��� ���� Ƚ�� ǥ��

            if (currentUsage == 0) // 3�� ����ϸ� �̹��� ȸ������, ��ư ��Ȱ��ȭ
            {
                weaponImage.color = new Color32(0x6A, 0x6A, 0x6A, 255);
                weaponButton.interactable = false;
            }
        }
    }
}
