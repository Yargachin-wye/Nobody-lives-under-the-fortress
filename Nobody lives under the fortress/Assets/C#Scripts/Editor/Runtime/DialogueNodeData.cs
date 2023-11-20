using System;
using System.Collections.Generic;
using UnityEngine;

namespace Subtegral.DialogueSystem.DataContainers
{
    [Serializable]
    public class DialogueNodeData
    {
        public int Id;
        public List<int> OutIds;
        public string NodeGUID;
        public string DialogueText;
        public string Type;
        public int Trial;
        public string Bg;
        public string Sound;
        public string Music;
        public Vector2 Position;
    }
}