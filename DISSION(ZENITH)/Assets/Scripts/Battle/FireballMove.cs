using UnityEngine;
using System.Collections;

public class FireballMove : MonoBehaviour
{
    public float speed = 10f;   // 날아가는 속도
    private Vector3 targetPos;  // 목표 위치
    private System.Action onHitCallback; // 맞았을 때 실행할 함수

    public void Setup(Vector3 target, System.Action onHit)
    {
        targetPos = target;
        onHitCallback = onHit;
        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            // 목표 방향으로 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        // 도착하면
        transform.position = targetPos;

        // 타격 효과 실행
        if (onHitCallback != null) onHitCallback.Invoke();

        // 파이어볼 삭제
        Destroy(gameObject);
    }
}