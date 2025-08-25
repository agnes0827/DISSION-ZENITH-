using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DustCleaningGame : MonoBehaviour
{
    [Header("UI 세팅")]
    public GameObject arrowPrefab;             // 방향키 아이콘 프리팹
    public Transform arrowContainer;           // 방향키 아이콘들을 담을 부모

    private List<KeyCode> keySequence = new List<KeyCode>();
    private int currentIndex = 0;
    private System.Action<GameObject> onDustCleaned;
    private GameObject currentDustObject;
    private bool isPlaying = false;

    // PNG 경로
    private const string ICON_PATH = "UI/icon/";

    // 미니게임 시작
    public void BeginGame(GameObject dustObject, System.Action<GameObject> onComplete)
    {
        currentDustObject = dustObject;
        onDustCleaned = onComplete;

        isPlaying = true;
        gameObject.SetActive(true);

        GenerateRandomSequence();
        CreateArrowUI();
    }

    // 랜덤 시퀀스 생성
    private void GenerateRandomSequence()
    {
        keySequence.Clear();
        KeyCode[] arrows = { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };

        int arrowCount = Random.Range(4, 6); // 4~5개 랜덤
        for (int i = 0; i < arrowCount; i++)
        {
            keySequence.Add(arrows[Random.Range(0, arrows.Length)]);
        }

        currentIndex = 0;
    }

    // 방향키 UI 생성
    private void CreateArrowUI()
    {
        // 기존 화살표 삭제
        foreach (Transform child in arrowContainer)
            Destroy(child.gameObject);

        // 새로 생성
        foreach (var key in keySequence)
        {
            GameObject arrow = Instantiate(arrowPrefab, arrowContainer);
            Image img = arrow.GetComponent<Image>();
            img.sprite = GetArrowSprite(key);
        }
    }

    // PNG 경로를 기반으로 스프라이트 로드
    private Sprite GetArrowSprite(KeyCode key)
    {
        string fileName = key switch
        {
            KeyCode.UpArrow => "Up",
            KeyCode.DownArrow => "Down",
            KeyCode.LeftArrow => "Left",
            KeyCode.RightArrow => "Right",
            _ => "Up"
        };

        return Resources.Load<Sprite>($"{ICON_PATH}{fileName}");
    }

    void Update()
    {
        if (!isPlaying) return;

        if (currentIndex < keySequence.Count)
        {
            if (Input.GetKeyDown(keySequence[currentIndex]))
            {
                arrowContainer.GetChild(currentIndex).GetComponent<Image>().color = Color.green;
                currentIndex++;

                // 모두 입력 완료
                if (currentIndex >= keySequence.Count)
                    OnSuccess();
            }
            else if (Input.anyKeyDown)
            {
                // 오답 → 다시 시작
                RestartSequence();
            }
        }
    }

    private void RestartSequence()
    {
        currentIndex = 0;
        for (int i = 0; i < arrowContainer.childCount; i++)
            arrowContainer.GetChild(i).GetComponent<Image>().color = Color.white;
    }

    private void OnSuccess()
    {
        isPlaying = false;
        gameObject.SetActive(false);

        // 먼지 제거 콜백 실행
        onDustCleaned?.Invoke(currentDustObject);
    }
}
