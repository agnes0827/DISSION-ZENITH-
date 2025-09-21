using UnityEngine;

public class ArtifactInteraction : MonoBehaviour
{
    [Tooltip("��Ƽ��Ʈ ��� �޴��� ����� ��������Ʈ")]
    [SerializeField] private Sprite artifactSprite;

    private bool hasPlayed = false;

    public void Interact()
    {
        if (hasPlayed) return;

        if (CutsceneManager.Instance != null && artifactSprite != null)
        {
            CutsceneManager.Instance.Play(artifactSprite);

            hasPlayed = true;

            // �ƾ� ���� �� �ݶ��̴� ��Ȱ��ȭ (���ȣ�ۿ� ����)
            GetComponent<Collider2D>().enabled = false;

            gameObject.SetActive(false); // �ƽ� ���� �� ��Ƽ��Ʈ ������Ʈ�� ����
        }
        else
        {
            Debug.LogWarning("[ArtifactInteraction] �ƾ� �Ǵ� ��������Ʈ �����Ͱ� �����Ǿ����ϴ�.");
        }
    }
}