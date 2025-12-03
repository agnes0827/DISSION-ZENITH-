using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("몬스터 정보")]
    public string enemyName = "몬스터";
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
            StartCoroutine(DieRoutine());
        }
    }

    IEnumerator DieRoutine()
    {
        // 1) 조각 효과 있으면 실행
        var shatter = GetComponent<UIShatter>();
        if (shatter != null)
            yield return StartCoroutine(shatter.Play());

        // 2) 사망 이벤트 (연출 후)
        OnDied?.Invoke();

        // 3) 자기 자신 파괴(또는 비활성)
        Destroy(gameObject);
    }
}
