using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeTrigger : MonoBehaviour
{
    [Header("이 나무에서 사용할 미니게임")]
    public FruitTimingMiniGame miniGame;   // Canvas 밑에 있는 미니게임 오브젝트

    private bool used = false;  // 이미 열매를 땄는지 여부

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (used) return;
        if (!collision.CompareTag("Player")) return;

        // 이미 켜져 있으면 중복 실행 막기
        if (miniGame.gameObject.activeSelf) return;

        // 미니게임 성공 시 호출될 콜백 등록
        miniGame.onSuccess = OnMiniGameSuccess;

        // 미니게임 켜고 시작
        miniGame.gameObject.SetActive(true);
        miniGame.StartMiniGame();
    }

    private void OnMiniGameSuccess()
    {
        used = true;  // 이 나무는 더 이상 사용 X
    }
}
