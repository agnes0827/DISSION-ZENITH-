using UnityEngine;
using System.Collections.Generic;

// *** 효과음 추가시 주의사항 ***
// 중간에 끼워 넣지 말고 반드시 맨 아래에 삽입 후
// Manager 프리팹에 있는 인스펙터 리스트에 같은 순서로 효과음을 넣어주세요.
public enum SfxType
{
    Door = 0,               // Element 0
    PageFlip = 1,           // Element 1
    Clock = 2,              // Element 2
    DustClean = 3,          // Element 3
    Fail = 4,               // Element 4
    GameOver1 = 5,          // Element 5
    GameOver2 = 6,          // Element 6
    NewQuest = 7,           // Element 7
    QuestClear = 8,         // Element 8
    Attack1 = 9,            // Element 9
    Attack2 = 10,           // Element 10
    Attack3 = 11,           // Element 11
    Attack4 = 12,           // Element 12
    Hit = 13,               // Element 13
    BattleEncounter = 14,   // Element 14
    EnemyDeath = 15,        // Element 15
    UISound = 16,           // Element 16
    ItemPickup = 17,        // Element 17
    Torch = 18,             // Element 18
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Clips")]
    public List<AudioClip> sfxClips;

    [Header("Settings")]
    public int channels = 10;
    private List<AudioSource> sfxPlayers;
    private int channelIndex;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitAudioPlayers();
    }

    void InitAudioPlayers()
    {
        sfxPlayers = new List<AudioSource>();
        GameObject sfxGroup = new GameObject("SFX_Players");
        sfxGroup.transform.parent = transform;

        for (int i = 0; i < channels; i++)
        {
            AudioSource source = sfxGroup.AddComponent<AudioSource>();
            source.playOnAwake = false;
            sfxPlayers.Add(source);
        }
    }

    public void PlaySFX(SfxType type, float volume = 0.7f, bool randomPitch = true)
    {
        // 해당 Enum 번호에 맞는 클립 찾기
        int index = (int)type;
        if (index >= sfxClips.Count) return;

        AudioSource player = sfxPlayers[channelIndex];
        player.clip = sfxClips[index];
        player.volume = volume;

        // 피치 랜덤화
        if (randomPitch)
            player.pitch = Random.Range(0.9f, 1.1f);
        else
            player.pitch = 1.0f;

        player.Play();

        // 다음 채널로 인덱스 이동
        channelIndex = (channelIndex + 1) % channels;
    }
}