using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DustCleaningGame : MonoBehaviour
{
    [Header("UI 세팅")]
    public GameObject arrowPrefab;
    public Transform arrowContainer;

    [Header("피드백 색상")]
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color wrongColor = Color.red;
    [SerializeField] private float wrongFlashDuration = 0.15f;

    private readonly List<KeyCode> keySequence = new List<KeyCode>();
    private readonly List<Image> arrowImages = new List<Image>(); // 인덱스 매칭용
    private int currentIndex = 0;

    private System.Action<GameObject> onDustCleaned;
    private GameObject currentDustObject;
    private bool isPlaying = false;

    private static readonly KeyCode[] ARROWS =
        { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };

    private const string ICON_PATH = "UI/icon/";

    public void BeginGame(GameObject dustObject, System.Action<GameObject> onComplete)
    {
        currentDustObject = dustObject;
        onDustCleaned = onComplete;

        isPlaying = true;
        gameObject.SetActive(true);

        GenerateRandomSequence();
        CreateArrowUI();
    }

    private void GenerateRandomSequence()
    {
        keySequence.Clear();
        int arrowCount = Random.Range(4, 6); // 4~5개
        for (int i = 0; i < arrowCount; i++)
            keySequence.Add(ARROWS[Random.Range(0, ARROWS.Length)]);

        currentIndex = 0;
    }

    private void CreateArrowUI()
    {
        // 기존 정리
        foreach (Transform child in arrowContainer)
            Destroy(child.gameObject);
        arrowImages.Clear();

        // 새로 생성
        foreach (var key in keySequence)
        {
            GameObject arrow = Instantiate(arrowPrefab, arrowContainer);
            Image img = arrow.GetComponent<Image>();
            img.sprite = GetArrowSprite(key);
            img.color = Color.white;
            arrowImages.Add(img);
        }
    }

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

    private bool IsArrowKeyDown(out KeyCode pressed)
    {
        // 어떤 방향키가 눌렸는지 판별
        foreach (var k in ARROWS)
        {
            if (Input.GetKeyDown(k))
            {
                pressed = k;
                return true;
            }
        }
        pressed = KeyCode.None;
        return false;
    }

    void Update()
    {
        if (!isPlaying) return;
        if (currentIndex >= keySequence.Count) return;

        if (IsArrowKeyDown(out KeyCode pressed))
        {
            if (pressed == keySequence[currentIndex])
            {
                arrowImages[currentIndex].color = correctColor;
                currentIndex++;

                if (currentIndex >= keySequence.Count)
                {
                    OnSuccess();
                }
            }
            else
            {
                Debug.Log("오답 입력 틀림");
                StartCoroutine(FlashWrongAndRestart(currentIndex));
            }
        }
    }

    private IEnumerator FlashWrongAndRestart(int index)
    {
        if (index >= 0 && index < arrowImages.Count)
        {
            arrowImages[index].color = wrongColor;
            yield return new WaitForSeconds(wrongFlashDuration);
        }
        RestartSequence();
    }

    private void RestartSequence()
    {
        currentIndex = 0;
        for (int i = 0; i < arrowImages.Count; i++)
            arrowImages[i].color = Color.white;
    }

    private void OnSuccess()
    {
        isPlaying = false;
        gameObject.SetActive(false);
        onDustCleaned?.Invoke(currentDustObject); // 먼지 제거 콜백(Manager에서 처리)
    }
}
