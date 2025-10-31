using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public Image white;
    public Image black;
    private Color color;

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);
    
    //페이드아웃
    public void FadeOut(float _speed = 0.02f)
    {
        StartCoroutine(FadeOutCoroutine(_speed));
    }
    IEnumerator FadeOutCoroutine(float _speed)
    {
        color = black.color;

        while (color.a < 1f)
        {
            color.a += _speed;
            black.color = color;
            yield return waitTime;
        }
    }

    //페이드인
    public void FadeIn(float _speed = 0.02f)
    {
        StartCoroutine(FadeInCoroutine(_speed));
    }
    IEnumerator FadeInCoroutine(float _speed)
    {
        if (black.color.a == 0)
        {
            color = black.color;
            color.a = 1f;          // 완전 어둡게 시작
            black.color = color;
        }
        color = black.color;
        while (color.a > 0f)
        {
            color.a -= _speed;
            black.color = color;
            yield return waitTime;
        }
    }
}
