using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Dialogue
{
    public string id;               // 대화 ID
    public string speaker;          // 화자(캐릭터 이름)
    public string dialogue;         // 대사 텍스트
    public string nextId;           // 다음 대화 ID
    public string portrait;         // 초상화 이미지

    public string choice1;          // 선택지 1
    public string choice1NextId;    // 선택지 1 선택 시 이동할 ID
    public string choice2;          // 선택지 2
    public string choice2NextId;    // 선택지 2 선택 시 이동할 ID

    public string questId;          // 퀘스트 ID

    // 생성자
    public Dialogue(string id, string speaker, string dialogue, string nextId, string portrait,
        string choice1, string choice1NextId, string choice2, string choice2NextId, string questId)
    {
        this.id = id;
        this.speaker = speaker;
        this.dialogue = dialogue;
        this.nextId = nextId;
        this.portrait = portrait;

        this.choice1 = choice1;
        this.choice1NextId = choice1NextId;
        this.choice2 = choice2;
        this.choice2NextId = choice2NextId;

        this.questId = questId;
    }
}
