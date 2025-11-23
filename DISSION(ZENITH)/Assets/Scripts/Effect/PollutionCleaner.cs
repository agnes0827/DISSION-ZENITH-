using UnityEngine;

public class PollutionCleaner : MonoBehaviour
{
    public PollutionController myPollution;

    void Start()
    {
        if (GameStateManager.Instance == null) return;

        // 도서관이 정화된 상태라면
        if (GameStateManager.Instance.isLibraryPurified)
        {
            if (myPollution != null)
                myPollution.SetPurifiedImmediate();
            else
                gameObject.SetActive(false);
        }
    }
}