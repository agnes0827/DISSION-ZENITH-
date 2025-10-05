using UnityEngine;

public class DustObject : MonoBehaviour
{
    public string dustId;

    void Start()
    {
        if (GameStateManager.Instance.cleanedDustIds.Contains(dustId))
        {
            gameObject.SetActive(false);
        }
    }
}