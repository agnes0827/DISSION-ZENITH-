using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EmotionController : MonoBehaviour
{
    // 1. 싱글톤 인스턴스
    public static EmotionController Instance { get; private set; }

    // 2. 인스펙터에서 연결할 변수
    [Header("UI & Animation")]
    [SerializeField] private Image iconImage;
    [SerializeField] private float scaleAnimationTime = 0.2f;
    [SerializeField] private float thinkingAnimationSpeed = 0.4f;
    [SerializeField] private float iconVisibleDuration = 1.5f;

    [Header("Emotion Sprites")]
    [SerializeField] private Sprite surpriseSprite;  // !
    [SerializeField] private Sprite questionSprite;  // ?
    [SerializeField] private Sprite thinkingSprite1; // .
    [SerializeField] private Sprite thinkingSprite2; // ..
    [SerializeField] private Sprite thinkingSprite3; // ...

    // 현재 실행 중인 감정 표현 코루틴을 저장
    private Coroutine currentEmotionCoroutine;

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 게임 시작 시 아이콘 숨기기
        if (iconImage != null)
        {
            iconImage.gameObject.SetActive(false);
        }
    }

    public void ShowSurprise()
    {
        StartEmotion(AnimateScaleRoutine(surpriseSprite));
    }

    public void ShowQuestion()
    {
        StartEmotion(AnimateScaleRoutine(questionSprite));
    }

    public void ShowThinking()
    {
        StartEmotion(AnimateThinkingRoutine());
    }

    public void HideIcon()
    {
        StopCurrentEmotion();
        iconImage.gameObject.SetActive(false);
    }


    private void StartEmotion(IEnumerator routine)
    {
        StopCurrentEmotion();
        currentEmotionCoroutine = StartCoroutine(routine);
    }

    private void StopCurrentEmotion()
    {
        if (currentEmotionCoroutine != null)
        {
            StopCoroutine(currentEmotionCoroutine);
            currentEmotionCoroutine = null;
        }
    }

    // 스케일링 애니메이션
    private IEnumerator AnimateScaleRoutine(Sprite spriteToShow)
    {
        // 1. 준비
        iconImage.gameObject.SetActive(true);
        iconImage.sprite = spriteToShow;
        iconImage.transform.localScale = Vector3.zero; // 0에서 시작

        // 2. 스케일 업
        float timer = 0f;
        while (timer < scaleAnimationTime)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / scaleAnimationTime);
            iconImage.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
            yield return null; // 다음 프레임까지 대기
        }

        iconImage.transform.localScale = Vector3.one;

        // 3. 유지
        yield return new WaitForSeconds(iconVisibleDuration);

        // 4. 숨기기
        iconImage.gameObject.SetActive(false);
        currentEmotionCoroutine = null; // 코루틴 종료
    }

    // ... 아이콘을 위한 순차 애니메이션
    private IEnumerator AnimateThinkingRoutine()
    {
        // 1. 준비
        iconImage.gameObject.SetActive(true);
        iconImage.transform.localScale = Vector3.one;

        // 2. . (점 1개)
        iconImage.sprite = thinkingSprite1;
        yield return new WaitForSeconds(thinkingAnimationSpeed);

        // 3. .. (점 2개)
        iconImage.sprite = thinkingSprite2;
        yield return new WaitForSeconds(thinkingAnimationSpeed);

        // 4. ... (점 3개)
        iconImage.sprite = thinkingSprite3;
        yield return new WaitForSeconds(iconVisibleDuration); // 마지막 아이콘 유지

        // 5. 숨기기
        iconImage.gameObject.SetActive(false);
        currentEmotionCoroutine = null; // 코루틴 종료
    }
}