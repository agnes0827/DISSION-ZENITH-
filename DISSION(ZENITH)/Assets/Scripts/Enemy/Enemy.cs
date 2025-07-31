using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp = 100;

    public event Action<int> OnDamaged;  // 남은 HP 전달
    public event Action OnDied;          // 사망 알림

    public void TakeDamage(int amount)
    {
        hp -= amount;
        Debug.Log($"적이 {amount} 피해를 입었습니다! 남은 체력: {hp}");

        OnDamaged?.Invoke(hp);

        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("적이 쓰러졌습니다!");
        OnDied?.Invoke();
        // 여기에 애니메이션, 제거 처리
    }
}
