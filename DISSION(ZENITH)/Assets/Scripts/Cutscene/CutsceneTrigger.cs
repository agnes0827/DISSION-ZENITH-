using UnityEngine;
using System.Collections.Generic;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("이 씬에서 재생할 컷신 시퀀스")]
    public List<CutsceneManager.CutsceneAction> actions;

    void Start()
    {
        // 씬이 시작되면 즉시 CutsceneManager를 찾아 컷신 재생을 요청합니다.
        if (CutsceneManager.Instance != null)
        {
            CutsceneManager.Instance.Play(actions);
        }
        else
        {
            Debug.LogError("CutsceneManager가 씬에 없습니다! 컷신을 재생할 수 없습니다.");
        }
    }
}
