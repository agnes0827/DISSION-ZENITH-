using UnityEngine;
using UnityEngine.SceneManagement; // SceneManager를 사용하기 위해 필수입니다.

public class SimpleSceneLoaderButton : MonoBehaviour
{
    [Header("이동할 씬 이름")]
    [Tooltip("이 버튼을 눌렀을 때 로드할 씬의 이름을 정확하게 입력하세요.")]
    [SerializeField] private string sceneToLoad;

    /// <summary>
    /// 이 함수를 UI 버튼의 OnClick() 이벤트에 연결하세요.
    /// </summary>
    public void LoadTargetScene()
    {
        Debug.Log("버튼 눌림, sceneToLoad = " + sceneToLoad);
        // sceneToLoad 변수에 이름이 비어있는지 확인합니다.
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("로드할 씬 이름이 지정되지 않았습니다! 인스펙터 창에서 sceneToLoad 변수를 설정해주세요.");
            return;
        }

        // 지정된 이름의 씬을 로드합니다.
        SceneManager.LoadScene(sceneToLoad);
    }
}
