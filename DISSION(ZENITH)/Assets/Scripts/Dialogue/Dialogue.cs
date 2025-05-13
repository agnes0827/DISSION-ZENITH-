using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Dialogue
{
    public string id;
    public string speaker;
    public string dialogue;
    public string nextId;
    public string portrait;

    public string choice1;
    public string choice1NextId;
    public string choice2;
    public string choice2NextId;

    public Dialogue(string id, string speaker, string dialogue, string nextId, string portrait,
        string choice1 = "", string choice1NextId = "", string choice2 = "", string choice2NextId = "")
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
    }
}
