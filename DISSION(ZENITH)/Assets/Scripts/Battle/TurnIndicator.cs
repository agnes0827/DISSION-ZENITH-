using UnityEngine;
using UnityEngine.UI;

public class TurnIndicator : MonoBehaviour
{
    [Header("설정 (픽셀 단위)")]
    public float floatSpeed = 5f;      // 애니메이션 속도
    public float floatHeight = 20f;    // 애니메이션 범위

    private Vector3 currentOffset;     // indicator 위치

    private Transform currentTarget;   // 따라다닐 타겟

    void Update()
    {
        if (currentTarget == null) return;

        // 타겟 위치 로드
        Vector3 targetPos = currentTarget.position;

        // 애니메이션 효과
        float bobbing = Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.position = targetPos + currentOffset + new Vector3(0, bobbing, 0);
    }

    public void SetTarget(Transform target, Vector3 offset)
    {
        currentTarget = target;
        currentOffset = offset;

        if (target != null) gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
}