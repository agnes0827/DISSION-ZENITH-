using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    // �⺻ ��ȭ UI
    [SerializeField] private Text speakerText;     // ȭ��(ĳ���� �̸�)
    [SerializeField] private Text dialogueText;    // ��� �ؽ�Ʈ
    [SerializeField] private Image portraitImage;  // �ʻ�ȭ �̹���

    // ������ UI
    public GameObject choicePanel;
    public Button choiceButton1;
    public Button choiceButton2;
    public Text choiceText1;
    public Text choiceText2;

    // Ÿ��(Ÿ����) ȿ��: �� ���ھ� ���
    [SerializeField] private float typingSpeed = 0.05f;     // Ÿ���� �ӵ�(1���� ��� ����)
    private Coroutine typingCoroutine;                      // Ÿ�� ȿ�� �ڷ�ƾ
    public bool IsTyping { get; private set; } = false;     // Ÿ�� ȿ�� ���� �� ����
    private string currentFullText = "";                    // ��ü ����(FullText) ����

    // ȭ��, ���, �ʻ�ȭ ���� �̸� UI ǥ��
    public void ShowDialogue(string speaker, string dialogue, string portraitName)
    {
        speakerText.text = speaker;
        currentFullText = dialogue;

        // Ÿ�� ȿ���� ���� ���� ��� ����
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentence(dialogue)); // Ÿ�� ȿ�� ����

        // �ʻ�ȭ ���� �Լ�
        SetPortrait(portraitName);
    }

    // �ʻ�ȭ ����
    private void SetPortrait(string portraitName)
    {
        if (string.IsNullOrEmpty(portraitName))
        {
            portraitImage.enabled = false;
            return;
        }

        // Portraits �������� ������ �̹��� �ε�
        var portrait = Resources.Load<Sprite>("Portraits/" + portraitName);
        portraitImage.enabled = portrait != null;

        if (portrait != null)
            portraitImage.sprite = portrait;
        else Debug.LogError("�ʻ�ȭ�� ã�� �� �����ϴ�: " + portraitName);
    }

    // Ÿ�� ȿ�� �ڷ�ƾ
    private IEnumerator TypeSentence(string sentence)
    {
        IsTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
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

            dialogueText.text = currentFullText;
            IsTyping = false;
        }
    }

    public void ShowDialoguePanel()
    {
        gameObject.SetActive(true);
    }

    public void HideDialogue()
    {
        gameObject.SetActive(false);
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
}