using UnityEngine;
using System.Collections;

public class QuestIconUI : MonoBehaviour
{
    public string questId;
    private GameObject iconObject;
    private SpriteRenderer spriteRenderer;
    private Coroutine animationCoroutine;

    void Start()
    {
        CreateIcon();
        UpdateIcon();
    }

    void CreateIcon()
    {
        Sprite iconSprite = Resources.Load<Sprite>("UI/icon/new_quest");
        if (iconSprite == null)
        {
            Debug.LogError("퀘스트 아이콘을 찾을 수 없습니다!");
            return;
        }

        GameObject icon = new GameObject("QuestIcon");
        icon.transform.SetParent(this.transform);
        icon.transform.localPosition = new Vector3(0, 0.8f, 0);   // 위치 조정
        icon.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);   // 크기 조정

        spriteRenderer = icon.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = iconSprite;
        spriteRenderer.sortingOrder = 10;

        iconObject = icon;
    }

    public void UpdateIcon()
    {
        if (QuestManager.Instance == null || string.IsNullOrEmpty(questId)) return;

        bool show = !QuestManager.Instance.HasAccepted(questId) && !QuestManager.Instance.HasCompleted(questId);

        if (iconObject != null)
        {
            iconObject.SetActive(show);

            if (show && animationCoroutine == null)
                animationCoroutine = StartCoroutine(BlinkAndWobble());
            else if (!show && animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
                animationCoroutine = null;
                iconObject.transform.localPosition = new Vector3(0, 1.0f, 0);
                spriteRenderer.color = Color.white;
            }
        }
    }

    IEnumerator BlinkAndWobble()
    {
        float timer = 0f;
        Vector3 originalPos = iconObject.transform.localPosition;
        Color originalColor = spriteRenderer.color;

        while (true)
        {
            timer += Time.deltaTime;

            // 깜빡임
            float alpha = Mathf.Abs(Mathf.Sin(timer * 2f)) * 0.5f + 0.5f;  // 0.5~1.0 사이
            spriteRenderer.color = new Color(1, 1, 1, alpha);

            // 흔들림
            float wobbleY = Mathf.Sin(timer * 5f) * 0.03f;
            iconObject.transform.localPosition = originalPos + new Vector3(0, wobbleY, 0);

            yield return null;
        }
    }
}
