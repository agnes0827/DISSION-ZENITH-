//using UnityEngine;

//public class SceneChange : MonoBehaviour
//{
//    public int sceneToLoad; // �ε��� �� ��ȣ
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
//    public int TargetSceneIndex; // �̵��� �� ��ȣ
//    private SceneFadeManager fadeManager;

//    private void Start()
//    {
//        // ���� �ִ� SceneFadeManager ã��
//        fadeManager = FindObjectOfType<SceneFadeManager>();
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision.CompareTag("Player"))
//        {
//            // �� ��ȯ ��û (FadeManager�� ó��)
//            fadeManager.FadeToScene(TargetSceneIndex);
//        }
//    }
//}
