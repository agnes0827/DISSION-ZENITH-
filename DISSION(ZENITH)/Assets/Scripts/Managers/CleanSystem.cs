using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CleanSystem : MonoBehaviour
{
    public Image rightArrowImage; // ������ ����Ű �̹���
    public Image leftArrowImage; // ���� ����Ű �̹���
    public Image dustImage; // ���� �̹���

    private Color originalColor; // ���� �� ����
    private Color pressedColor = new Color(0x6F / 255f, 0x4B / 255f, 0x4B / 255f); // #6F4B4B

    private List<string> inputHistory = new List<string>();
    private string[] pattern1 = { "Left", "Right", "Left", "Right", "Left", "Right", "Left", "Right", "Left", "Right" };
    private string[] pattern2 = { "Right", "Left", "Right", "Left", "Right", "Left", "Right", "Left", "Right", "Left" };


    void Start()
    {
        if (rightArrowImage != null)
        {
            originalColor = rightArrowImage.color; // ������ ����Ű������ �� ���� �� ����
        }
        if (leftArrowImage != null)
        {
            originalColor = rightArrowImage.color; // ���� ����Ű ������ �� ���� �� ����
        }
    }

    void Update()
    {
        // ������ ����Ű
        if (rightArrowImage != null)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                rightArrowImage.color = pressedColor;
                AddInput("Right");
            }

            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                rightArrowImage.color = originalColor;
            }
        }

        // ���� ����Ű
        if (leftArrowImage != null)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                leftArrowImage.color = pressedColor;
                AddInput("Left");
            }
                
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                leftArrowImage.color = originalColor;
            }  
        }
    }

    void AddInput(string direction)
    {
        inputHistory.Add(direction);

        // �Է� ����� 8���� ������ �տ��� ����
        if (inputHistory.Count > 8)
            inputHistory.RemoveAt(0);

        // �Է��� 8���� �� ���� ��
        if (inputHistory.Count == 8)
        {
            bool match1 = true, match2 = true;
            for (int i = 0; i < 8; i++)
            {
                if (inputHistory[i] != pattern1[i]) match1 = false;
                if (inputHistory[i] != pattern2[i]) match2 = false;
            }

            if (match1 || match2)
            {
                // ���� �̹��� ����
                if (dustImage != null)
                    dustImage.gameObject.SetActive(false);

                // �Է� ��� �ʱ�ȭ
                inputHistory.Clear();
            }
        }
    }
}
