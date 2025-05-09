using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private Text speakerText;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Image portraitImage;  // �ʻ�ȭ ǥ�ÿ�

    public void ShowDialogue(string speaker, string dialogue, string portraitName)
    {
        speakerText.text = speaker;
        dialogueText.text = dialogue;

        // �ʻ�ȭ �ε�
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
                Debug.LogError("�ʻ�ȭ�� ã�� �� �����ϴ�: " + portraitName);
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
