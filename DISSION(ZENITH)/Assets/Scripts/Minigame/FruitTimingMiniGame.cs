using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FruitTimingMiniGame : MonoBehaviour
{
    [Header("UI 참조")]
    public RectTransform arrow;        // MoveBar의 자식
    public RectTransform moveBar;      // 빨간 바 (부모)
    public RectTransform successZone;  // 초록 구간 (MoveBar의 자식)

    public float halfWidth; // 전체 구간
    public float successhalfwidth; // 성공 구간

    public System.Action onSuccess;

    public GameObject playUI;          // 바 + 화살표 + SPACE 안내
    public GameObject resultUI;        // "열매 획득!" + 이미지

    public TMP_Text countText;   // 1/3, 2/3 표시

    [Header("설정")]
    public float moveSpeed = 300f;     // 화살표 속도 (픽셀/초)

    float leftX;
    float rightX;
    float direction = 1f;
    bool isPlaying = true;

    void Start()
    {
        // ★ MoveBar의 로컬 좌표 기준으로 좌우 끝 계산
        leftX = -halfWidth;
        rightX = halfWidth;

        // ★ 화살표, SuccessZone 둘 다 MoveBar의 자식인 상태에서
        //     anchoredPosition.x = 0 이면 바의 정중앙이 됨

        // 시작 위치를 왼쪽 끝으로
        Vector2 pos = arrow.anchoredPosition;
        pos.x = leftX;
        arrow.anchoredPosition = pos;

        playUI.SetActive(true);
        resultUI.SetActive(false);
    }

    void Update()
    {
        if (!isPlaying) return;

        MoveArrow();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckHit();
        }
    }

    void MoveArrow()
    {
        Vector2 pos = arrow.anchoredPosition;
        pos.x += direction * moveSpeed * Time.deltaTime;

        if (pos.x >= rightX)
        {
            pos.x = rightX;
            direction = -1f;
        }
        else if (pos.x <= leftX)
        {
            pos.x = leftX;
            direction = 1f;
        }

        arrow.anchoredPosition = pos;
    }

    void CheckHit()
    {
        float arrowX = arrow.anchoredPosition.x;
        // ★ successZone도 MoveBar의 자식이므로 같은 로컬 좌표계 사용
        float centerX = 0f;
        bool isSuccess = Mathf.Abs(arrowX - centerX) <= successhalfwidth;

        if (isSuccess) OnSuccess();
        else OnFail();
    }

    void OnSuccess()
    {
        isPlaying = false;
        playUI.SetActive(false);
        resultUI.SetActive(true);

        onSuccess?.Invoke();
        StartCoroutine(CloseAfterDelay(4f));

        // 여기서 펫 언락도 가능:
        // if (PetC.Instance != null) PetC.Instance.followUnlocked = true;
    }

    void OnFail()
    {
        ResetGame();
    }

    public void ResetGame()
    {
        isPlaying = true;
        playUI.SetActive(true);
        resultUI.SetActive(false);

        Vector2 pos = arrow.anchoredPosition;
        pos.x = leftX;
        arrow.anchoredPosition = pos;

        direction = 1f;
    }

    public void StartMiniGame()
    {
        gameObject.SetActive(true);
        ResetGame();
    }

    public void UpdateCountText(int collected, int required)
    {
        if (countText != null)
            countText.text = $"{collected}/{required}";
    }

    private IEnumerator CloseAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        // 미니게임 종료 (Prefab 오브젝트 자체를 비활성화)
        gameObject.SetActive(false);
    }
}