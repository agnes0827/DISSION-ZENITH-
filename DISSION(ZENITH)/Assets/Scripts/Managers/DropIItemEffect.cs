using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropIItemEffect : MonoBehaviour
{
    [Header("Drop Motion")]
    public float dropHeight = 1.5f;     // 위에서 얼마나 떨어질지(월드 유닛)
    public float dropDuration = 0.35f;    // 떨어지는 시간
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Pickup")]
    public Collider2D pickupTrigger;      // 착지 후 활성화할 트리거(없으면 자동 탐색)
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
        // 시작/끝 지점
        Vector3 end = transform.position;
        Vector3 start = end + Vector3.up * dropHeight;

        // 시작 위치로 이동
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

        // 착지 후 집기 가능
        if (disableTriggerUntilLand && pickupTrigger) pickupTrigger.enabled = true;
    }
}
