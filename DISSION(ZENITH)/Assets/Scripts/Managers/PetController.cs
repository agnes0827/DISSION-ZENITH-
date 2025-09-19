using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PetController : MonoBehaviour
{
    public GameObject player;
    public float speed = 5f;
    public float followDistance = 1.5f;

    private Vector3 lastPlayerPosition;
    private Vector3 lastMoveDirection;



    void Start()
    {
        lastPlayerPosition = player.transform.position;
    }

    void Update()
    {
        // 1. 이동 방향 계산 (XY 평면에서만 사용)
        Vector3 moveDir = player.transform.position - lastPlayerPosition;

        // 이동이 있다면 방향 저장
        if (moveDir.magnitude > 0.01f)
        {
            lastMoveDirection = moveDir.normalized;
        }

        lastPlayerPosition = player.transform.position;

        // 2. 플레이어 다리 높이 계산 (Y축이 높이일 경우)
        float playerLegY = player.transform.position.y - (player.transform.localScale.y / 2f);

        // 3. 따라갈 위치 계산 (화면 상 뒤쪽으로 떨어지게 해야 함)
        // => lastMoveDirection 기준으로 반대 방향
        Vector3 targetPosition;
        if (Mathf.Abs(lastMoveDirection.y) > Mathf.Abs(lastMoveDirection.x))   // 상하 이동이 더 큰 경우 분기 추가
        {
            if (lastMoveDirection.y > 0)
            {
                // 위로 이동 → 펫은 아래쪽
                targetPosition = player.transform.position - Vector3.up * followDistance;
            }
            else
            {
                // 아래로 이동 → 펫은 위쪽
                targetPosition = player.transform.position + Vector3.up * followDistance;
            }
        }
        else   // 좌우 이동일 경우 기존 방식 유지
        {
            targetPosition = player.transform.position - lastMoveDirection * followDistance;
        }

        // Y축은 다리 높이에 맞추기 (펫은 땅에 붙이기)
        targetPosition.y = playerLegY;

        // 4. 펫 이동
        Vector3 direction = (targetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance > 0.05f)
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}
