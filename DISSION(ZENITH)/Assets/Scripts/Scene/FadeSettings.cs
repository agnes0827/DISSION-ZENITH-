using UnityEngine;

public class FadeSettings : MonoBehaviour
{
    [Header("페이드 동작 설정")]
    public bool enableFade = true;        // 페이드 효과 사용 여부
    public Color fadeColor = Color.black; // 페이드 색상

    [Header("시간 설정)")]
    public float fadeInDuration = 0.95f; 
    public float fadeOutDuration = 0.4f; 
    public float startDelay = 0.15f;     
}