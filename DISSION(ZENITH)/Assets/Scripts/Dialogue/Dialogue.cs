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

    public Dialogue(string id, string speaker, string dialogue, string nextId)
    {
        this.id = id;
        this.speaker = speaker;
        this.dialogue = dialogue;
        this.nextId = nextId;
    }
}

//[System.Serializable]
//public class Dialogue
//{

//    [Tooltip("캐릭터 이름")]
//    public string name;

//    [Tooltip("대사 내용")]
//    public string[] contexts;

//    [Tooltip("이벤트 번호")]
//    public string[] eventnum;

//    [Tooltip("스킵라인")]
//    public string[] skipnum;
//    //[Tooltip("이벤트번호")]
//    //public string[] number;

//}

//[System.Serializable]
//public class DialogueEvent
//{

//    //이벤트 이름
//    public string name;

//    //public Vector2 line;
//    public Dialogue[] dialgoues;
//}
