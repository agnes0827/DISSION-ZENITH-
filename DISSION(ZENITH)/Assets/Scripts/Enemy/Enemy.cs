using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp = 100;

    public event Action<int, int> OnDamaged;  // 남은 HP 전달
    public event Action OnDied;          // 사망 알림

 
    public void TakeDamage(int amount)
    {
        int previousHp = hp;
        hp -= amount;
        if (hp < 0) hp = 0;

        int damageTaken = previousHp - hp;
        OnDamaged?.Invoke(hp, damageTaken); // 남은 체력, 입은 데미지 전달

        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {

        OnDied?.Invoke();
        // 여기에 애니메이션, 제거 처리
    }
}
