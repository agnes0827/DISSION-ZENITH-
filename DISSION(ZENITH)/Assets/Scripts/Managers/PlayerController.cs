using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    public string currentMapName; //맵 이름
    public float speed = 3f;
    public float runSpeed;
    private float applyRunSpeed;
    private bool canMove = true;

    public int walkCount;

    private BoxCollider2D boxCollider;
    public LayerMask layermask; //이동 불가 지역

    private Vector2 lastMove = Vector2.down;
    Animator anim;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            anim = GetComponent<Animator>();
            boxCollider = GetComponent<BoxCollider2D>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 새로 로드되었을 때(맵 이동)
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResumeMovement();
        currentMapName = scene.name;

        if (GameStateManager.Instance != null && !string.IsNullOrEmpty(GameStateManager.Instance.returnSceneAfterBattle) && scene.name == GameStateManager.Instance.returnSceneAfterBattle)
        {
            // 전투 전 위치로 플레이어 이동
            transform.position = GameStateManager.Instance.playerPositionBeforeBattle;
            // 애니메이터 설정
            anim.SetFloat("InputX", lastMove.x);
            anim.SetFloat("InputY", lastMove.y);
            anim.SetBool("isMoving", false);

            Debug.Log($"전투 복귀: 이전 위치({GameStateManager.Instance.playerPositionBeforeBattle})로 이동 완료.");

            // 사용한 복귀 씬 이름 정보 초기화
            GameStateManager.Instance.returnSceneAfterBattle = null;

            return; // 스폰 포인트 찾는 로직 실행 안 함
        }

        string spawnId = GameStateManager.Instance.nextSpawnPointId;

        if (!string.IsNullOrEmpty(spawnId))
        {
            SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
            foreach (var spawnPoint in spawnPoints)
            {
                if (spawnPoint.spawnPointId == spawnId)
                {
                    transform.position = spawnPoint.transform.position;
                    transform.rotation = spawnPoint.transform.rotation;

                    anim.SetFloat("InputX", lastMove.x);
                    anim.SetFloat("InputY", lastMove.y);
                    anim.SetBool("isMoving", false);

                    Debug.Log($"'{spawnId}'로 이동");
                    break;
                }
            }
            GameStateManager.Instance.nextSpawnPointId = null;
        }
    }

    IEnumerator MoveCoroutine()
    {
        Vector2 moveVector = Vector2.zero;

        //이동
        while (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            applyRunSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : 0;

            moveVector.x = Input.GetAxisRaw("Horizontal");
            moveVector.y = Input.GetAxisRaw("Vertical");

            if (moveVector.x != 0)
                moveVector.y = 0;

            lastMove = moveVector;

            //애니메이션 파라미터
            anim.SetFloat("InputX", moveVector.x);
            anim.SetFloat("InputY", moveVector.y);
            anim.SetBool("isMoving", true);

            //레이어 마스크
            RaycastHit2D hit;
            Vector2 start = transform.position;
            Vector2 end = start + moveVector * 0.5f;

            boxCollider.enabled = false;
            hit = Physics2D.Linecast(start, end, layermask);
            boxCollider.enabled = true;

            if (hit.transform != null)
            {
                break;

            }

            transform.Translate(moveVector * (speed + applyRunSpeed) * Time.deltaTime);

            yield return null; // 다음 프레임까지 대기
        }

        anim.SetBool("isMoving", false);
        anim.SetFloat("InputX", lastMove.x);
        anim.SetFloat("InputY", lastMove.y);

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

    public void StopMovement()
    {
        StopAllCoroutines();
        anim.SetBool("isMoving", false);
        canMove = false;
    }

    public void ResumeMovement()
    {
        canMove = true;
    }
}
