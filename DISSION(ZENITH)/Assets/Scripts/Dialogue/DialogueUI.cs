using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private Text speakerText;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Image portraitImage;  // 초상화 표시용

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
}
