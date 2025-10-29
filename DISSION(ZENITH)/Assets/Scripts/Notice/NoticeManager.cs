using UnityEngine;

public class NoticeManager : MonoBehaviour
{
    public static NoticeManager Instance { get; private set; }

    [SerializeField] private GameObject noticeUIPrefab; // Inspector에서 NoticeUI 프리팹 연결

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowNotice(string message)
    {
        if (noticeUIPrefab == null)
        {
            Debug.LogError("NoticeManager: NoticeUIPrefab이 연결되지 않았습니다!");
            return;
        }

        // 현재 씬의 Canvas 찾기
        GameObject canvasGO = GameObject.FindWithTag("GameCanvas");
        if (canvasGO == null)
        {
            Debug.LogError("NoticeManager: 현재 씬에서 'GameCanvas' 태그를 가진 Canvas를 찾을 수 없습니다!");
            return;
        }

        // Canvas 아래에 프리팹 생성
        GameObject noticeObject = Instantiate(noticeUIPrefab, canvasGO.transform);

        // 생성된 오브젝트에서 NoticeUI 스크립트 가져오기
        NoticeUI noticeUI = noticeObject.GetComponent<NoticeUI>();
        if (noticeUI != null)
        {
            // 메시지 전달 및 애니메이션 시작
            noticeUI.StartNotice(message);
        }
        else
        {
            Debug.LogError("NoticeManager: 생성된 NoticeUI 오브젝트에서 NoticeUI 스크립트를 찾을 수 없습니다!");
            Destroy(noticeObject); // 스크립트 없으면 그냥 파괴
        }
    }
}