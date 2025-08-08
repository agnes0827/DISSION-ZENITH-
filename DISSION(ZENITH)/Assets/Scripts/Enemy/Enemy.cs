using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp = 100;

    public event Action<int, int> OnDamaged;  // ���� HP ����
    public event Action OnDied;          // ��� �˸�

 
    public void TakeDamage(int amount)
    {
        int previousHp = hp;
        hp -= amount;
        if (hp < 0) hp = 0;

        int damageTaken = previousHp - hp;
        OnDamaged?.Invoke(hp, damageTaken); // ���� ü��, ���� ������ ����

        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {

        OnDied?.Invoke();
        // ���⿡ �ִϸ��̼�, ���� ó��
    }
}
