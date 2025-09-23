using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadHandler : MonoBehaviour
{
    void Start()
    {
        CheckForPendingAcquisition();
    }

    private void CheckForPendingAcquisition()
    {
        if (CutsceneManager.Instance != null && CutsceneManager.Instance.needsAcquisitionCompletion)
        {
            Debug.Log("회상 씬에서 복귀했습니다. 아티팩트 획득 후처리를 시작합니다.");
            CutsceneManager.Instance.FinalizeAcquisition();
        }
    }
}
