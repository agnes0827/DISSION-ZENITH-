using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetC : MonoBehaviour
{
    public GameObject player;
    public float speed = 5f;
    public float followDistance = 1.5f;
    public bool lockToGround = false; //  새 옵션: true면 펫 Y를 플레이어 다리 높이에 고정

    private Vector3 lastPlayerPosition;
    private Vector3 lastMoveDirection;
    private Animator playerAnim; //  플레이어 Animator 참조 추가

    void Start()
    {
        lastPlayerPosition = player.transform.position;
        playerAnim = player.GetComponent<Animator>(); //  Animator가 있으면 받아옴 (없으면 null)
    }

    void Update()
    {
        // 이동 방향(이전 코드와 동일)
        Vector3 moveDir = player.transform.position - lastPlayerPosition;
        if (moveDir.magnitude > 0.01f)
        {
            lastMoveDirection = moveDir.normalized;
        }
        lastPlayerPosition = player.transform.position;

        // 1) 플레이어가 "바라보는 방향"을 우선으로 사용
        Vector3 lookDir = Vector3.zero;
        

        // 2) 바라보는 방향이 없으면(예: 애니메이터 없음 또는 파라미터 0) 이동 방향으로 폴백
        Vector3 dirToUse;
        if (lookDir.magnitude > 0.01f)
        {
            dirToUse = lookDir.normalized; // "플레이어가 보는 방향"
        }
        else
        {
            dirToUse = lastMoveDirection;   //  폴백: 이동 방향
        }

        //  3) target은 "플레이어 뒤" -> 플레이어가 보는 방향의 반대
        // 따라갈 위치 계산
        Vector3 targetPosition;

        // 이동 방향이 위/아래인 경우
        if (Mathf.Abs(lastMoveDirection.y) > Mathf.Abs(lastMoveDirection.x))
        {
            if (lastMoveDirection.y > 0)
            {
                // 플레이어가 위로 이동 → 펫은 아래쪽
                targetPosition = player.transform.position - Vector3.up * followDistance;
            }
            else
            {
                // 플레이어가 아래로 이동 → 펫은 위쪽
                targetPosition = player.transform.position + Vector3.up * followDistance;
            }
        }
        else
        {
            // 좌우 이동은 기존 방식 유지
            targetPosition = player.transform.position - lastMoveDirection * followDistance;
        }


        //  4) 필요하면 Y를 플레이어 다리 높이에 고정 (side-scroller 등에서 사용)
        if (lockToGround)
        {
            float playerLegY = player.transform.position.y - (player.transform.localScale.y / 2f);
            targetPosition.y = playerLegY;
        }

        // 이동 처리(이전과 동일)
        Vector3 direction = (targetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance > 0.05f)
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}
