using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInventory : MonoBehaviour
{
    public GameObject inventory; // inventory 객체 여기에 넣기
    private bool activated; // inventory 화면 활성화/비활성화

    public void Continue()
    {
        activated = false; // activated 상태 변경
        inventory.SetActive(false); // inventory 화면 비활성화
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) // Tab 키 클릭 시
        {
            activated = !activated; // activated 상태 변경

            if (activated)
            {
                inventory.SetActive(true); // inventory 화면 활성화
            }
            else
            {
                inventory.SetActive(false); // inventory 화면 비활성화
            }
        }
    }
}
