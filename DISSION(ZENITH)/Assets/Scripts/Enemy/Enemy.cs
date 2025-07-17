using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp = 100;

    public void TakeDamage(int amount)
    {
        hp -= amount;
        Debug.Log($"���� {amount} ���ظ� �Ծ����ϴ�! ���� ü��: {hp}");
        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("���� ���������ϴ�!");
        // ���⿡ �ִϸ��̼�, ���� ó��
    }
}
