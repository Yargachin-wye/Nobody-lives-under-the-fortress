using System;
using System.Linq;

namespace Subtegral.DialogueSystem.DataContainers
{
    [Serializable]
    public class NodeLinkData
    {
        public int BaseNodeGUID;
        public int TargetNodeGUID;
    }
}