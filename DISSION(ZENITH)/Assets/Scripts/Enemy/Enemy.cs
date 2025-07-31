using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp = 100;

    public event Action<int> OnDamaged;  // ���� HP ����
    public event Action OnDied;          // ��� �˸�

    public void TakeDamage(int amount)
    {
        hp -= amount;
        Debug.Log($"���� {amount} ���ظ� �Ծ����ϴ�! ���� ü��: {hp}");

        OnDamaged?.Invoke(hp);

        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("���� ���������ϴ�!");
        OnDied?.Invoke();
        // ���⿡ �ִϸ��̼�, ���� ó��
    }
}
