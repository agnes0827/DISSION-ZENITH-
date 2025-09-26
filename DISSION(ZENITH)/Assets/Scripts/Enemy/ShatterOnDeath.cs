using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatterOnDeath : MonoBehaviour
{
    [Header("Grid")]
    public int cols = 6;
    public int rows = 6;

    [Header("Physics")]
    public float explodeForce = 6f;      // ������ Ƣ�� ����
    public float randomForce = 2f;       // ���� �߰�
    public float torque = 180f;          // ȸ�� ��ũ(������)
    public float pieceGravity = 1f;

    [Header("VFX")]
    public float life = 0.8f;            // ������ ���������� �ð�(���̵� ����)
    public float fadeTime = 0.4f;        // ������ ���̵� �ð�
    public Material materialOverride;    // (����) ������ �ٸ� ��Ƽ���� �� ��

    private SpriteRenderer _sr;

    void Awake() => _sr = GetComponent<SpriteRenderer>();

    /// <summary>���� óġ�ϴ� ���� ȣ��</summary>
    public void Shatter()
    {
        if (_sr == null || _sr.sprite == null) { Destroy(gameObject); return; }

        var sprite = _sr.sprite;
        var tex = sprite.texture;
        var rect = sprite.rect;                  // px ����
        var ppu = sprite.pixelsPerUnit;

        float cellW = rect.width / cols;
        float cellH = rect.height / rows;

        // ���� ����
        _sr.enabled = false;

        // ������ ���� ����/���̾� ����
        int sortingOrder = _sr.sortingOrder;
        string sortingLayer = _sr.sortingLayerName;

        // ���� �߽� ���� ������ ���(����)
        Vector2 centerPx = new(rect.width / 2f, rect.height / 2f);

        for (int y = 0; y < rows; y++)
            for (int x = 0; x < cols; x++)
            {
                // ��������Ʈ ���� ����� (���� �ؽ�ó�� �Ϻκ� ����)
                var subRect = new Rect(
                    rect.x + x * cellW,
                    rect.y + (y * cellH),
                    cellW, cellH
                );
                var sub = Sprite.Create(tex, subRect, new Vector2(0.5f, 0.5f), ppu);

                // ���� ������Ʈ
                var go = new GameObject($"piece_{x}_{y}");
                go.transform.position = transform.position;
                go.transform.rotation = transform.rotation;

                var psr = go.AddComponent<SpriteRenderer>();
                psr.sprite = sub;
                psr.sortingLayerName = sortingLayer;
                psr.sortingOrder = sortingOrder;
                if (materialOverride != null) psr.material = materialOverride;

                // ������ ���� ������(�ȼ������� ��ȯ)
                Vector2 pieceCenterPx = new(
                    x * cellW + cellW / 2f,
                    y * cellH + cellH / 2f
                );
                Vector2 offsetPx = pieceCenterPx - centerPx;
                Vector3 localOffset = new(offsetPx.x / ppu, offsetPx.y / ppu, 0f);
                go.transform.position += localOffset;

                // ���� �ο�
                var rb = go.AddComponent<Rigidbody2D>();
                rb.gravityScale = pieceGravity;
                var col = go.AddComponent<BoxCollider2D>();
                col.size = new Vector2(cellW / ppu, cellH / ppu);

                // �ٱ���+���� �������� ����
                Vector2 dir = (localOffset).normalized;        // �߽ɿ��� �ٱ�
                if (dir.sqrMagnitude < 0.01f) dir = Random.insideUnitCircle.normalized;
                Vector2 impulse = dir * explodeForce + Random.insideUnitCircle * randomForce;
                rb.AddForce(impulse, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-torque, torque), ForceMode2D.Impulse);

                // ���� & ���̵�
                StartCoroutine(FadeAndDie(psr, rb, life, fadeTime));
            }

        // ���� ����
        Destroy(gameObject);
    }

    private IEnumerator FadeAndDie(SpriteRenderer sr, Rigidbody2D rb, float ttl, float fade)
    {
        // ��¦ ��Ƽ�ٰ�
        float t = 0f;
        while (t < ttl - fade)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // ���̵� �ƿ�
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
