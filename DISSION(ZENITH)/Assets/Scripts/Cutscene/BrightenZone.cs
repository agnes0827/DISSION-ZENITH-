using UnityEngine;
using UnityEngine.Rendering.Universal; // Light2D
using UnityEngine.UI; // UI 제어

public class BrightenZone : MonoBehaviour
{
    [Header("연결 정보")]
    public Light2D globalLight;
    public Image whiteOverlay; // 화면 하얗게 덮을 패널
    public Transform player;

    [Header("구역 설정 (Y축)")]
    public Transform startPoint; // 여기서부터 밝아지기 시작
    public Transform endPoint;   // 여기가 완전히 하얀 세상(출구)

    [Header("밝기 설정")]
    [Tooltip("StartPoint에 있을 때의 밝기 (횃불 밝기와 맞추세요! 보통 1.0)")]
    public float startIntensity = 1.0f;
    [Tooltip("EndPoint에 도달했을 때의 밝기 (화이트아웃)")]
    public float targetIntensity = 5.0f;

    private void Update()
    {
        if (player == null || startPoint == null || endPoint == null) return;

        // 1. Y축 기준으로 진행률 계산
        float playerY = player.position.y;
        float startY = startPoint.position.y;
        float endY = endPoint.position.y;

        // 진행률 (0 ~ 1)
        float progress = Mathf.InverseLerp(startY, endY, playerY);

        // [핵심] 플레이어가 시작점보다 아래에 있으면(progress 0), 
        // 이 스크립트는 조명에 간섭하지 않는다! (횃불 켠 상태 유지)
        if (progress <= 0)
        {
            if (whiteOverlay != null)
            {
                Color c = whiteOverlay.color;
                c.a = 0; // 하얀 패널은 꺼둠
                whiteOverlay.color = c;
            }
            return;
        }

        // 2. 조명 밝기 조절 (StartIntensity ~ TargetIntensity)
        // 예: 횃불 밝기(1.0)에서 시작해서 -> 눈부심(5.0)으로
        if (globalLight != null)
        {
            globalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, progress);
        }

        // 3. 화면 하얗게 (UI 투명도)
        if (whiteOverlay != null)
        {
            Color color = whiteOverlay.color;
            color.a = progress; // 0(투명) ~ 1(불투명)
            whiteOverlay.color = color;
        }
    }
        private void OnDisable()
    {
        if (whiteOverlay != null)
        {
            Color color = whiteOverlay.color;
            color.a = 0f; // 투명도 0
            whiteOverlay.color = color;
        }

        if (globalLight != null)
        {
            globalLight.intensity = 1.0f;
        }

        if (player != null)
        {
            // 플레이어 자식에 있는 Light2D를 찾아서 끔
            var playerLight = player.GetComponentInChildren<Light2D>();
            if (playerLight != null) playerLight.gameObject.SetActive(false);
        }
    }
}