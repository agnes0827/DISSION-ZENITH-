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
                Debug.Log($"[GameStateManager] ���ο� ��Ƽ��Ʈ ID ���: {newArtifactID}");
            }

            // UI ��� �� ������ �ʱ�ȭ
            CutsceneManager.Instance.FinalizeAcquisition();
        }
    }

    private void UpdateArtifactsState()
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogWarning("GameStateManager�� ���� ��Ƽ��Ʈ ���¸� ������Ʈ �� �� �����ϴ�.");
            return;
        }

        ArtifactProximityPickup[] artifactsInScene = FindObjectsOfType<ArtifactProximityPickup>();

        foreach (var artifact in artifactsInScene)
        {
            if (GameStateManager.Instance.collectedArtifactIDs.Contains(artifact.artifactID))
            {
                artifact.gameObject.SetActive(false);
            }
        }
        Debug.Log("���� ��� ��Ƽ��Ʈ ���¸� �ֽ�ȭ�߽��ϴ�.");
    }
}

