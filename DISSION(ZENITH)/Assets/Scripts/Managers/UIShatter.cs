using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIShatter : MonoBehaviour
{
    [Header("Grid")]
    public int piecesX = 8;          // ���� ���� ����
    public int piecesY = 8;          // ���� ���� ����

    [Header("Motion")]
    public float explodeRadius = 180f;  // ������� ����(anchoredPosition ������)
    public float rotateMax = 360f;      // ���� ȸ����(��)
    public float duration = 0.8f;       // ���� ���� �ð�(���̵�ƿ�)

    [Header("Randomness")]
    public float upBias = 0.3f;         // �������� ��¦ �� Ƣ��(0~1)

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
        if (src.sprite == null) yield break;
        if (piecesX < 1) piecesX = 1;
        if (piecesY < 1) piecesY = 1;

        // ���� ����� ���� ���� ũ��/��������Ʈ ����
        Sprite sprite = src.sprite;
        Texture2D tex = sprite.texture;
        Rect spriteRect = sprite.textureRect; // �ؽ�ó �� ����(px)

        // ���� ����
        src.enabled = false;

        // ������ ���� ��ġ(�θ� ĵ���� ��ǥ ����)
        Vector2 baseAnchored = srcRT.anchoredPosition;
        Vector2 size = srcRT.rect.size;

        // �� ������ UI ũ��
        Vector2 pieceSize = new Vector2(size.x / piecesX, size.y / piecesY);

        // ��������Ʈ���� �߶�� �ȼ� ũ��
        float rectW = spriteRect.width / piecesX;
        float rectH = spriteRect.height / piecesY;

        // ���� ����
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
                // ���� ������ �ش� ���� ��ġ�� ��ġ
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
                img.raycastTarget = false; // Ŭ�� ����

                var cg = go.GetComponent<CanvasGroup>();
                cg.alpha = 1f;

                // ���� �̵�/ȸ�� Ÿ�� ���
                Vector2 dir = (Random.insideUnitCircle + Vector2.up * upBias).normalized;
                float dist = Random.Range(explodeRadius * 0.4f, explodeRadius);
                Vector2 targetPos = rt.anchoredPosition + dir * dist;
                float rotZ = Random.Range(-rotateMax, rotateMax);

                // ���� ���� �ִϸ��̼�
                StartCoroutine(AnimatePiece(rt, cg, targetPos, rotZ));
            }
        }

        // ��ü ���� �ð���ŭ ��ٸ� �� ����
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
            // ��¦ ƨ��� ��¡
            float moveK = 1f - Mathf.Pow(1f - k, 2f);
            rt.anchoredPosition = Vector2.LerpUnclamped(startPos, targetAnchoredPos, moveK);
            rt.localRotation = Quaternion.Slerp(startRot, endRot, k);
            cg.alpha = 1f - k;
            yield return null;
        }
        cg.alpha = 0f;
    }
}
