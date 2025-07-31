using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CleanSystem : MonoBehaviour
{
    public Image rightArrowImage; // 오른쪽 방향키 이미지
    public Image leftArrowImage; // 왼쪽 방향키 이미지
    public Image dustImage; // 먼지 이미지

    private Color originalColor; // 원래 색 저장
    private Color pressedColor = new Color(0x6F / 255f, 0x4B / 255f, 0x4B / 255f); // #6F4B4B

    private List<string> inputHistory = new List<string>();
    private string[] pattern1 = { "Left", "Right", "Left", "Right", "Left", "Right", "Left", "Right", "Left", "Right" };
    private string[] pattern2 = { "Right", "Left", "Right", "Left", "Right", "Left", "Right", "Left", "Right", "Left" };


    void Start()
    {
        if (rightArrowImage != null)
        {
            originalColor = rightArrowImage.color; // 오른쪽 방향키시작할 때 원래 색 저장
        }
        if (leftArrowImage != null)
        {
            originalColor = rightArrowImage.color; // 왼쪽 방향키 시작할 때 원래 색 저장
        }
    }

    void Update()
    {
        // 오른쪽 방향키
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

        // 왼쪽 방향키
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

        // 입력 기록이 8개를 넘으면 앞에서 제거
        if (inputHistory.Count > 8)
            inputHistory.RemoveAt(0);

        // 입력이 8개일 때 패턴 비교
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
                // 먼지 이미지 제거
                if (dustImage != null)
                    dustImage.gameObject.SetActive(false);

                // 입력 기록 초기화
                inputHistory.Clear();
            }
        }
    }
}
