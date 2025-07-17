using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Dialogue
{
    public string id;               // ��ȭ ID
    public string speaker;          // ȭ��(ĳ���� �̸�)
    public string dialogue;         // ��� �ؽ�Ʈ
    public string nextId;           // ���� ��ȭ ID
    public string portrait;         // �ʻ�ȭ �̹���

    public string choice1;          // ������ 1
    public string choice1NextId;    // ������ 1 ���� �� �̵��� ID
    public string choice2;          // ������ 2
    public string choice2NextId;    // ������ 2 ���� �� �̵��� ID

    public string questId;          // ����Ʈ ID

    // ������
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
