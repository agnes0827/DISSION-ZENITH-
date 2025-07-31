using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp = 100;
    public HpBar healthBar;

    public event Action<int> OnDamaged;  // 남은 HP 전달
    public event Action OnDied;          // 사망 알림

    void Start()
    {
        healthBar.Initialize(hp); // 체력바 초기화
    }
    public void TakeDamage(int amount)
    {
        hp -= amount;
        if (hp < 0) hp = 0;

        // 체력바 갱신
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
        // 여기에 애니메이션, 제거 처리
    }
}
