using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public Image fillImage; // 체력 채워지는 이미지
    private float maxHealth;

    public void Initialize(float maxHP)
    {
        maxHealth = maxHP;
        UpdateHealth(maxHP);
    }

    public void UpdateHealth(float currentHP)
    {
        float fill = Mathf.Clamp01(currentHP / maxHealth);
        fillImage.fillAmount = fill;
    }
}
