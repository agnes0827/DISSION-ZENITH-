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
//        // 키 입력 처리
//        float moveX = Input.GetAxisRaw("Horizontal");
//        float moveY = Input.GetAxisRaw("Vertical");
//        moveInput = new Vector2(moveX, moveY).normalized;

//        // 이동 여부에 따라 파라미터 설정
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

//        // 마지막 바라본 방향은 항상 설정
//        animator.SetFloat("LookX", lastMoveDir.x);
//        animator.SetFloat("LookY", lastMoveDir.y);
//    }

//    private void FixedUpdate()
//    {
//        // 실제 이동 처리
//        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
//    }

//    private void LateUpdate()
//    {
//        // y 좌표에 따라 렌더 순서 결정
//        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        Debug.Log("도서관");
//        SceneManager.LoadScene("Library");
//    }
//}