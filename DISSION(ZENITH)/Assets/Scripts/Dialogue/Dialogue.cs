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

//    [Tooltip("ĳ���� �̸�")]
//    public string name;

//    [Tooltip("��� ����")]
//    public string[] contexts;

//    [Tooltip("�̺�Ʈ ��ȣ")]
//    public string[] eventnum;

//    [Tooltip("��ŵ����")]
//    public string[] skipnum;
//    //[Tooltip("�̺�Ʈ��ȣ")]
//    //public string[] number;

//}

//[System.Serializable]
//public class DialogueEvent
//{

//    //�̺�Ʈ �̸�
//    public string name;

//    //public Vector2 line;
//    public Dialogue[] dialgoues;
//}
