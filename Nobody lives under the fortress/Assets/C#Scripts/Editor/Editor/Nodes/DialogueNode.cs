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
        public string DialogueText;
        public string GUID;
        public string Type;
        public int Trial;
        public string Bg;
        public string Sound;
        public string Music;
        public bool EntyPoint = false;
    }
}