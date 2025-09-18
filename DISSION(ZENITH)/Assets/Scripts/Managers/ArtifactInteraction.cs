using UnityEngine;

public class ArtifactInteraction : MonoBehaviour
{
    [Tooltip("아티팩트 상단 메뉴에 등록할 스프라이트")]
    [SerializeField] private Sprite artifactSprite;

    private bool hasPlayed = false;

    public void Interact()
    {
        if (hasPlayed) return;

        if (CutsceneManager.Instance != null && artifactSprite != null)
        {
            CutsceneManager.Instance.Play(artifactSprite);

            hasPlayed = true;

            // 컷씬 시작 후 콜라이더 비활성화 (재상호작용 방지)
            GetComponent<Collider2D>().enabled = false;

            gameObject.SetActive(false); // 컷신 시작 시 아티팩트 오브젝트를 숨김
        }
        else
        {
            Debug.LogWarning("[ArtifactInteraction] 컷씬 또는 스프라이트 데이터가 누락되었습니다.");
        }
    }
}