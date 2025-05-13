using UnityEngine;
using UnityEngine.UI;

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

    public void ShowDialogue(string speaker, string dialogue, string portraitName)
    {
        speakerText.text = speaker;
        dialogueText.text = dialogue;

        // 초상화 로드
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