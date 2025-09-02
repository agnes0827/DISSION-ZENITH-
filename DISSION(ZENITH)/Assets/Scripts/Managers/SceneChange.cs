//using UnityEngine;

//public class SceneChange : MonoBehaviour
//{
//    public int sceneToLoad; // 로드할 씬 번호
//    private SceneFadeManager fadeManager;

//    void Start()
//    {
//        fadeManager = FindObjectOfType<SceneFadeManager>();
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision.CompareTag("Player"))
//        {
//            fadeManager.Fade(sceneToLoad);
//        }
//    }
//}
//using UnityEngine;

//public class SceneChange : MonoBehaviour
//{
//    public int TargetSceneIndex; // 이동할 씬 번호
//    private SceneFadeManager fadeManager;

//    private void Start()
//    {
//        // 씬에 있는 SceneFadeManager 찾기
//        fadeManager = FindObjectOfType<SceneFadeManager>();
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision.CompareTag("Player"))
//        {
//            // 씬 전환 요청 (FadeManager가 처리)
//            fadeManager.FadeToScene(TargetSceneIndex);
//        }
//    }
//}
