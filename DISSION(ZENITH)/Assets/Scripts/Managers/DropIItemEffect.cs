using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropIItemEffect : MonoBehaviour
{
    [Header("Drop Motion")]
    public float dropHeight = 1.5f;     // ������ �󸶳� ��������(���� ����)
    public float dropDuration = 0.35f;    // �������� �ð�
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Pickup")]
    public Collider2D pickupTrigger;      // ���� �� Ȱ��ȭ�� Ʈ����(������ �ڵ� Ž��)
    public bool disableTriggerUntilLand = true;

    void Awake()
    {
        if (pickupTrigger == null) pickupTrigger = GetComponent<Collider2D>();
        if (disableTriggerUntilLand && pickupTrigger) pickupTrigger.enabled = false;
    }

    void OnEnable()
    {
        StartCoroutine(Drop());
    }

    IEnumerator Drop()
    {
        // ����/�� ����
        Vector3 end = transform.position;
        Vector3 start = end + Vector3.up * dropHeight;

        // ���� ��ġ�� �̵�
        transform.position = start;

        float t = 0f;
        while (t < dropDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / dropDuration);
            float k = ease.Evaluate(p);
            transform.position = Vector3.Lerp(start, end, k);
            yield return null;
        }

        transform.position = end;

        // ���� �� ���� ����
        if (disableTriggerUntilLand && pickupTrigger) pickupTrigger.enabled = true;
    }
}
