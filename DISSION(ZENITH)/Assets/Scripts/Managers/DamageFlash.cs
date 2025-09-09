using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class DamageFlash : MonoBehaviour
{
    private Image spriteImage; // ui �̹���
    private Color originalColor; // ���� �� ����

    void Awake()
    {
        spriteImage = GetComponent<Image>();
        originalColor = spriteImage.color;
    }

    // �ܺο��� ȣ�� �� �ǰ� ȿ�� ����(���� ����)
    public void FlashRed(float duration = 1f)
    {
        StopAllCoroutines(); // �ߺ� ���� ����
        StartCoroutine(FlashRoutine(duration));
    }

    private IEnumerator FlashRoutine(float duration)
    {
        // ������ ����
        spriteImage.color = Color.red;

        // duration ���� ����
        yield return new WaitForSeconds(duration);

        // ���� ������ ����
        spriteImage.color = originalColor;
    }
}
