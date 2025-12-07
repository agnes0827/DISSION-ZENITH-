using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchFlicker : MonoBehaviour
{
    private Light2D _light2D;

    [Header("설정")]
    [Tooltip("기본 빛의 밝기")]
    public float baseIntensity = 1f;
    [Tooltip("빛의 밝기가 흔들리는 정도")]
    public float intensityFluctuation = 0.2f;

    [Space(10)]
    [Tooltip("기본 빛의 범위(반지름)")]
    public float baseRadius = 3f;
    [Tooltip("빛의 크기가 흔들리는 정도")]
    public float radiusFluctuation = 0.1f;

    [Space(10)]
    [Tooltip("흔들리는 속도 (높을수록 파르르 떰)")]
    public float flickerSpeed = 3f;

    private float _timeOffset;

    void Awake()
    {
        _light2D = GetComponent<Light2D>();
        // 모든 횃불이 똑같이 깜빡이지 않도록 랜덤 시간값 설정
        _timeOffset = Random.Range(0f, 100f);

        // baseIntensity = _light2D.intensity;
        // baseRadius = _light2D.pointLightOuterRadius;
    }

    void Update()
    {
        if (_light2D == null) return;

        // 시간 흐름에 따라 부드러운 노이즈 값 생성 (0.0 ~ 1.0 사이 값 반환)
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed + _timeOffset, 0f);

        // 강도(Intensity) 흔들기: 기본값 + (노이즈 * 변동폭) - (변동폭 절반) -> 위아래로 흔들리게
        _light2D.intensity = baseIntensity + (noise * intensityFluctuation * 2) - intensityFluctuation;

        // 범위(Radius) 흔들기 (선택사항: 원하면 주석 처리 가능)
        _light2D.pointLightOuterRadius = baseRadius + (noise * radiusFluctuation * 2) - radiusFluctuation;
    }
}