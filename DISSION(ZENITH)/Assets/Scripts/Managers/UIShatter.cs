using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIShatter : MonoBehaviour
{
    [Header("Grid")]
    public int piecesX = 8;          // 가로 조각 개수
    public int piecesY = 8;          // 세로 조각 개수

    [Header("Motion")]
    public float explodeRadius = 180f;  // 흩어지는 범위(anchoredPosition 오프셋)
    public float rotateMax = 360f;      // 랜덤 회전량(도)
    public float duration = 0.8f;       // 조각 생존 시간(페이드아웃)

    [Header("Randomness")]
    public float upBias = 0.3f;         // 위쪽으로 살짝 더 튀게(0~1)

    private Image src;
    private RectTransform srcRT;
    private List<GameObject> spawned = new List<GameObject>();

    void Awake()
    {
        src = GetComponent<Image>();
        srcRT = GetComponent<RectTransform>();
    }

    public IEnumerator Play()
    {
        SoundManager.Instance.PlaySFX(SfxType.EnemyDeath, 0.5f, false);

        if (src.sprite == null) yield break;
        if (piecesX < 1) piecesX = 1;
        if (piecesY < 1) piecesY = 1;

        // 조각 만들기 전에 원본 크기/스프라이트 정보
        Sprite sprite = src.sprite;
        Texture2D tex = sprite.texture;
        Rect spriteRect = sprite.textureRect; // 텍스처 내 영역(px)

        // 원본 숨김
        src.enabled = false;

        // 조각의 기준 위치(부모 캔버스 좌표 기준)
        Vector2 baseAnchored = srcRT.anchoredPosition;
        Vector2 size = srcRT.rect.size;

        // 각 조각의 UI 크기
        Vector2 pieceSize = new Vector2(size.x / piecesX, size.y / piecesY);

        // 스프라이트에서 잘라올 픽셀 크기
        float rectW = spriteRect.width / piecesX;
        float rectH = spriteRect.height / piecesY;

        // 조각 생성
        for (int y = 0; y < piecesY; y++)
        {
            for (int x = 0; x < piecesX; x++)
            {
                var subRect = new Rect(
                    spriteRect.x + rectW * x,
                    spriteRect.y + rectH * y,
                    rectW,
                    rectH
                );

                Sprite pieceSprite = Sprite.Create(
                    tex,
                    subRect,
                    new Vector2(0.5f, 0.5f),
                    sprite.pixelsPerUnit
                );

                GameObject go = new GameObject($"piece_{x}_{y}", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
                spawned.Add(go);
                go.transform.SetParent(srcRT.parent, worldPositionStays: false);

                var rt = go.GetComponent<RectTransform>();
                rt.sizeDelta = pieceSize;
                // 원본 내부의 해당 조각 위치로 배치
                Vector2 localOffset = new Vector2(
                    (-size.x * 0.5f) + pieceSize.x * (x + 0.5f),
                    (-size.y * 0.5f) + pieceSize.y * (y + 0.5f)
                );
                rt.anchoredPosition = baseAnchored + localOffset;
                rt.localRotation = Quaternion.identity;
                rt.localScale = Vector3.one;

                var img = go.GetComponent<Image>();
                img.sprite = pieceSprite;
                img.preserveAspect = true;
                img.raycastTarget = false; // 클릭 막기

                var cg = go.GetComponent<CanvasGroup>();
                cg.alpha = 1f;

                // 랜덤 이동/회전 타겟 계산
                Vector2 dir = (Random.insideUnitCircle + Vector2.up * upBias).normalized;
                float dist = Random.Range(explodeRadius * 0.4f, explodeRadius);
                Vector2 targetPos = rt.anchoredPosition + dir * dist;
                float rotZ = Random.Range(-rotateMax, rotateMax);

                // 개별 조각 애니메이션
                StartCoroutine(AnimatePiece(rt, cg, targetPos, rotZ));
            }
        }

        // 전체 지속 시간만큼 기다린 후 정리
        yield return new WaitForSeconds(duration);

        foreach (var go in spawned)
            if (go) Destroy(go);
        spawned.Clear();
    }

    private IEnumerator AnimatePiece(RectTransform rt, CanvasGroup cg, Vector2 targetAnchoredPos, float addRotZ)
    {
        float t = 0f;
        Vector2 startPos = rt.anchoredPosition;
        Quaternion startRot = rt.localRotation;
        Quaternion endRot = Quaternion.Euler(0, 0, addRotZ);

        while (t < duration)
        {
            t += Time.deltaTime;
            float k = t / duration;
            // 살짝 튕기는 이징
            float moveK = 1f - Mathf.Pow(1f - k, 2f);
            rt.anchoredPosition = Vector2.LerpUnclamped(startPos, targetAnchoredPos, moveK);
            rt.localRotation = Quaternion.Slerp(startRot, endRot, k);
            cg.alpha = 1f - k;
            yield return null;
        }
        cg.alpha = 0f;
    }
}
