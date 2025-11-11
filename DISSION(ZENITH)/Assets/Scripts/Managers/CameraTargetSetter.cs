using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class CameraTargetSetter : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;
    void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        TrySetTarget();   // 처음 씬에서도 한 번
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TrySetTarget();   // 씬이 바뀔 때마다 다시 시도
    }

    private void TrySetTarget()
    {
        Transform target = null;

        // 1) 싱글톤 PlayerController가 있으면 그거 우선
        if (PlayerController.Instance != null)
        {
            target = PlayerController.Instance.transform;
        }
        

        if (target != null && vcam != null)
        {
            vcam.Follow = target;
            vcam.LookAt = target;
            Debug.Log($"[CameraTargetSetter] 타겟 설정: {target.name}");
        }
        else
        {
            Debug.LogWarning("[CameraTargetSetter] 플레이어를 찾지 못했습니다.");
        }
    }
}