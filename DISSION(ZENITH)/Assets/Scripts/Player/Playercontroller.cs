using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playercontroller : MonoBehaviour
{
    private Rigidbody2D rb;

    //플레이어 이동 속도
    public float moveSpeed = 3f;

    //스프라이트 이미지
    private SpriteRenderer spriteRenderer;

    //키 입력 방향 벡터 x,y값
    private Vector2 movement;

    //현재 바라보는 방향 Front, Back, Left, Right
    private string currentDirection = "Front";

    //걷기 모션 프레임 번호 0~3
    private int walkFrame = 0;
    //애니메이션 간 시간 측정을 위한 타이머
    private float animationTimer = 0f;
    //프레임이 바뀌는 시간(0.1초마다 전환)
    private float animationInterval = 0.1f; // 애니메이션 프레임 간격 (초)

    // 대화 출력 중 플레이어 이동 차단
    private DialogueManager dialogueManager;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //좌/우 방향키
        movement.x = Input.GetAxisRaw("Horizontal");
        //상/하 방향키
        movement.y = Input.GetAxisRaw("Vertical");

        //방향키가 눌렸을 때
        if (movement != Vector2.zero && !dialogueManager.isDialogue &&
            !(MiniGameManager.Instance != null && MiniGameManager.Instance.IsMiniGameActive))
        {
            MovePlayer(); //실제 이동 수행
            AnimateWalk();//걷는 애니메이션 재생
        }
        else
        {
            SetIdleSprite();//아무것도 안눌었을 때 가만히 서 있는 이미지
        }
    }

    //실제 이동 수행하는 함수
    void MovePlayer()
    {
        transform.Translate(movement.normalized * moveSpeed * Time.deltaTime);
        UpdateDirection(); //현재 바라보는 방향(currentDirection) 갱신
    }

    //현재 바라보는 방향 판별
    void UpdateDirection()
    {
        //x축 움직임 > y축 움직임 -> 좌/우 방향으로 판단
        if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
        {
            currentDirection = movement.x > 0 ? "Right" : "Left";
        }
        else
        {
            currentDirection = movement.y > 0 ? "Back" : "Front";
        }
    }

    //일정 시간(animationInterval)마다 스프라이트를 변경해 걷게 보이게 함
    void AnimateWalk()
    {
        animationTimer += Time.deltaTime;

        if (animationTimer >= animationInterval)
        {
            animationTimer = 0f;

            switch (walkFrame)
            {
                case 0:
                    SetSprite(currentDirection + "_Left");
                    break;
                case 1:
                    SetSprite(currentDirection + "_Idle");
                    break;
                case 2:
                    SetSprite(currentDirection + "_Right");
                    break;
                case 3:
                    SetSprite(currentDirection + "_Idle");
                    break;
            }

            walkFrame = (walkFrame + 1) % 4;
        }
    }

    //정지 상태일 때 애니메이션 초기화
    void SetIdleSprite()
    {
        walkFrame = 0;
        animationTimer = 0f;
        SetSprite(currentDirection + "_Idle");
    }

    //Resources/PlayerSprites/ 폴더에서 지정한 이름의 스프라이트를 로드
    void SetSprite(string spriteName)
    {
        Sprite sprite = Resources.Load<Sprite>("PlayerSprites/" + spriteName);
        if (sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
        else
        {
            Debug.LogWarning("스프라이트를 찾을 수 없습니다: " + spriteName);
            //해당 스프라이트가 없을 때 경고 메세지
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
