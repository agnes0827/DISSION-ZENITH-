using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance { get; private set; }

    [SerializeField] private DustCleaningGame dustCleaningGameUI;
    private PlayerController playerController;
    public static bool IsMiniGameActive { get; private set; } = false;

    private AudioSource currentBgmSource;
    private float savedBgmVolume;        

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("MiniGameManager 인스턴스가 이미 존재합니다. 새로 생성된 인스턴스를 파괴합니다.");
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        FindPlayer();
        FindUI();

        if (dustCleaningGameUI != null)
        {
            dustCleaningGameUI.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void FindPlayer()
    {
        if (playerController != null) return;

        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("MiniGameManager: PlayerController를 찾을 수 없습니다!");
        }
    }

    private void FindUI()
    {
        if (dustCleaningGameUI == null)
        {
            dustCleaningGameUI = FindObjectOfType<DustCleaningGame>(true);
            if (dustCleaningGameUI == null)
            {
                Debug.LogError("MiniGameManager: DustCleaningGame UI를 찾을 수 없습니다!");
            }
        }
    }

    void OnEnable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.OnMiniGameRequested += StartDustCleaning;
        }
        else
        {
            Debug.LogError("MiniGameManager: DialogueManager를 찾을 수 없습니다!");
        }
    }

    void OnDisable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.OnMiniGameRequested -= StartDustCleaning;
        }
    }

    public void StartDustCleaning(GameObject dustObject)
    {
        if (playerController == null) FindPlayer();
        if (playerController == null) return;

        if (dustCleaningGameUI == null) FindUI();
        if (dustCleaningGameUI == null) return;

        playerController.StopMovement();
        IsMiniGameActive = true;

        FindAndLowerBGM();

        dustCleaningGameUI.gameObject.SetActive(true);
        dustCleaningGameUI.BeginGame(dustObject, this.OnDustCleaned);
    }

    private void FindAndLowerBGM()
    {
        AudioSource[] allSources = FindObjectsOfType<AudioSource>();

        foreach (var source in allSources)
        {
            if (source.isPlaying && source != dustCleaningGameUI.clockAudioSource)
            {
                currentBgmSource = source;
                savedBgmVolume = source.volume; // 원래 볼륨 저장 (예: 0.35)
                source.volume = 0.15f;           // 0.1로 줄임
                break;
            }
        }
    }

    // 먼지 하나 제거 성공 시 호출됨
    private void OnDustCleaned(GameObject dustObject)
    {
        dustObject.SetActive(false);
        EndDustCleaning();

        var dustComponent = dustObject.GetComponent<DustObject>();
        if (dustComponent != null)
        {
            GameStateManager.Instance.cleanedDustIds.Add(dustComponent.dustId);
        }
        else
        {
            Debug.LogWarning("DustObject에 DustObject 스크립트가 없습니다!");
            return;
        }

        int currentCleanedCount = GameStateManager.Instance.cleanedDustIds.Count;
        Debug.Log($"먼지 제거 완료 ({currentCleanedCount} / 3)");

        // 먼지 3개 전부 제거 시 → 2층 열쇠 드랍
        if (currentCleanedCount >= 3 && !GameStateManager.Instance.isDustCleaningQuestCompleted)
        {
            Debug.Log("2층 열쇠 획득");
            GameStateManager.Instance.isDustCleaningQuestCompleted = true;
            InventoryManager.Instance.AddItem("Library_Key_Floor2", "도서관 2층 열쇠");

            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue("40005");
            }
        }
    }

    public void EndDustCleaning()
    {
        IsMiniGameActive = false;

        if (currentBgmSource != null)
        {
            currentBgmSource.volume = savedBgmVolume; // 저장해둔 볼륨으로 복구
            currentBgmSource = null; // 참조 해제
        }

        if (dustCleaningGameUI != null)
        {
            dustCleaningGameUI.gameObject.SetActive(false);
        }

        if (playerController != null)
        {
            playerController.ResumeMovement();
        }
    }
}
