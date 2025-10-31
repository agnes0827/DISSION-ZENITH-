using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetC : MonoBehaviour
{
    public GameObject player;
    public float smoothTime = 0.05f;       // 부드러운 따라오기 속도
    public float tileSize = 1f;           // 격자 크기
    public float maxDistance = 3f;        // 순간이동 트리거 거리
    public int teleportOffset = 2;        // 순간이동 시 플레이어 뒤쪽 거리 (타일 기준)
    public float yOffset = 0.2f;          // 순간이동 시 Y축 오프셋
    public float followDistance = 1f;     // 이동 시 유지할 거리

    private Queue<Vector3Int> playerHistory = new Queue<Vector3Int>();
    private Vector3Int lastPlayerGridPos;
    private Vector3 targetPetWorldPos;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        lastPlayerGridPos = WorldToGrid(player.transform.position);
        targetPetWorldPos = transform.position;
    }

    void Update()
    {
        Vector3Int currentPlayerGrid = WorldToGrid(player.transform.position);

        // 플레이어가 새로운 칸으로 이동했을 때 기록
        if (currentPlayerGrid != lastPlayerGridPos)
        {
            playerHistory.Enqueue(lastPlayerGridPos);
            lastPlayerGridPos = currentPlayerGrid;
        }

        bool teleported = false;

        // 플레이어와 펫 거리 체크 (순간이동)
        if (Vector3.Distance(transform.position, player.transform.position) > maxDistance)
        {
            Vector3Int moveDir = currentPlayerGrid - lastPlayerGridPos;
            if (moveDir == Vector3Int.zero)
            {
                moveDir = Vector3Int.up;
            }

            Vector3Int behindPos = currentPlayerGrid - moveDir * teleportOffset;
            Vector3 behindWorld = GridToWorld(behindPos);

            // 플레이어 위치 기준 계산
            float playerCenterY = player.transform.position.y;
            float playerHeadY = player.transform.position.y + (player.transform.localScale.y / 2f);
            float playerLegY = player.transform.position.y - (player.transform.localScale.y / 2f);

            // 방향별 위치 보정
            if (Mathf.Abs(moveDir.y) > Mathf.Abs(moveDir.x))
            {
                if (moveDir.y > 0)
                {
                    // 위로 이동 → 중심과 머리 사이보다 조금 더 아래
                    behindWorld.y = (playerCenterY * 2f + playerHeadY) / 3f + followDistance * 0.1f;
                }
                else
                {
                    // 아래로 이동 → 발 밑
                    behindWorld.y = playerLegY + yOffset;
                }
            }
            else
            {
                // 좌우 이동 → 중심보다 살짝 위
                behindWorld.y = playerCenterY + followDistance * 0.1f;
            }

            targetPetWorldPos = behindWorld;
            transform.position = targetPetWorldPos;
            velocity = Vector3.zero;
            playerHistory.Clear();

            teleported = true;
        }

        // 펫이 목표에 거의 도달했으면 다음 목표로
        if (Vector3.Distance(transform.position, targetPetWorldPos) < 0.05f && playerHistory.Count > 0)
        {
            Vector3Int nextGrid = playerHistory.Dequeue();
            targetPetWorldPos = GridToWorld(nextGrid);
        }

        // SmoothDamp 이동 (순간이동하지 않았을 때만)
        if (!teleported)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPetWorldPos,
                ref velocity,
                smoothTime
            );
        }
    }

    Vector3Int WorldToGrid(Vector3 worldPos)
    {
        return new Vector3Int(
            Mathf.RoundToInt(worldPos.x / tileSize),
            Mathf.RoundToInt(worldPos.y / tileSize),
            0
        );
    }

    Vector3 GridToWorld(Vector3Int gridPos)
    {
        return new Vector3(
            gridPos.x * tileSize,
            gridPos.y * tileSize,
            0f
        );
    }
}
