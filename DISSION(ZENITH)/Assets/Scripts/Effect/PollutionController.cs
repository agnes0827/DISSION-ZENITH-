using System.Collections;
using UnityEngine;

public class PollutionController : MonoBehaviour
{
    [Header("연결 요소")]
    public SpriteRenderer fogOverlay;    // 뿌연 안개 이미지 (선택)
    public ParticleSystem dustParticles; // 먼지 파티클 (필수)

    [Header("설정")]
    public float fadeDuration = 2.0f;    // 사라지는 데 걸리는 시간

    // 정화 시작 함수 (외부에서 호출)
    public void StartPurification()
    {
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        // 1. 먼지 파티클 생성 중단 (기존 먼지는 서서히 사라짐)
        if (dustParticles != null)
        {
            var emission = dustParticles.emission;
            emission.enabled = false;
        }

        // 2. 안개 이미지 투명하게 만들기 (페이드 아웃)
        if (fogOverlay != null)
        {
            Color startColor = fogOverlay.color;
            float time = 0f;

            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startColor.a, 0f, time / fadeDuration);

                Color newColor = startColor;
                newColor.a = newAlpha;
                fogOverlay.color = newColor;

                yield return null;
            }

            fogOverlay.gameObject.SetActive(false); // 안개 끄기
        }

        // 3. 파티클이 화면에서 다 사라질 때까지 조금 대기했다가 오브젝트 끄기
        if (dustParticles != null)
        {
            // 파티클 수명만큼 대기 (혹은 3초 정도)
            yield return new WaitForSeconds(dustParticles.main.startLifetime.constantMax);
            dustParticles.gameObject.SetActive(false);
        }

        gameObject.SetActive(false); 
    }

    // 이미 정화된 상태라면 즉시 끄기 (로딩 직후 호출용)
    public void SetPurifiedImmediate()
    {
        if (fogOverlay != null) fogOverlay.gameObject.SetActive(false);
        if (dustParticles != null) dustParticles.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}