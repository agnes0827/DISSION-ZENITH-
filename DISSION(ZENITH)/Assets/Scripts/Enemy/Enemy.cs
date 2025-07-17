using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp = 100;

    public void TakeDamage(int amount)
    {
        hp -= amount;
        Debug.Log($"적이 {amount} 피해를 입었습니다! 남은 체력: {hp}");
        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("적이 쓰러졌습니다!");
        // 여기에 애니메이션, 제거 처리
    }
}
