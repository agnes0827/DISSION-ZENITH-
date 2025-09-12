using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class FightTrigger : MonoBehaviour
{
    [Header("Identity")]
    public string monsterId = "Dust_001";   // ���� ������ ���̵� ����

    [Header("Transition")]
    public string fightSceneName = "Fight";
    public Image fadePanel;
    public float fadeDuration = 1f;

    [Header("Reward")]
    public GameObject bookItemPrefab;       // óġ �� ��Ÿ�� å ������
    public Transform rewardSpawnPoint;      // ���� ���� �ڸ� �״��

    private bool _loading = false;
    private bool _replaced = false;

    void Start()
    {
        // �̹� óġ�� ���͸� �� �ڽ� ����� ���� �����۸� �����
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

        // �̹� ������ � ���Ϳ��� ���۵ƴ��� ���
        if (LibraryGameState.Instance != null)
            LibraryGameState.Instance.lastMonsterId = monsterId;

        // ���̵� �� ���� ������
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
            t += Time.unscaledDeltaTime; // Ÿ�ӽ����� 0�̾ ����
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
            Instantiate(bookItemPrefab, pos, rot, transform.parent); // ���� ���

        gameObject.SetActive(false); // ���� ����
    }
}