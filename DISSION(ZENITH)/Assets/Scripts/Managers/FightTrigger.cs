using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class FightTrigger : MonoBehaviour
{
    [Header("Identity")]
    public string monsterId = "Dust_001";   // 먼지 프리팹 아이디 지정

    [Header("Transition")]
    public string fightSceneName = "Fight";
    public Image fadePanel;
    public float fadeDuration = 1f;

    [Header("Reward")]
    public GameObject bookItemPrefab;       // 처치 후 나타날 책 아이템
    public Transform rewardSpawnPoint;      // 비우면 몬스터 자리 그대로

    private bool _loading = false;
    private bool _replaced = false;

    void Start()
    {
        // 이미 처치된 몬스터면 → 자신 숨기고 보상 아이템만 남기기
        if (LibraryGameState.Instance != null && LibraryGameState.Instance.IsDefeated(monsterId))
        {
            ReplaceWithReward();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_loading) return;
        if (!other.CompareTag("Player")) return;
        _loading = true;

        // 이번 전투가 어떤 몬스터에서 시작됐는지 기록
        if (LibraryGameState.Instance != null)
            LibraryGameState.Instance.lastMonsterId = monsterId;

        // 페이드 후 전투 씬으로
        if (fadePanel != null)
            StartCoroutine(FadeAndLoad());
        else
            SceneManager.LoadScene(fightSceneName);
    }

    private IEnumerator FadeAndLoad()
    {
        float t = 0f;
        Color c = fadePanel.color;
        fadePanel.gameObject.SetActive(true);

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime; // 타임스케일 0이어도 동작
            c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }

        SceneManager.LoadScene(fightSceneName);
    }

    private void ReplaceWithReward()
    {
        if (_replaced) return;
        _replaced = true;

        Vector3 pos = rewardSpawnPoint ? rewardSpawnPoint.position : transform.position;
        Quaternion rot = rewardSpawnPoint ? rewardSpawnPoint.rotation : Quaternion.identity;

        if (bookItemPrefab != null)
            Instantiate(bookItemPrefab, pos, rot, transform.parent); // 씬에 드랍

        gameObject.SetActive(false); // 몬스터 숨김
    }
}