//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//public class PetC : MonoBehaviour
//{
//    public GameObject player;
//    public float smoothTime = 0.2f;       // 펫 부드럽게 따라오기 속도
//    public float tileSize = 1f;           // 격자 크기
//    public float maxDistance = 3f;        // 일정 거리 이상 떨어지면 순간이동
//    public int teleportOffset = 2;        // 순간이동 시 플레이어 뒤쪽 거리 (타일 기준)
//    public float yOffset = 0.2f;          // 순간이동 시 Y축 오프셋 (발 아래 겹침 방지)
//    public float followDistance = 1f;     //펫과 플레이어 간의 거리

//    private Queue<Vector3Int> playerHistory = new Queue<Vector3Int>();
//    private Vector3Int lastPlayerGridPos;
//    private Vector3 targetPetWorldPos;
//    private Vector3 velocity = Vector3.zero;

//    void Start()
//    {
//        lastPlayerGridPos = WorldToGrid(player.transform.position);
//        targetPetWorldPos = transform.position;
//    }

//    void Update()
//    {
//        Vector3Int currentPlayerGrid = WorldToGrid(player.transform.position);

//        // 플레이어가 새로운 칸으로 이동했을 때 기록
//        if (currentPlayerGrid != lastPlayerGridPos)
//        {
//            playerHistory.Enqueue(lastPlayerGridPos);
//            lastPlayerGridPos = currentPlayerGrid;
//        }

//        if (Vector3.Distance(transform.position, player.transform.position) > maxDistance)
//        {
//            Vector3Int moveDir = currentPlayerGrid - lastPlayerGridPos;
//            if (moveDir == Vector3Int.zero)
//            {
//                moveDir = Vector3Int.up;
//            }

//            Vector3Int behindPos = currentPlayerGrid - moveDir * teleportOffset;
//            Vector3 behindWorld = GridToWorld(behindPos);

//            //  시작: 위로 이동 시 펫과 일정 거리 유지, 좌우 이동 시 한 칸 아래
//            if (Mathf.Abs(moveDir.y) > Mathf.Abs(moveDir.x))
//            {
//                float playerLegY = player.transform.position.y - (player.transform.localScale.y / 2f); // 다리 위치

//                if (Mathf.Abs(moveDir.y) > Mathf.Abs(moveDir.x))
//                {
//                    // 세로 이동
//                    if (moveDir.y > 0)
//                    {
//                        // 플레이어 위로 이동 → 펫 발 아래에서 followDistance 유지
//                        behindWorld.y = playerLegY - followDistance;
//                    }
//                    else
//                    {
//                        // 플레이어 아래로 이동 → 발 아래 살짝 yOffset
//                        behindWorld.y = playerLegY + yOffset;
//                    }
//                }
//                else
//                {
//                    // 좌우 이동 → 발 아래 한 칸(tileSize) 떨어진 위치
//                    behindWorld.y = playerLegY - tileSize;
//                }

//            }


//            targetPetWorldPos = behindWorld;
//            transform.position = targetPetWorldPos;
//            playerHistory.Clear();
//        }

//        // 펫이 목표에 거의 도달했으면 다음 목표로
//        if (Vector3.Distance(transform.position, targetPetWorldPos) < 0.05f && playerHistory.Count > 0)
//        {
//            Vector3Int nextGrid = playerHistory.Dequeue();
//            targetPetWorldPos = GridToWorld(nextGrid);
//        }

//        // SmoothDamp로 부드럽게 이동
//        transform.position = Vector3.SmoothDamp(
//            transform.position,
//            targetPetWorldPos,
//            ref velocity,
//            smoothTime
//        );
//    }

//    // 월드 → 격자 좌표
//    Vector3Int WorldToGrid(Vector3 worldPos)
//    {
//        return new Vector3Int(
//            Mathf.RoundToInt(worldPos.x / tileSize),
//            Mathf.RoundToInt(worldPos.y / tileSize),
//            0
//        );
//    }

//    // 격자 → 월드 좌표
//    Vector3 GridToWorld(Vector3Int gridPos)
//    {
//        return new Vector3(
//            gridPos.x * tileSize,
//            gridPos.y * tileSize,
//            0f
//        );
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetC : MonoBehaviour
{
    public GameObject player;
    public float smoothTime = 0.2f;       // 부드러운 따라오기 속도
    public float tileSize = 1f;           // 격자 크기
    public float maxDistance = 3f;        // 순간이동 트리거 거리
    public int teleportOffset = 2;        // 순간이동 시 플레이어 뒤쪽 거리 (타일 기준)
    public float yOffset = 0.2f;          // 순간이동 시 Y축 오프셋

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

        //  플레이어와 펫 거리 체크 (순간이동)
        if (Vector3.Distance(transform.position, player.transform.position) > maxDistance)
        {
            Vector3Int moveDir = currentPlayerGrid - lastPlayerGridPos;
            if (moveDir == Vector3Int.zero)
            {
                moveDir = Vector3Int.up;
            }

            Vector3Int behindPos = currentPlayerGrid - moveDir * teleportOffset;
            Vector3 behindWorld = GridToWorld(behindPos);

            //  세로 이동 시 플레이어와 일정 거리 유지
            if (Mathf.Abs(moveDir.y) > Mathf.Abs(moveDir.x))
            {
                behindWorld.y = player.transform.position.y; 
            }
            else
            {
                // 좌우 이동 → 플레이어보다 한 칸 아래
                behindWorld.y = player.transform.position.y - tileSize;
            }

            targetPetWorldPos = behindWorld;
            transform.position = targetPetWorldPos;
            velocity = Vector3.zero;
            playerHistory.Clear();
            
        }

        // 펫이 목표에 거의 도달했으면 다음 목표로
        if (Vector3.Distance(transform.position, targetPetWorldPos) < 0.05f && playerHistory.Count > 0)
        {
            Vector3Int nextGrid = playerHistory.Dequeue();
            targetPetWorldPos = GridToWorld(nextGrid);
        }

        // SmoothDamp로 부드럽게 이동
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPetWorldPos,
            ref velocity,
            smoothTime
        );
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
