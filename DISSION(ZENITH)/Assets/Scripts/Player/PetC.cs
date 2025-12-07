using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetC : MonoBehaviour
{
    public Transform player;

    public float smoothTime = 0.1f;    // 부드럽게 따라오기 시간
    public float minDistance = 0.5f;   // 플레이어와 유지할 최소 거리
    public float warpDistance = 4f;    // 순간이동 트리거 거리
    public float followOffset = 0.8f;  // 플레이어 반대쪽에 있을 때 거리

    private Vector3 velocity = Vector3.zero;
    private Animator anim;

    private Vector3 lastPlayerPos;
    private Vector3 lastMoveDir;       // 플레이어의 마지막 이동 방향 (반대 위치 계산용)
    private Vector3 lastMoveForAnim;   // 애니메이션용 마지막 방향 (Idle 시 유지)

    public static PetC Instance { get; private set; } // 싱글턴 + DontDestroyOnLoad 패턴

    private bool isSitting = false; // 앉음 상태 추가

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 이 오브젝트를 씬이 바뀌어도 계속 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

        anim = GetComponent<Animator>();

        // 먼저 PlayerController.Instance로 이전 씬에서 넘어온 플레이어 찾기
        if (PlayerController.Instance != null)
        {
            player = PlayerController.Instance.transform;
            lastPlayerPos = player.position;
        }
        else
        {
            Debug.LogError("PetFollow: PlayerController.Instance를 찾을 수 없습니다!");
            return;
        }
        lastMoveDir = Vector3.down;
        lastMoveForAnim = Vector3.down;

        // 씬 시작 시부터 계속 앉아 있는 상태로 세팅
        isSitting = true;
        anim.SetBool("isSit", true);
        anim.SetBool("isMoving", false);
    }

    void Update()
    {
        // G 키를 눌렀을 때 처음으로 일어나게 (한 번만)
        if (Input.GetKeyDown(KeyCode.G) && isSitting)
        {
            isSitting = false;
            anim.SetBool("isSit", false);
            // 일어난 뒤에는 그냥 기존 로직대로 따라다님
        }

        // 앉아 있는 동안에는 아무것도 안 하게
        if (isSitting || player == null)
        {
            anim.SetBool("isMoving", false);
            velocity = Vector3.zero;
            return;
        }

        // 플레이어 입력 값 확인 (Input 기반으로 isMoving 판정)
        float dx = Input.GetAxisRaw("Horizontal");
        float dy = Input.GetAxisRaw("Vertical");
        bool playerInputMoving = (Mathf.Abs(dx) > 0.01f || Mathf.Abs(dy) > 0.01f);

        // 플레이어 입력이 있으면 그 입력을 기준으로 마지막 이동 방향 갱신
        if (playerInputMoving)
        {
            Vector3 inputDir = new Vector3(dx, dy, 0f).normalized;
            lastMoveDir = inputDir;
            lastMoveForAnim = inputDir; // 움직일 때 애니메이션은 입력 방향을 사용
        }
        else
        {
            // 입력이 없으면 실제 플레이어 위치 변화로 방향을 갱신할 수 있음
            Vector3 currentPlayerPos = player.position;
            Vector3 posDelta = currentPlayerPos - lastPlayerPos;
            if (posDelta.magnitude > 0.01f)
            {
                lastMoveDir = posDelta.normalized;
                // posDelta는 플레이어 이동이 이미 멈춘 프레임에 0이 될 수 있으므로
                // 애니메이션은 입력이 없을 때는 이전 lastMoveForAnim 유지 (Idle)
            }
            lastPlayerPos = currentPlayerPos;
        }

        // isMoving은 플레이어 입력 기준으로 설정: 플레이어가 멈추면 false
        bool isMoving = playerInputMoving;
        anim.SetBool("isMoving", isMoving);

        // 애니메이션의 inputX, inputY는 움직일 때는 lastMoveForAnim (갱신된 값),
        // 멈출 때는 마지막으로 저장된 lastMoveForAnim 값이 유지된다.
        anim.SetFloat("inputX", lastMoveForAnim.x);
        anim.SetFloat("inputY", lastMoveForAnim.y);

        // 순간이동 조건: 플레이어와 거리가 너무 멀면 워프
        if (Vector3.Distance(transform.position, player.position) > warpDistance)
        {
            transform.position = GetOppositePosition();
            velocity = Vector3.zero;
            return;
        }

        // 자연스럽게 따라가기: 플레이어 이동 반대쪽 위치를 목표로 한다
        Vector3 targetPos = GetOppositePosition();
        float dist = Vector3.Distance(transform.position, targetPos);

        if (dist > minDistance)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPos,
                ref velocity,
                smoothTime
            );
        }
    }

    // 플레이어 이동의 반대 위치를 계산
    Vector3 GetOppositePosition()
    {
        Vector3 p = player.position;
        Vector3 d = lastMoveDir;

        Vector3 opposite;

        if (Mathf.Abs(d.x) > Mathf.Abs(d.y))
        {
            if (d.x > 0f)
                opposite = p + Vector3.left * followOffset;
            else
                opposite = p + Vector3.right * followOffset;
        }
        else
        {
            if (d.y > 0f)
                opposite = p + Vector3.down * followOffset;
            else
                opposite = p + Vector3.up * followOffset;
        }

        // 플레이어와 극히 가까워서 겹칠 위험이 있으면 약간 밀어냄
        if (Vector3.Distance(opposite, p) < minDistance)
        {
            Vector3 push = (opposite - p).normalized * minDistance;
            if (push.magnitude > 0f) opposite = p + push;
        }

        return opposite;
    }
}
