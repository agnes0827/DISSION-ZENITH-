using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetC : MonoBehaviour
{
    public GameObject player;
    public float speed = 5f;
    public float followDistance = 1.5f;
    public bool lockToGround = false; //  �� �ɼ�: true�� �� Y�� �÷��̾� �ٸ� ���̿� ����

    private Vector3 lastPlayerPosition;
    private Vector3 lastMoveDirection;
    private Animator playerAnim; //  �÷��̾� Animator ���� �߰�

    void Start()
    {
        lastPlayerPosition = player.transform.position;
        playerAnim = player.GetComponent<Animator>(); //  Animator�� ������ �޾ƿ� (������ null)
    }

    void Update()
    {
        // �̵� ����(���� �ڵ�� ����)
        Vector3 moveDir = player.transform.position - lastPlayerPosition;
        if (moveDir.magnitude > 0.01f)
        {
            lastMoveDirection = moveDir.normalized;
        }
        lastPlayerPosition = player.transform.position;

        // 1) �÷��̾ "�ٶ󺸴� ����"�� �켱���� ���
        Vector3 lookDir = Vector3.zero;
        

        // 2) �ٶ󺸴� ������ ������(��: �ִϸ����� ���� �Ǵ� �Ķ���� 0) �̵� �������� ����
        Vector3 dirToUse;
        if (lookDir.magnitude > 0.01f)
        {
            dirToUse = lookDir.normalized; // "�÷��̾ ���� ����"
        }
        else
        {
            dirToUse = lastMoveDirection;   //  ����: �̵� ����
        }

        //  3) target�� "�÷��̾� ��" -> �÷��̾ ���� ������ �ݴ�
        // ���� ��ġ ���
        Vector3 targetPosition;

        // �̵� ������ ��/�Ʒ��� ���
        if (Mathf.Abs(lastMoveDirection.y) > Mathf.Abs(lastMoveDirection.x))
        {
            if (lastMoveDirection.y > 0)
            {
                // �÷��̾ ���� �̵� �� ���� �Ʒ���
                targetPosition = player.transform.position - Vector3.up * followDistance;
            }
            else
            {
                // �÷��̾ �Ʒ��� �̵� �� ���� ����
                targetPosition = player.transform.position + Vector3.up * followDistance;
            }
        }
        else
        {
            // �¿� �̵��� ���� ��� ����
            targetPosition = player.transform.position - lastMoveDirection * followDistance;
        }


        //  4) �ʿ��ϸ� Y�� �÷��̾� �ٸ� ���̿� ���� (side-scroller ��� ���)
        if (lockToGround)
        {
            float playerLegY = player.transform.position.y - (player.transform.localScale.y / 2f);
            targetPosition.y = playerLegY;
        }

        // �̵� ó��(������ ����)
        Vector3 direction = (targetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance > 0.05f)
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}
