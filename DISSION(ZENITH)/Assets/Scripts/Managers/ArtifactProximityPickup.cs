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
    [SerializeField] private ArtifactMenu artifactMenu; // 비워두면 자동 탐색
    [SerializeField] private bool openMenuAfterPick = true; // 줍고 자동 열기

    [Header("UI Prompt (선택)")]
    [SerializeField] private GameObject promptUI; // "F키로 줍기" 안내

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
                Debug.LogWarning("ArtifactMenu를 찾지 못했습니다.");
                return;
            }

            // ↓ 메뉴 초기화/활성 보장 (방법 1 쓰면 Open만으로 충분)
            if (openMenuAfterPick)
            {
                // 메뉴 열면서(애니 포함) 슬롯 초기화도 내부에서 보장
                artifactMenu.Open();
            }
            else
            {
                // 열지는 않더라도 Awake에서 Init 되었을 것이므로 안전
                artifactMenu.gameObject.SetActive(true); // 필요에 따라
            }

            if (artifactMenu.TryAddArtifact(sr.sprite))
            {
                if (destroyAfterPick) Destroy(gameObject);
                else gameObject.SetActive(false);
            }
            else
            {
                // 꽉 찼을 때 피드백
                Debug.Log("아티팩트 메뉴가 가득 찼습니다.");
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
