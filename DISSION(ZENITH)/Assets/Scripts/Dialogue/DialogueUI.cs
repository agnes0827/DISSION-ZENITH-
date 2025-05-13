using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private Text speakerText;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Image portraitImage;  // 초상화 표시용

    public GameObject choicePanel;
    public Button choiceButton1;
    public Button choiceButton2;
    public Text choiceText1;
    public Text choiceText2;

    [SerializeField] private float typingSpeed = 0.05f;
    private Coroutine typingCoroutine;
    public bool IsTyping { get; private set; } = false;
    private string currentFullText = "";

    public void ShowDialogue(string speaker, string dialogue, string portraitName)
    {
        speakerText.text = speaker;
        currentFullText = dialogue;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentence(dialogue));

        if (!string.IsNullOrEmpty(portraitName))
        {
            Sprite portrait = Resources.Load<Sprite>("Portraits/" + portraitName);
            if (portrait != null)
            {
                portraitImage.sprite = portrait;
                portraitImage.enabled = true;
            }
            else
            {
                Debug.LogError("초상화를 찾을 수 없습니다: " + portraitName);
                portraitImage.enabled = false;
            }
        }
    }

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

    // 외부에서 즉시 출력 요청 시 사용
    public void FinishTypingImmediately()
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