using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public string currentMapName; //�� �̸�

    public float speed = 3f;
    private Vector3 vector;
    public int walkCount;
    public float runSpeed;
    private float applyRunSpeed;
    private bool canMove = true;

    private CapsuleCollider2D capsuleColider;
    public LayerMask layermask; //�̵� �Ұ� ����

    Animator anim;

    private string lastDirection = "Front"; // �⺻ ����

    void Start()
    {
        // DontDestroyOnLoad(this.gameObject);
        anim = GetComponent<Animator>();
        capsuleColider = GetComponent<CapsuleCollider2D>();
    }

    IEnumerator MoveCoroutine()
    {
        //�̵�
        while (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                applyRunSpeed = runSpeed;
                //applyRunFlag = true;
            }
            else
            {
                applyRunSpeed = 0;
                //applyRunFlag = false;
            }

            vector.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);
            if (vector.x != 0)
                vector.y = 0;

            // ���� ����
            if (vector.y < 0) lastDirection = "Back";
            else if (vector.y > 0) lastDirection = "Front";
            else if (vector.x > 0) lastDirection = "Right";
            else if (vector.x < 0) lastDirection = "Left";
            
            //�ִϸ��̼� �Ķ����
            anim.SetFloat("InputX", vector.x);
            anim.SetFloat("InputY", vector.y);

            //���̾� ����ũ
            RaycastHit2D hit;
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(vector.x * speed * walkCount, vector.y * speed * walkCount);

            capsuleColider.enabled = false;
            hit = Physics2D.Linecast(start, end, layermask);
            capsuleColider.enabled = true;

            if (hit.transform != null)
            {
                break;

            }

            //������ �Ǵ�
            anim.SetBool("isMoving", true);


            transform.Translate(vector.x * (speed + applyRunSpeed) * Time.deltaTime,
                                 vector.y * (speed + applyRunSpeed) * Time.deltaTime,
                                 0);

            yield return null; // ���� �����ӱ��� ���
        }

        anim.SetBool("isMoving", false);
        canMove = true;
    }

    void Update()
    {
        if (canMove)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                canMove = false;
                StartCoroutine(MoveCoroutine());
            }
        }
    }

    public string GetDirection()
    {
        return lastDirection;
    }
}
