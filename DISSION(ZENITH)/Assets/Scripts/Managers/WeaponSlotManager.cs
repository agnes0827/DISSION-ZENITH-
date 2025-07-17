using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotManager : MonoBehaviour
{
    public Image[] weaponSlots; // ���� ���� 3���� �ν����Ϳ��� ����

    public void AssignWeaponToSlot(Sprite weaponSprite)
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i].sprite == null) // ����ִ� ���� ã��
            {
                weaponSlots[i].sprite = weaponSprite;
                weaponSlots[i].color = Color.white; // �̹����� ��Ȱ��ȭ ���¿��ٸ� ǥ�õǵ���
                break;
            }
        }
    }
}