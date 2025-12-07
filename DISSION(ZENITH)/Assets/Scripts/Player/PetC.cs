using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetC : MonoBehaviour
{
    public float speed = 3f;

    // 딜레이 시간
    public float delaySeconds = 1f;
    public float sampleInterval = 0.05f;

    private Animator anim;
    private Queue<Vector2> inputQueue = new Queue<Vector2>();

    private Vector2 currentDirection = Vector2.zero;
    private Vector2 lastMove = Vector2.down;

    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(InputRecorder());
        StartCoroutine(InputConsumer());
    }

    // 입력 기록
    IEnumerator InputRecorder()
    {
        while (true)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector2 input = Vector2.zero;

            if (h != 0 || v != 0)
            {
                if (h != 0) v = 0;
                input = new Vector2(h, v);
            }

            inputQueue.Enqueue(input);

            yield return new WaitForSeconds(sampleInterval);
        }
    }

    // 입력 소비 (1초 뒤 따라하기)
    IEnumerator InputConsumer()
    {
        yield return new WaitForSeconds(delaySeconds);

        while (true)
        {
            if (inputQueue.Count > 0)
            {
                currentDirection = inputQueue.Dequeue();

                if (currentDirection != Vector2.zero)
                    lastMove = currentDirection;
            }
            else
            {
                currentDirection = Vector2.zero;
            }

            yield return new WaitForSeconds(sampleInterval);
        }
    }

    // 실제 이동
    void Update()
    {
        if (currentDirection != Vector2.zero)
        {
            transform.Translate(currentDirection * speed * Time.deltaTime);

            if (anim != null)
            {
                anim.SetBool("isMoving", true);
                anim.SetFloat("inputX", currentDirection.x);
                anim.SetFloat("inputY", currentDirection.y);
            }
        }
        else
        {
            if (anim != null)
            {
                anim.SetBool("isMoving", false);
                anim.SetFloat("inputX", lastMove.x);
                anim.SetFloat("inputY", lastMove.y);
            }
        }
    }
}
