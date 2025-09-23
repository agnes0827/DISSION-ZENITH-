using UnityEngine;
using UnityEngine.SceneManagement;

public class ArtifactProximityPickup : MonoBehaviour
{
    [Header("아티팩트 정보")]
    [SerializeField] private Sprite artifactSprite; // 직접 할당하거나 Awake에서 가져오도록 수정

    [Header("상호작용")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private KeyCode pickupKey = KeyCode.F;
    [SerializeField] private GameObject promptUI;

    [Header("회상 컷신")]
    [SerializeField] private bool hasFlashbackCutscene = false;
    [SerializeField] private string flashbackSceneName; // 컷신이 재생될 씬 이름

    private bool _playerInRange = false;

    void Awake()
    {
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
        if (artifactSprite == null)
        {
            Debug.LogError("획득할 아티팩트 Sprite가 없습니다!");
            return;
        }

        if (hasFlashbackCutscene && !string.IsNullOrEmpty(flashbackSceneName))
        {
            // 회상 씬이 있는 경우
            CutsceneManager.Instance.SetFlashbackData(artifactSprite, SceneManager.GetActiveScene().name);
            gameObject.SetActive(false);
            SceneManager.LoadScene(flashbackSceneName);
        }
        else
        {
            // 회상 씬이 없는 경우
            Debug.Log("컷신 없이 즉시 획득합니다.");
            var artifactMenu = FindObjectOfType<ArtifactMenu>(true);
            if (artifactMenu != null)
            {
                artifactMenu.TryAddArtifact(artifactSprite);
            }
            Destroy(gameObject);
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
