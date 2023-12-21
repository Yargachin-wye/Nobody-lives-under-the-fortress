using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueNodeData
{
    public int Id;
    public List<int> OutIds;
    public bool IsRepeatable;
    public string DialogueText;
    public NodeType Type;
    public string[] Stipulations;
    public int Trial;
    public string Gift;
    public string Bg;
    public string Sound;
    public string Music;
    public Vector2 Position;
}
