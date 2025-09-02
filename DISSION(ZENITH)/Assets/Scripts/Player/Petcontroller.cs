using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Petcontroller : MonoBehaviour
{
    public GameObject player;
    public float speed = 5f;
    public float followDistance = 1.5f;

    private Vector3 lastPlayerPosition;
    private Vector3 lastMoveDirection;
    private SpriteRenderer sr;         // 펫의 SpriteRenderer
    private SpriteRenderer playerSR;   // 플레이어의 SpriteRenderer

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        lastPlayerPosition = player.transform.position;

        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null) Debug.LogWarning("Petcontroller: SpriteRenderer (pet) not found on " + gameObject.name);

        if (player != null)
        {
            playerSR = player.GetComponent<SpriteRenderer>();
            if (playerSR == null) playerSR = player.GetComponentInChildren<SpriteRenderer>();
            if (playerSR == null) Debug.LogWarning("Petcontroller: SpriteRenderer (player) not found on " + player.name);
        }
        else
        {
            Debug.LogError("Petcontroller: player reference is null on " + gameObject.name);
        }
    }

    void Update()
    {
        // 기존 코드: 이동 방향 계산, lastMoveDirection 업데이트 등은 그대로 유지
        Vector3 moveDir = player.transform.position - lastPlayerPosition;

        if (moveDir.magnitude > 0.01f)
        {
            lastMoveDirection = moveDir.normalized;
        }

        lastPlayerPosition = player.transform.position;

        float playerLegY = player.transform.position.y - (player.transform.localScale.y / 2f);

        // --- 플레이어 방향에 따라 펫 위치 조절하기 ---
        string playerDir = "Front";
        PlayerController1 pc = player.GetComponent<PlayerController1>();
        if (pc != null)
            playerDir = pc.GetDirection();

        Vector3 offset = Vector3.zero;
        float sideDistance = followDistance;
        float backDistance = followDistance * 1.5f;

        switch (playerDir)
        {
            case "Front":    // 플레이어 아래 보는 중 → 펫은 뒤 (위쪽)
                offset = new Vector3(0, backDistance, 0);
                break;
            case "Back":     // 플레이어 위 보는 중 → 펫은 뒤 (아래쪽)
                offset = new Vector3(0, -backDistance, 0);
                break;
            case "Right":    // 플레이어 오른쪽 → 펫은 왼쪽
                offset = new Vector3(-sideDistance, 0, 0);
                break;
            case "Left":     // 플레이어 왼쪽 → 펫은 오른쪽
                offset = new Vector3(sideDistance, 0, 0);
                break;
            default:
                offset = -lastMoveDirection * followDistance;
                break;
        }

        Vector3 targetPosition = player.transform.position + offset;

        // Y축은 플레이어 다리 높이에 맞춤
        targetPosition.y = playerLegY;

        // 부드럽게 펫 이동
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
        }
    }

    void LateUpdate()
    {
        string dir = "Front"; // 기본값
        if (player != null)
        {
            PlayerController1 pc = player.GetComponent<PlayerController1>();
            if (pc != null)
            {
                dir = pc.GetDirection();
            }
            else
            {
                // [ADDED] 플레이어에 PlayerController1이 없을 때의 폴백: 최근 위치 변화로 방향 추정
                Vector3 playerVel = player.transform.position - lastPlayerPosition;
                if (Mathf.Abs(playerVel.y) > Mathf.Abs(playerVel.x))
                {
                    dir = (playerVel.y < 0) ? "Front" : "Back";
                }
                else if (Mathf.Abs(playerVel.x) > 0.01f)
                {
                    dir = (playerVel.x > 0) ? "Right" : "Left";
                }
            }
        }

        if (dir == "Front")
        {
            //플레이어가 아래를 보고 있으면 펫이 뒤에 있어야 함
            if (sr != null && playerSR != null)
                sr.sortingOrder = playerSR.sortingOrder + 1;
        }
        else if (dir == "Back")
        {
            //플레이어가 위를 보고 있으면 펫이 앞에 있어야 함
            if (sr != null && playerSR != null)
                sr.sortingOrder = playerSR.sortingOrder - 1;
        }
        else
        {
            //좌우 방향에서는 Y 위치 기준
            if (sr != null)
                sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
        }
    }
}