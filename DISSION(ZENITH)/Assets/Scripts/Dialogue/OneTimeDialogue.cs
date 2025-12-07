using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class OneTimeDialogue : MonoBehaviour
{
    [Header("설정")]
    [Tooltip("실행할 대화 ID (CSV에 있는 것)")]
    public string dialogueID; // 예: "Book_History_01"

    [Tooltip("씬 내에서 유일한 오브젝트 ID (중복 금지)")]
    public string objectID;   // 예: "Library_NormalBook_01"

    private bool isPlayerInRange = false;

    private void Start()
    {
        // 게임 매니저가 없으면 중단
        if (GameStateManager.Instance == null) return;

        // 이미 읽은(수집된) 책이라면 시작하자마자 파괴 (화면에 안 그림)
        if (GameStateManager.Instance.collectedSceneObjectIDs.Contains(objectID))
        {
            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = false;
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F) && !DialogueManager.Instance.isDialogue)
        {
            ReadAndDestroy();
        }
    }

    private void ReadAndDestroy()
    {
        // 1. 대화 매니저에게 대화 시작 요청
        DialogueManager.Instance.StartDialogue(dialogueID);

        // 2. 게임 매니저에 기록
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.collectedSceneObjectIDs.Add(objectID);
        }

        // 3. 반짝이 오브젝트 즉시 파괴
        Destroy(gameObject);
    }
}