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
        // 1. �̵� ���� ��� (XY ��鿡���� ���)
        Vector3 moveDir = player.transform.position - lastPlayerPosition;

        // �̵��� �ִٸ� ���� ����
        if (moveDir.magnitude > 0.01f)
        {
            lastMoveDirection = moveDir.normalized;
        }

        lastPlayerPosition = player.transform.position;

        // 2. �÷��̾� �ٸ� ���� ��� (Y���� ������ ���)
        float playerLegY = player.transform.position.y - (player.transform.localScale.y / 2f);

        // 3. ���� ��ġ ��� (ȭ�� �� �������� �������� �ؾ� ��)
        // => lastMoveDirection �������� �ݴ� ����
        Vector3 targetPosition;
        if (Mathf.Abs(lastMoveDirection.y) > Mathf.Abs(lastMoveDirection.x))   // ���� �̵��� �� ū ��� �б� �߰�
        {
            if (lastMoveDirection.y > 0)
            {
                // ���� �̵� �� ���� �Ʒ���
                targetPosition = player.transform.position - Vector3.up * followDistance;
            }
            else
            {
                // �Ʒ��� �̵� �� ���� ����
                targetPosition = player.transform.position + Vector3.up * followDistance;
            }
        }
        else   // �¿� �̵��� ��� ���� ��� ����
        {
            targetPosition = player.transform.position - lastMoveDirection * followDistance;
        }

        // Y���� �ٸ� ���̿� ���߱� (���� ���� ���̱�)
        targetPosition.y = playerLegY;

        // 4. �� �̵�
        Vector3 direction = (targetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance > 0.05f)
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}
