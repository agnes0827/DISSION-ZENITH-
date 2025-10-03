using UnityEngine;
using System.Collections.Generic;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("�� ������ ����� �ƽ� ������")]
    public List<CutsceneManager.CutsceneAction> actions;

    void Start()
    {
        // ���� ���۵Ǹ� ��� CutsceneManager�� ã�� �ƽ� ����� ��û�մϴ�.
        if (CutsceneManager.Instance != null)
        {
            CutsceneManager.Instance.Play(actions);
        }
        else
        {
            Debug.LogError("CutsceneManager�� ���� �����ϴ�! �ƽ��� ����� �� �����ϴ�.");
        }
    }
}
