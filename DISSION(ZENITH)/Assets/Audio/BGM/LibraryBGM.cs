using UnityEngine;
using UnityEngine.SceneManagement;

public class LibraryBGM : MonoBehaviour
{
    public static LibraryBGM instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName != "Library1F" && sceneName != "Library2F")
        {
            Destroy(gameObject);
            instance = null;
        }
    }
}