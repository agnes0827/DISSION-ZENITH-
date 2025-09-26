using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatterOnDeath : MonoBehaviour
{
    [Header("Grid")]
    public int cols = 6;
    public int rows = 6;

    [Header("Physics")]
    public float explodeForce = 6f;      // 조각이 튀는 세기
    public float randomForce = 2f;       // 랜덤 추가
    public float torque = 180f;          // 회전 토크(±범위)
    public float pieceGravity = 1f;

    [Header("VFX")]
    public float life = 0.8f;            // 조각이 사라지기까지 시간(페이드 포함)
    public float fadeTime = 0.4f;        // 마지막 페이드 시간
    public Material materialOverride;    // (선택) 원본과 다른 머티리얼 쓸 때

    private SpriteRenderer _sr;

    void Awake() => _sr = GetComponent<SpriteRenderer>();

    /// <summary>적을 처치하는 순간 호출</summary>
    public void Shatter()
    {
        if (_sr == null || _sr.sprite == null) { Destroy(gameObject); return; }

        var sprite = _sr.sprite;
        var tex = sprite.texture;
        var rect = sprite.rect;                  // px 단위
        var ppu = sprite.pixelsPerUnit;

        float cellW = rect.width / cols;
        float cellH = rect.height / rows;

        // 원본 끄기
        _sr.enabled = false;

        // 원본의 렌더 순서/레이어 보존
        int sortingOrder = _sr.sortingOrder;
        string sortingLayer = _sr.sortingLayerName;

        // 월드 중심 기준 오프셋 계산(로컬)
        Vector2 centerPx = new(rect.width / 2f, rect.height / 2f);

        for (int y = 0; y < rows; y++)
            for (int x = 0; x < cols; x++)
            {
                // 스프라이트 조각 만들기 (원본 텍스처의 일부분 참조)
                var subRect = new Rect(
                    rect.x + x * cellW,
                    rect.y + (y * cellH),
                    cellW, cellH
                );
                var sub = Sprite.Create(tex, subRect, new Vector2(0.5f, 0.5f), ppu);

                // 조각 오브젝트
                var go = new GameObject($"piece_{x}_{y}");
                go.transform.position = transform.position;
                go.transform.rotation = transform.rotation;

                var psr = go.AddComponent<SpriteRenderer>();
                psr.sprite = sub;
                psr.sortingLayerName = sortingLayer;
                psr.sortingOrder = sortingOrder;
                if (materialOverride != null) psr.material = materialOverride;

                // 조각의 로컬 오프셋(픽셀→유닛 변환)
                Vector2 pieceCenterPx = new(
                    x * cellW + cellW / 2f,
                    y * cellH + cellH / 2f
                );
                Vector2 offsetPx = pieceCenterPx - centerPx;
                Vector3 localOffset = new(offsetPx.x / ppu, offsetPx.y / ppu, 0f);
                go.transform.position += localOffset;

                // 물리 부여
                var rb = go.AddComponent<Rigidbody2D>();
                rb.gravityScale = pieceGravity;
                var col = go.AddComponent<BoxCollider2D>();
                col.size = new Vector2(cellW / ppu, cellH / ppu);

                // 바깥쪽+랜덤 방향으로 폭발
                Vector2 dir = (localOffset).normalized;        // 중심에서 바깥
                if (dir.sqrMagnitude < 0.01f) dir = Random.insideUnitCircle.normalized;
                Vector2 impulse = dir * explodeForce + Random.insideUnitCircle * randomForce;
                rb.AddForce(impulse, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-torque, torque), ForceMode2D.Impulse);

                // 수명 & 페이드
                StartCoroutine(FadeAndDie(psr, rb, life, fadeTime));
            }

        // 원본 제거
        Destroy(gameObject);
    }

    private IEnumerator FadeAndDie(SpriteRenderer sr, Rigidbody2D rb, float ttl, float fade)
    {
        // 살짝 버티다가
        float t = 0f;
        while (t < ttl - fade)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // 페이드 아웃
        float f = 0f;
        Color c = sr.color;
        while (f < fade)
        {
            f += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, f / fade);
            sr.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }

        Destroy(sr.gameObject);
    }
}
