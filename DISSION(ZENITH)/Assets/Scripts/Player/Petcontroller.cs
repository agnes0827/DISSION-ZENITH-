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
    private SpriteRenderer sr;         // ���� SpriteRenderer
    private SpriteRenderer playerSR;   // �÷��̾��� SpriteRenderer

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
        // ���� �ڵ�: �̵� ���� ���, lastMoveDirection ������Ʈ ���� �״�� ����
        Vector3 moveDir = player.transform.position - lastPlayerPosition;

        if (moveDir.magnitude > 0.01f)
        {
            lastMoveDirection = moveDir.normalized;
        }

        lastPlayerPosition = player.transform.position;

        float playerLegY = player.transform.position.y - (player.transform.localScale.y / 2f);

        // --- �÷��̾� ���⿡ ���� �� ��ġ �����ϱ� ---
        string playerDir = "Front";
        PlayerController1 pc = player.GetComponent<PlayerController1>();
        if (pc != null)
            playerDir = pc.GetDirection();

        Vector3 offset = Vector3.zero;
        float sideDistance = followDistance;
        float backDistance = followDistance * 1.5f;

        switch (playerDir)
        {
            case "Front":    // �÷��̾� �Ʒ� ���� �� �� ���� �� (����)
                offset = new Vector3(0, backDistance, 0);
                break;
            case "Back":     // �÷��̾� �� ���� �� �� ���� �� (�Ʒ���)
                offset = new Vector3(0, -backDistance, 0);
                break;
            case "Right":    // �÷��̾� ������ �� ���� ����
                offset = new Vector3(-sideDistance, 0, 0);
                break;
            case "Left":     // �÷��̾� ���� �� ���� ������
                offset = new Vector3(sideDistance, 0, 0);
                break;
            default:
                offset = -lastMoveDirection * followDistance;
                break;
        }

        Vector3 targetPosition = player.transform.position + offset;

        // Y���� �÷��̾� �ٸ� ���̿� ����
        targetPosition.y = playerLegY;

        // �ε巴�� �� �̵�
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
        }
    }

    void LateUpdate()
    {
        string dir = "Front"; // �⺻��
        if (player != null)
        {
            PlayerController1 pc = player.GetComponent<PlayerController1>();
            if (pc != null)
            {
                dir = pc.GetDirection();
            }
            else
            {
                // [ADDED] �÷��̾ PlayerController1�� ���� ���� ����: �ֱ� ��ġ ��ȭ�� ���� ����
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
            //�÷��̾ �Ʒ��� ���� ������ ���� �ڿ� �־�� ��
            if (sr != null && playerSR != null)
                sr.sortingOrder = playerSR.sortingOrder + 1;
        }
        else if (dir == "Back")
        {
            //�÷��̾ ���� ���� ������ ���� �տ� �־�� ��
            if (sr != null && playerSR != null)
                sr.sortingOrder = playerSR.sortingOrder - 1;
        }
        else
        {
            //�¿� ���⿡���� Y ��ġ ����
            if (sr != null)
                sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
        }
    }
}