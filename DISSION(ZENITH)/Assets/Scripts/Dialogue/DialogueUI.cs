using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [Header("UI ���� ���")]
    [SerializeField] private GameObject dialogueContainer;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image portraitImage;
    [SerializeField] private GameObject speakerPanel;

    [Header("������ UI")]
    public GameObject choicePanel;
    public Button choiceButton1;
    public Button choiceButton2;
    public TextMeshProUGUI choiceText1;
    public TextMeshProUGUI choiceText2;

    // Ÿ��(Ÿ����) ȿ��: �� ���ھ� ���
    [SerializeField] private float typingSpeed = 0.05f;       // 1���� ��� ����(��)
    private Coroutine typingCoroutine;                        // Ÿ�� ȿ�� �ڷ�ƾ
    public bool IsTyping { get; private set; } = false;       // Ÿ�� ȿ�� ���� �� ����
    private string currentFullText = "";                      // ��ü ����(FullText) ����

    private void Awake()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.RegisterDialogueUI(this);
        }
        else
        {
            Debug.LogError("DialogueManager�� ���� �����ϴ�! UI�� ����� �� �����ϴ�.");
        }
    }

    private void OnDestroy()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.UnregisterDialogueUI();
        }
    }

    public void ShowDialoguePanel()
    {
        dialogueContainer.SetActive(true);
    }

    public void HideDialoguePanel()
    {
        dialogueContainer.SetActive(false);
    }

    // ȭ��, ���, �ʻ�ȭ ���� �̸� UI ǥ��
    public void ShowDialogue(string speaker, string dialogue, string portraitName)
    {
        // �̸��� ��������� �̸� UI �����
        if (string.IsNullOrEmpty(speaker))
        {
            speakerPanel.SetActive(false);
        }
        else
        {
            speakerPanel.SetActive(true);
            speakerText.text = speaker;
        }

        currentFullText = dialogue;

        // Ÿ�� ȿ�� ���̸� ����
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        // TMP ���: ����(�±� ����)�� ���� �����ϰ�, ���� ���� ���� ����
        dialogueText.richText = true;               
        dialogueText.text = currentFullText;        // �±� ���� ���� �� ���� ����
        dialogueText.ForceMeshUpdate();            
        dialogueText.maxVisibleCharacters = 0;     

        typingCoroutine = StartCoroutine(TypeSentenceTMP());

        // �ʻ�ȭ ����
        SetPortrait(portraitName);
    }

    // �ʻ�ȭ ����
    private void SetPortrait(string portraitName)
    {
        if (portraitImage == null)
            return;

        if (string.IsNullOrEmpty(portraitName))
        {
            portraitImage.gameObject.SetActive(false); // �� ���̰�
            return;
        }

        Sprite portrait = Resources.Load<Sprite>("Portraits/" + portraitName);

        if (portrait == null)
        {
            portraitImage.gameObject.SetActive(false); // �ε� ���� �� ����
        }
        else
        {
            portraitImage.sprite = portrait;
            portraitImage.gameObject.SetActive(true);  // ���� �ε� �� ������
        }
    }

    // TMP Ÿ�� ȿ�� �ڷ�ƾ
    private IEnumerator TypeSentenceTMP()
    {
        IsTyping = true;
        int total = dialogueText.textInfo.characterCount;

        if (total == 0)
        {
            yield return null;
            dialogueText.ForceMeshUpdate();
            total = dialogueText.textInfo.characterCount;
        }

        int visible = 0;
        while (visible < total)
        {
            visible++;
            dialogueText.maxVisibleCharacters = visible;
            yield return new WaitForSeconds(typingSpeed);
        }

        IsTyping = false;
    }

    // Ÿ�� ȿ�� �� �����̽� �� �Է� ��,
    // ȿ�� ��� �����ϰ� ��ü ���� ���
    public void FinishTyping()
    {
        if (IsTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            dialogueText.ForceMeshUpdate();
            dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;

            IsTyping = false;
        }
    }

    public void ShowChoices(string c1, string c2)
    {
        choicePanel.SetActive(true);
        choiceText1.text = c1;
        choiceText2.text = c2;
    }

    public void HideChoices()
    {
        choicePanel.SetActive(false);
    }

    public void HighlightChoice(int index)
    {
        Color highlightColor = Color.yellow;
        Color normalColor = Color.white;

        choiceButton1.GetComponent<Image>().color = (index == 1) ? highlightColor : normalColor;
        choiceButton2.GetComponent<Image>().color = (index == 2) ? highlightColor : normalColor;
    }
}
