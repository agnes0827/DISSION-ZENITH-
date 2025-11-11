using UnityEngine;
using UnityEngine.SceneManagement;

public class ArtifactPickup : MonoBehaviour
{
    [Header("아티팩트 고유 정보")]
    [Tooltip("절대 중복되지 않는 고유 ID를 입력하세요. 예: World01_Pendant")]
    public string artifactID;

    [Header("상호작용")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private KeyCode pickupKey = KeyCode.F;
    [SerializeField] private GameObject promptUI;

    [Header("데이터 연결")]
    [Tooltip("모든 아티팩트 정보가 담긴 ArtifactDatabase를 연결하세요.")]
    [SerializeField] private ArtifactDatabase artifactDatabase;

    private bool _playerInRange = false;
    private ArtifactEventViewer _eventViewer;

    void Awake()
    {
        if (string.IsNullOrEmpty(artifactID))
        {
            Debug.LogError($"{gameObject.name}에 artifactID가 설정되지 않았습니다! 진행 상황 저장이 불가능합니다.", gameObject);
        }

        if (promptUI) promptUI.SetActive(false);
    }

    void Start()
    {
        // GameStateManager에 artifactID가 이미 저장되어 있다면
        if (GameStateManager.Instance != null && GameStateManager.Instance.collectedArtifactIDs.Contains(artifactID))
        {
            gameObject.SetActive(false);
            Debug.Log($"아티팩트 '{artifactID}'는 이미 획득했습니다.");
        }
        _eventViewer = ArtifactEventViewer.Instance;
    }
    void Update()
    {
        if (!_playerInRange || (CutsceneManager.Instance != null && CutsceneManager.IsCutscenePlaying))
        {
            return;
        }

        if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogue)
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
        if (artifactDatabase == null)
        {
            Debug.LogError("ArtifactDatabase가 ArtifactPickup에 연결되지 않았습니다!", gameObject);
            return;
        }

        ArtifactDefinition def = artifactDatabase.GetArtifactByID(artifactID);
        if (def == null)
        {
            Debug.LogError($"ArtifactDatabase에서 ID '{artifactID}'를 찾을 수 없습니다.", gameObject);
            return;
        }

        if (def.hasInPlaceEvent && _eventViewer != null)
        {
            Debug.Log($"인플레이스 이벤트 시작: {artifactID}");
            if (promptUI) promptUI.SetActive(false);
            _playerInRange = false;

            // 이벤트 뷰어를 보여주고, 이벤트가 끝나면 AddArtifactToSystem을 호출하도록 콜백 전달
            _eventViewer.ShowEvent(def, () => {
                AddArtifactToSystem();
            });

        }
        else if (!string.IsNullOrEmpty(def.flashbackSceneName))
        {
            Debug.Log($"[ArtifactPickup] 회상 씬 로드: {def.flashbackSceneName}");
            CutsceneManager.Instance.SetFlashbackData(def.artifactSprite, SceneManager.GetActiveScene().name, artifactID);

            gameObject.SetActive(false);
            SceneManager.LoadScene(def.flashbackSceneName);
        }
        // 3. 아무 이벤트 없는 경우 (즉시 획득)
        else
        {
            Debug.Log($"즉시 획득: {artifactID}");
            AddArtifactToSystem();
        }
    }

    private void AddArtifactToSystem()
    {
        if (GameStateManager.Instance != null && !GameStateManager.Instance.collectedArtifactIDs.Contains(artifactID))
        {
            GameStateManager.Instance.collectedArtifactIDs.Add(artifactID);
            Debug.Log($"GameStateManager에 아티팩트 ID '{artifactID}' 기록됨.");

            ArtifactMenu currentMenu = FindObjectOfType<ArtifactMenu>();
            if (currentMenu != null)
            {
                currentMenu.RepopulateUI();
                Debug.Log("현재 씬의 ArtifactMenu UI 새로고침 요청됨.");
            }
            else
            {
                Debug.LogWarning("현재 씬에서 ArtifactMenu를 찾을 수 없어 UI를 새로고침할 수 없습니다.");
            }
        }
        gameObject.SetActive(false);
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
