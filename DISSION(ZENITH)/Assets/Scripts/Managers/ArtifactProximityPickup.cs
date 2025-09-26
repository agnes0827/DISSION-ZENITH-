using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactProximityPickup : MonoBehaviour
{
    [Header("Pickup")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private KeyCode pickupKey = KeyCode.F;
    [SerializeField] private bool destroyAfterPick = true;

    [Header("Menu")]
    [SerializeField] private ArtifactMenu artifactMenu; // ����θ� �ڵ� Ž��
    [SerializeField] private bool openMenuAfterPick = true; // �ݰ� �ڵ� ����

    [Header("UI Prompt (����)")]
    [SerializeField] private GameObject promptUI; // "FŰ�� �ݱ�" �ȳ�

    private bool _playerInRange = false;

    void Awake()
    {
        if (artifactMenu == null)
            artifactMenu = FindObjectOfType<ArtifactMenu>(true);

        if (promptUI) promptUI.SetActive(false);
    }

    void Update()
    {
        if (!_playerInRange) return;
        if (Input.GetKeyDown(pickupKey))
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr == null || sr.sprite == null) return;

            if (artifactMenu == null)
            {
                Debug.LogWarning("ArtifactMenu�� ã�� ���߽��ϴ�.");
                return;
            }

            // �� �޴� �ʱ�ȭ/Ȱ�� ���� (��� 1 ���� Open������ ���)
            if (openMenuAfterPick)
            {
                // �޴� ���鼭(�ִ� ����) ���� �ʱ�ȭ�� ���ο��� ����
                artifactMenu.Open();
            }
            else
            {
                // ������ �ʴ��� Awake���� Init �Ǿ��� ���̹Ƿ� ����
                artifactMenu.gameObject.SetActive(true); // �ʿ信 ����
            }

            if (artifactMenu.TryAddArtifact(sr.sprite))
            {
                if (destroyAfterPick) Destroy(gameObject);
                else gameObject.SetActive(false);
            }
            else
            {
                // �� á�� �� �ǵ��
                Debug.Log("��Ƽ��Ʈ �޴��� ���� á���ϴ�.");
            }
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
