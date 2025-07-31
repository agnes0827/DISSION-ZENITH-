using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp = 100;
    public HpBar healthBar;

    public event Action<int> OnDamaged;  // ���� HP ����
    public event Action OnDied;          // ��� �˸�

    void Start()
    {
        healthBar.Initialize(hp); // ü�¹� �ʱ�ȭ
    }
    public void TakeDamage(int amount)
    {
        hp -= amount;
        if (hp < 0) hp = 0;

        // ü�¹� ����
        if (healthBar != null)
            healthBar.UpdateHealth(hp);


        OnDamaged?.Invoke(hp);

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
