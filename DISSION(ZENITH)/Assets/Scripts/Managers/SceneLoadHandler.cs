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
            Debug.Log("ȸ�� ������ �����߽��ϴ�. ��Ƽ��Ʈ ȹ�� ��ó���� �����մϴ�.");
            CutsceneManager.Instance.FinalizeAcquisition();
        }
    }
}
