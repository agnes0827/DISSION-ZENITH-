using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class ArtifactClickHandler : MonoBehaviour
{
    public Transform artifactMenuPanel; // ArtifactMenuPanel 오브젝트
    public GameObject artifactImagePrefab; // Image 프리팹
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 클릭 감지
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Artifact"))
            {
                StartCoroutine(HandleArtifactClick(hit.collider.gameObject));
                Debug.Log("오브젝트 클릭");
            }
            Debug.Log("클릭");
        }
    }

    IEnumerator HandleArtifactClick(GameObject clickedArtifact)
    {
        yield return new WaitForSeconds(3f); // 3초 대기

        Destroy(clickedArtifact); // 오브젝트 제거

        if (artifactImagePrefab != null && artifactMenuPanel != null)
        {
            // 이미지 프리팹 생성하여 ArtifactMenuPanel 아래에 추가
            Instantiate(artifactImagePrefab, artifactMenuPanel);
        }
    }
}
