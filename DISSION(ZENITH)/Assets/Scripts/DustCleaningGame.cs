using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DustCleaningGame : MonoBehaviour
{
    [Header("UI ����")]
    public GameObject arrowPrefab;             // ����Ű ������ ������
    public Transform arrowContainer;           // ����Ű �����ܵ��� ���� �θ�

    private List<KeyCode> keySequence = new List<KeyCode>();
    private int currentIndex = 0;
    private System.Action<GameObject> onDustCleaned;
    private GameObject currentDustObject;
    private bool isPlaying = false;

    // PNG ���
    private const string ICON_PATH = "UI/icon/";

    // �̴ϰ��� ����
    public void BeginGame(GameObject dustObject, System.Action<GameObject> onComplete)
    {
        currentDustObject = dustObject;
        onDustCleaned = onComplete;

        isPlaying = true;
        gameObject.SetActive(true);

        GenerateRandomSequence();
        CreateArrowUI();
    }

    // ���� ������ ����
    private void GenerateRandomSequence()
    {
        keySequence.Clear();
        KeyCode[] arrows = { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };

        int arrowCount = Random.Range(4, 6); // 4~5�� ����
        for (int i = 0; i < arrowCount; i++)
        {
            keySequence.Add(arrows[Random.Range(0, arrows.Length)]);
        }

        currentIndex = 0;
    }

    // ����Ű UI ����
    private void CreateArrowUI()
    {
        // ���� ȭ��ǥ ����
        foreach (Transform child in arrowContainer)
            Destroy(child.gameObject);

        // ���� ����
        foreach (var key in keySequence)
        {
            GameObject arrow = Instantiate(arrowPrefab, arrowContainer);
            Image img = arrow.GetComponent<Image>();
            img.sprite = GetArrowSprite(key);
        }
    }

    // PNG ��θ� ������� ��������Ʈ �ε�
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

                // ��� �Է� �Ϸ�
                if (currentIndex >= keySequence.Count)
                    OnSuccess();
            }
            else if (Input.anyKeyDown)
            {
                // ���� �� �ٽ� ����
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

        // ���� ���� �ݹ� ����
        onDustCleaned?.Invoke(currentDustObject);
    }
}
