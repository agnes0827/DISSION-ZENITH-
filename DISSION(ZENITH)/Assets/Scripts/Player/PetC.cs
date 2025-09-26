//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//public class PetC : MonoBehaviour
//{
//    public GameObject player;
//    public float smoothTime = 0.2f;       // �� �ε巴�� ������� �ӵ�
//    public float tileSize = 1f;           // ���� ũ��
//    public float maxDistance = 3f;        // ���� �Ÿ� �̻� �������� �����̵�
//    public int teleportOffset = 2;        // �����̵� �� �÷��̾� ���� �Ÿ� (Ÿ�� ����)
//    public float yOffset = 0.2f;          // �����̵� �� Y�� ������ (�� �Ʒ� ��ħ ����)
//    public float followDistance = 1f;     //��� �÷��̾� ���� �Ÿ�

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

//        // �÷��̾ ���ο� ĭ���� �̵����� �� ���
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

//            //  ����: ���� �̵� �� ��� ���� �Ÿ� ����, �¿� �̵� �� �� ĭ �Ʒ�
//            if (Mathf.Abs(moveDir.y) > Mathf.Abs(moveDir.x))
//            {
//                float playerLegY = player.transform.position.y - (player.transform.localScale.y / 2f); // �ٸ� ��ġ

//                if (Mathf.Abs(moveDir.y) > Mathf.Abs(moveDir.x))
//                {
//                    // ���� �̵�
//                    if (moveDir.y > 0)
//                    {
//                        // �÷��̾� ���� �̵� �� �� �� �Ʒ����� followDistance ����
//                        behindWorld.y = playerLegY - followDistance;
//                    }
//                    else
//                    {
//                        // �÷��̾� �Ʒ��� �̵� �� �� �Ʒ� ��¦ yOffset
//                        behindWorld.y = playerLegY + yOffset;
//                    }
//                }
//                else
//                {
//                    // �¿� �̵� �� �� �Ʒ� �� ĭ(tileSize) ������ ��ġ
//                    behindWorld.y = playerLegY - tileSize;
//                }

//            }


//            targetPetWorldPos = behindWorld;
//            transform.position = targetPetWorldPos;
//            playerHistory.Clear();
//        }

//        // ���� ��ǥ�� ���� ���������� ���� ��ǥ��
//        if (Vector3.Distance(transform.position, targetPetWorldPos) < 0.05f && playerHistory.Count > 0)
//        {
//            Vector3Int nextGrid = playerHistory.Dequeue();
//            targetPetWorldPos = GridToWorld(nextGrid);
//        }

//        // SmoothDamp�� �ε巴�� �̵�
//        transform.position = Vector3.SmoothDamp(
//            transform.position,
//            targetPetWorldPos,
//            ref velocity,
//            smoothTime
//        );
//    }

//    // ���� �� ���� ��ǥ
//    Vector3Int WorldToGrid(Vector3 worldPos)
//    {
//        return new Vector3Int(
//            Mathf.RoundToInt(worldPos.x / tileSize),
//            Mathf.RoundToInt(worldPos.y / tileSize),
//            0
//        );
//    }

//    // ���� �� ���� ��ǥ
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
    public float smoothTime = 0.2f;       // �ε巯�� ������� �ӵ�
    public float tileSize = 1f;           // ���� ũ��
    public float maxDistance = 3f;        // �����̵� Ʈ���� �Ÿ�
    public int teleportOffset = 2;        // �����̵� �� �÷��̾� ���� �Ÿ� (Ÿ�� ����)
    public float yOffset = 0.2f;          // �����̵� �� Y�� ������

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

        // �÷��̾ ���ο� ĭ���� �̵����� �� ���
        if (currentPlayerGrid != lastPlayerGridPos)
        {
            playerHistory.Enqueue(lastPlayerGridPos);
            lastPlayerGridPos = currentPlayerGrid;
        }

        //  �÷��̾�� �� �Ÿ� üũ (�����̵�)
        if (Vector3.Distance(transform.position, player.transform.position) > maxDistance)
        {
            Vector3Int moveDir = currentPlayerGrid - lastPlayerGridPos;
            if (moveDir == Vector3Int.zero)
            {
                moveDir = Vector3Int.up;
            }

            Vector3Int behindPos = currentPlayerGrid - moveDir * teleportOffset;
            Vector3 behindWorld = GridToWorld(behindPos);

            //  ���� �̵� �� �÷��̾�� ���� �Ÿ� ����
            if (Mathf.Abs(moveDir.y) > Mathf.Abs(moveDir.x))
            {
                behindWorld.y = player.transform.position.y; 
            }
            else
            {
                // �¿� �̵� �� �÷��̾�� �� ĭ �Ʒ�
                behindWorld.y = player.transform.position.y - tileSize;
            }

            targetPetWorldPos = behindWorld;
            transform.position = targetPetWorldPos;
            velocity = Vector3.zero;
            playerHistory.Clear();
            
        }

        // ���� ��ǥ�� ���� ���������� ���� ��ǥ��
        if (Vector3.Distance(transform.position, targetPetWorldPos) < 0.05f && playerHistory.Count > 0)
        {
            Vector3Int nextGrid = playerHistory.Dequeue();
            targetPetWorldPos = GridToWorld(nextGrid);
        }

        // SmoothDamp�� �ε巴�� �̵�
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
