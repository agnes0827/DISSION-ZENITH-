using UnityEngine;

public class SceneLoadHandler : MonoBehaviour
{
    void Start()
    {
        CheckForPendingAcquisition();
        UpdateArtifactsState();
    }

    private void CheckForPendingAcquisition()
    {
        if (CutsceneManager.Instance != null && CutsceneManager.Instance.needsAcquisitionCompletion)
        {
            string newArtifactID = CutsceneManager.Instance.collectedArtifactID;

            if (GameStateManager.Instance != null && !GameStateManager.Instance.collectedArtifactIDs.Contains(newArtifactID))
            {
                GameStateManager.Instance.collectedArtifactIDs.Add(newArtifactID);
                Debug.Log($"[GameStateManager] 새로운 아티팩트 ID 기록: {newArtifactID}");
            }

            // UI 등록 및 데이터 초기화
            CutsceneManager.Instance.FinalizeAcquisition();
        }
    }

    private void UpdateArtifactsState()
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogWarning("GameStateManager가 없어 아티팩트 상태를 업데이트 할 수 없습니다.");
            return;
        }

        ArtifactPickup[] artifactsInScene = FindObjectsOfType<ArtifactPickup>();

        foreach (var artifact in artifactsInScene)
        {
            if (GameStateManager.Instance.collectedArtifactIDs.Contains(artifact.artifactID))
            {
                artifact.gameObject.SetActive(false);
            }
        }
        Debug.Log("씬의 모든 아티팩트 상태를 최신화했습니다.");
    }
}

