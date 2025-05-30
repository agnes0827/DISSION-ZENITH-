using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class ArtifactClickHandler : MonoBehaviour
{
    public Transform artifactMenuPanel; // ArtifactMenuPanel ������Ʈ
    public GameObject artifactImagePrefab; // Image ������
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ���콺 Ŭ�� ����
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Artifact"))
            {
                StartCoroutine(HandleArtifactClick(hit.collider.gameObject));
                Debug.Log("������Ʈ Ŭ��");
            }
            Debug.Log("Ŭ��");
        }
    }

    IEnumerator HandleArtifactClick(GameObject clickedArtifact)
    {
        yield return new WaitForSeconds(3f); // 3�� ���

        Destroy(clickedArtifact); // ������Ʈ ����

        if (artifactImagePrefab != null && artifactMenuPanel != null)
        {
            // �̹��� ������ �����Ͽ� ArtifactMenuPanel �Ʒ��� �߰�
            Instantiate(artifactImagePrefab, artifactMenuPanel);
        }
    }
}
