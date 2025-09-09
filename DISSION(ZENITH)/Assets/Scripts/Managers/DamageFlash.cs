using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class DamageFlash : MonoBehaviour
{
    private Image spriteImage; // ui 이미지
    private Color originalColor; // 원래 색 저장

    void Awake()
    {
        spriteImage = GetComponent<Image>();
        originalColor = spriteImage.color;
    }

    // 외부에서 호출 → 피격 효과 실행(색상 변경)
    public void FlashRed(float duration = 1f)
    {
        StopAllCoroutines(); // 중복 실행 방지
        StartCoroutine(FlashRoutine(duration));
    }

    private IEnumerator FlashRoutine(float duration)
    {
        // 빨갛게 변함
        spriteImage.color = Color.red;

        // duration 동안 유지
        yield return new WaitForSeconds(duration);

        // 원래 색으로 복원
        spriteImage.color = originalColor;
    }
}
