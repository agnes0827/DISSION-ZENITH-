using UnityEngine;
using UnityEngine.SceneManagement;

public class ArtifactProximityPickup : MonoBehaviour
{
    [Header("��Ƽ��Ʈ ����")]
    [SerializeField] private Sprite artifactSprite; // ���� �Ҵ��ϰų� Awake���� ���������� ����

    [Header("��ȣ�ۿ�")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private KeyCode pickupKey = KeyCode.F;
    [SerializeField] private GameObject promptUI;

    [Header("ȸ�� �ƽ�")]
    [SerializeField] private bool hasFlashbackCutscene = false;
    [SerializeField] private string flashbackSceneName; // �ƽ��� ����� �� �̸�

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
            Debug.LogError("ȹ���� ��Ƽ��Ʈ Sprite�� �����ϴ�!");
            return;
        }

        if (hasFlashbackCutscene && !string.IsNullOrEmpty(flashbackSceneName))
        {
            // ȸ�� ���� �ִ� ���
            CutsceneManager.Instance.SetFlashbackData(artifactSprite, SceneManager.GetActiveScene().name);
            gameObject.SetActive(false);
            SceneManager.LoadScene(flashbackSceneName);
        }
        else
        {
            // ȸ�� ���� ���� ���
            Debug.Log("�ƽ� ���� ��� ȹ���մϴ�.");
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
