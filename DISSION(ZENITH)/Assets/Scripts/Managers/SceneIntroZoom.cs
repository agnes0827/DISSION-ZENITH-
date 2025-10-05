using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SceneIntroZoom : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera vcam;
    [SerializeField] Transform player;
    [SerializeField] Transform axe;

    [Header("Timing")]
    [SerializeField] float delayAfterLoad = 2f; // �� �ε� �� ��ٸ� �ð�

    [SerializeField] float focusZoom = 6f;   // ������ ���� �� ��(�������� �� Ȯ��)
    [SerializeField] float focusHold = 1.0f; // ������ ���ߴ� �ð�
    [SerializeField] float blendTime = 0.6f; // �� ��/�ƿ� ���� �ð�

    float originalSize;
    Transform originalFollow;

    IEnumerator Start()
    {
        if (!vcam) vcam = FindObjectOfType<CinemachineVirtualCamera>();
        originalSize = vcam.m_Lens.OrthographicSize;
        originalFollow = vcam.Follow;

        // �� �ε� �� 2�� ��ٷȴٰ� ���� ����
        yield return new WaitForSecondsRealtime(delayAfterLoad);

        // ������ ��ȯ + �� ��
        vcam.Follow = axe;
        yield return LerpSize(originalSize, focusZoom, blendTime);

        // ��� ����
        yield return new WaitForSeconds(focusHold);

        // �÷��̾�� ���� + �� �ƿ�
        vcam.Follow = player;
        yield return LerpSize(focusZoom, originalSize, blendTime);
    }

    IEnumerator LerpSize(float from, float to, float dur)
    {
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(from, to, Mathf.SmoothStep(0, 1, t / dur));
            yield return null;
        }
        vcam.m_Lens.OrthographicSize = to;
    }
}
