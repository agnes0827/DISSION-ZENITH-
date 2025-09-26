using UnityEngine;
using UnityEngine.SceneManagement; // SceneManager�� ����ϱ� ���� �ʼ��Դϴ�.

public class SimpleSceneLoaderButton : MonoBehaviour
{
    [Header("�̵��� �� �̸�")]
    [Tooltip("�� ��ư�� ������ �� �ε��� ���� �̸��� ��Ȯ�ϰ� �Է��ϼ���.")]
    [SerializeField] private string sceneToLoad;

    /// <summary>
    /// �� �Լ��� UI ��ư�� OnClick() �̺�Ʈ�� �����ϼ���.
    /// </summary>
    public void LoadTargetScene()
    {
        // sceneToLoad ������ �̸��� ����ִ��� Ȯ���մϴ�.
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("�ε��� �� �̸��� �������� �ʾҽ��ϴ�! �ν����� â���� sceneToLoad ������ �������ּ���.");
            return;
        }

        // ������ �̸��� ���� �ε��մϴ�.
        SceneManager.LoadScene(sceneToLoad);
    }
}
