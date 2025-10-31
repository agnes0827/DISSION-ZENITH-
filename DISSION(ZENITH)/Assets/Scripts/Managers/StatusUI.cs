using UnityEngine;
using TMPro;

public class StatusUI : MonoBehaviour
{
    [SerializeField] private TMP_Text hpText;
    // [SerializeField] private HpBar healthBar;

    void Start()
    {
        //if (healthBar != null)
        //{
        //    healthBar.Initialize(GameStateManager.Instance.playerMaxHP);
        //}
    }

    private void OnEnable()
    {
        UpdateStatus();
    }

    public void UpdateStatus()
    {
        float currentHP = GameStateManager.Instance.playerHP;
        float maxHP = GameStateManager.Instance.playerMaxHP;

        if (hpText != null)
        {
            hpText.text = $"Hp {currentHP} / {maxHP}";
        }

        //if (healthBar != null)
        //{
        //    healthBar.UpdateHealth(currentHP);
        //}
    }
}