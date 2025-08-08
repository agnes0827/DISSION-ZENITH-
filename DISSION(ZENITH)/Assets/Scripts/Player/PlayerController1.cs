using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController1 : MonoBehaviour
{
    public float speed = 3f;
    private Vector3 vector;
    public float runSpeed;
    private float applyRunSpeed;
    //private bool applyRunFlag = false;
    private bool canMove = true;

    Animator anim;

    private string lastDirection = "Front"; // 기본 방향

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    IEnumerator MoveCoroutine()
    {
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

            // 방향 갱신
            if (vector.y < 0) lastDirection = "Back";
            else if (vector.y > 0) lastDirection = "Front";
            else if (vector.x > 0) lastDirection = "Right";
            else if (vector.x < 0) lastDirection = "Left";

            anim.SetFloat("InputX", vector.x);
            anim.SetFloat("InputY", vector.y);
            anim.SetBool("isMoving", true);

            // **중첩 while 제거, 프레임 단위 이동**
            transform.Translate(vector.x * (speed + applyRunSpeed) * Time.deltaTime,
                                 vector.y * (speed + applyRunSpeed) * Time.deltaTime,
                                 0);

            yield return null; // 다음 프레임까지 대기
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
