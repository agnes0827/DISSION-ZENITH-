using UnityEngine;

public class NoticeTrigger : MonoBehaviour
{
    public string noticeId;
    [TextArea]
    [SerializeField] private string noticeMessage;

    private Collider2D triggerCollider;

    void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        if (triggerCollider == null)
        {
            Debug.LogError($"NoticeTriggerZone ({gameObject.name}): Collider2D가 없습니다!");
        }
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(noticeId) && GameStateManager.Instance.triggeredNoticeIds.Contains(noticeId))
        {
            if (triggerCollider != null) triggerCollider.enabled = false; // 이미 봤으면 콜라이더 끄기
                                                                          // 또는 gameObject.SetActive(false);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (string.IsNullOrEmpty(noticeId)) return;
        if (!other.CompareTag("Player")) return;

        // GameStateManager 플래그 확인 (이미 봤으면 무시)
        if (!GameStateManager.Instance.triggeredNoticeIds.Contains(noticeId))
        {
            // 알림 띄우기
            if (NoticeManager.Instance != null)
            {
                NoticeManager.Instance.ShowNotice(noticeMessage);
                // GameStateManager에 기록
                GameStateManager.Instance.triggeredNoticeIds.Add(noticeId);
                Debug.Log($"Notice Triggered and Flagged: {noticeId}");


                if (triggerCollider != null) triggerCollider.enabled = false;
                // 또는 gameObject.SetActive(false); // 오브젝트 전체를 숨겨도 되면 이게 더 확실
            }
        }
    }
}