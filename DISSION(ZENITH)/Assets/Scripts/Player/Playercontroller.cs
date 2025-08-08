//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class playercontroller : MonoBehaviour
//{
//    public float moveSpeed = 3f;

//    private Rigidbody2D rb;
//    private Animator animator;
//    private Vector2 moveInput;
//    private Vector2 lastMoveDir;

//    private SpriteRenderer spriteRenderer;

//    private void Start()
//    {
//        rb = GetComponent<Rigidbody2D>();
//        animator = GetComponent<Animator>();
//        spriteRenderer = GetComponent<SpriteRenderer>();
//    }

//    private void Update()
//    {
//        // Ű �Է� ó��
//        float moveX = Input.GetAxisRaw("Horizontal");
//        float moveY = Input.GetAxisRaw("Vertical");
//        moveInput = new Vector2(moveX, moveY).normalized;

//        // �̵� ���ο� ���� �Ķ���� ����
//        if (moveInput != Vector2.zero)
//        {
//            lastMoveDir = moveInput;

//            animator.SetBool("IsMoving", true);
//            animator.SetFloat("MoveX", moveInput.x);
//            animator.SetFloat("MoveY", moveInput.y);
//        }
//        else
//        {
//            animator.SetBool("IsMoving", false);
//        }

//        // ������ �ٶ� ������ �׻� ����
//        animator.SetFloat("LookX", lastMoveDir.x);
//        animator.SetFloat("LookY", lastMoveDir.y);
//    }

//    private void FixedUpdate()
//    {
//        // ���� �̵� ó��
//        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
//    }

//    private void LateUpdate()
//    {
//        // y ��ǥ�� ���� ���� ���� ����
//        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        Debug.Log("������");
//        SceneManager.LoadScene("Library");
//    }
//}