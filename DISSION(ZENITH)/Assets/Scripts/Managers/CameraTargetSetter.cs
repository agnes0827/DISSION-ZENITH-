using UnityEngine;
using Cinemachine;

public class CameraTargetSetter : MonoBehaviour
{
    void Start()
    {
        var vcam = GetComponent<CinemachineVirtualCamera>();

        if (PlayerController.Instance != null)
        {
            vcam.Follow = PlayerController.Instance.transform;
        }
        else
        {
            Debug.LogError("플레이어를 찾을 수 없습니다!");
        }
    }
}