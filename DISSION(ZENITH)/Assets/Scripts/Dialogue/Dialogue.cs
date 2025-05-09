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

    public Dialogue(string id, string speaker, string dialogue, string nextId, string portrait)
    {
        this.id = id;
        this.speaker = speaker;
        this.dialogue = dialogue;
        this.nextId = nextId;
        this.portrait = portrait;
    }
}