using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


namespace Subtegral.DialogueSystem.Editor
{
    public class DialogueNode : Node
    {
        public int Id;
        public bool IsRepeatable;
        public string DialogueText;
        public NodeType Type;
        public List<string> Stipulations;
        public int Trial;
        public string Gift;
        public string Bg;
        public string Sound;
        public string Music;
        public bool EntyPoint = false;
    }
}