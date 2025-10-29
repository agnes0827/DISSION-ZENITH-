using UnityEngine;
using UnityEngine.SceneManagement;

public class ArtifactPickup : MonoBehaviour
{
    [Header("아티팩트 고유 정보")]
    [Tooltip("절대 중복되지 않는 고유 ID를 입력하세요. 예: World01_Pendant")]
    public string artifactID;
    [SerializeField] private Sprite artifactSprite;

    [Header("상호작용")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private KeyCode pickupKey = KeyCode.F;
    [SerializeField] private GameObject promptUI;

    [Header("회상 컷신")]
    [SerializeField] private bool hasFlashbackCutscene = false;
    [SerializeField] private string flashbackSceneName;

    private bool _playerInRange = false;

    void Awake()
    {
        if (string.IsNullOrEmpty(artifactID))
        {
            Debug.LogError($"{gameObject.name}에 artifactID가 설정되지 않았습니다! 진행 상황 저장이 불가능합니다.", gameObject);
        }

        if (artifactSprite == null)
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                artifactSprite = sr.sprite;
            }
        }
        if (promptUI) promptUI.SetActive(false);
    }

    void Start()
    {
        // GameStateManager에 artifactID가 이미 저장되어 있다면
        if (GameStateManager.Instance != null && GameStateManager.Instance.collectedArtifactIDs.Contains(artifactID))
        {
            // 오브젝트를 비활성화
            gameObject.SetActive(false);
            Debug.Log($"아티팩트 '{artifactID}'는 이미 획득했습니다.");
        }

    }
    void Update()
    {
        if (!_playerInRange || (CutsceneManager.Instance != null && CutsceneManager.IsCutscenePlaying))
        {
            return;
        }

        if (Input.GetKeyDown(pickupKey))
        {
            PickUp();
        }
    }

    private void PickUp()
    {
        if (hasFlashbackCutscene)
        {
            // 회상 씬이 있는 경우: CutsceneManager에게 데이터를 넘기고 씬 이동
            Debug.Log($"[ArtifactProximityPickup] PickUp called. artifactID: '{artifactID}'");
            CutsceneManager.Instance.SetFlashbackData(artifactSprite, SceneManager.GetActiveScene().name, artifactID);
            gameObject.SetActive(false);
            SceneManager.LoadScene(flashbackSceneName);
        }
        else
        {
            // 회상 씬이 없는 경우: GameStateManager에 즉시 기록
            if (GameStateManager.Instance != null && !GameStateManager.Instance.collectedArtifactIDs.Contains(artifactID))
            {
                GameStateManager.Instance.collectedArtifactIDs.Add(artifactID);
                Debug.Log($"GameStateManager에 아티팩트 ID '{artifactID}' 기록됨.");

                if (ArtifactMenu.Instance != null)
                {
                    ArtifactMenu.Instance.RepopulateUI();
                    Debug.Log("ArtifactMenu UI 새로고침 요청됨.");
                }
                else
                {
                    Debug.LogWarning("ArtifactMenu Instance를 찾을 수 없어 UI를 새로고침할 수 없습니다.");
                }
            }
            gameObject.SetActive(false);
        }
    }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(playerTag))
            {
                _playerInRange = true;
                if (promptUI) promptUI.SetActive(true);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(playerTag))
            {
                _playerInRange = false;
                if (promptUI) promptUI.SetActive(false);
            }
        }
    }
