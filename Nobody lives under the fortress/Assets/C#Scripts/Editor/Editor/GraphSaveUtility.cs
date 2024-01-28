using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Subtegral.DialogueSystem.DataContainers;
using UnityEngine.UIElements;

namespace Subtegral.DialogueSystem.Editor
{
    public class GraphSaveUtility
    {
        const string GRAPH_FILE_NAME = "Graph";
        private List<Edge> Edges => _graphView.edges.ToList();
        private List<DialogueNode> Nodes => _graphView.nodes.ToList().Cast<DialogueNode>().ToList();

        private List<Group> CommentBlocks =>
            _graphView.graphElements.ToList().Where(x => x is Group).Cast<Group>().ToList();

        private DialogueContainer _dialogueContainer;
        private StoryGraphView _graphView;

        
    public static GraphSaveUtility GetInstance(StoryGraphView graphView)
        {
            return new GraphSaveUtility
            {
                _graphView = graphView
            };
        }
        DialogueNodeData StartNodeData = new DialogueNodeData()
        {
            Id = 0,
            IsRepeatable = true,
            DialogueText = "STARTNODE",
            Type = NodeType.Text,
            Stipulations = new string[0],
            Trial = -1,
            Bg = "firstBG",
            Sound = "",
            Music = "firstMusic",
        };
        public void SaveGraph(string fileName)
        {
            
            _SaveGraph(fileName);
        }

        private void _SaveGraph(string fileName)
        {
            var dialogueContainerObject = ScriptableObject.CreateInstance<DialogueContainer>();
            if (!SaveNodes(dialogueContainerObject)) return;
            SaveExposedProperties(dialogueContainerObject);
            // SaveCommentBlocks(dialogueContainerObject);

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            UnityEngine.Object loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/Resources/{fileName}.asset", typeof(DialogueContainer));

            if (loadedAsset == null || !AssetDatabase.Contains(loadedAsset))
            {
                AssetDatabase.CreateAsset(dialogueContainerObject, $"Assets/Resources/{fileName}.asset");
            }
            else
            {
                DialogueContainer container = loadedAsset as DialogueContainer;
                container.NodeLinks = dialogueContainerObject.NodeLinks;
                container.DialogueNodeData = dialogueContainerObject.DialogueNodeData;
                container.ExposedProperties = dialogueContainerObject.ExposedProperties;
                container.CommentBlockData = dialogueContainerObject.CommentBlockData;
                EditorUtility.SetDirty(container);
            }
            AssetDatabase.SaveAssets();
        }

        private bool SaveNodes(DialogueContainer dialogueContainerObject)
        {
            if (!Edges.Any()) return false;
            var connectedSockets = Edges.Where(x => x.input.node != null).ToArray();
            int ID = 1;
            foreach (var node in Nodes.Where(node => !node.EntryPoint))
            {
                node.Id = ID;
                dialogueContainerObject.DialogueNodeData.Add(new DialogueNodeData
                {
                    Id = node.Id,
                    IsRepeatable = node.IsRepeatable,
                    DialogueText = node.DialogueText,
                    Type = node.Type,
                    Stipulations = node.Stipulations.ToArray(),
                    Trial = node.Trial,
                    Gift = node.Gift,
                    Bg = node.Bg,
                    Sound = node.Sound,
                    Music = node.Music,
                    Position = node.GetPosition().position
                });
                ID++;
            }
            dialogueContainerObject.DialogueNodeData = dialogueContainerObject.DialogueNodeData.OrderBy(node => node.Id).ToList();

            Dictionary<int, List<int>> targetNodesByBaseNode = new Dictionary<int, List<int>>();
            for (var i = 0; i < connectedSockets.Count(); i++)
            {
                var outputNode = (connectedSockets[i].output.node as DialogueNode);
                var inputNode = (connectedSockets[i].input.node as DialogueNode);
                int outputNodeId = outputNode.EntryPoint ? 0 : outputNode.Id;

                dialogueContainerObject.NodeLinks.Add(new NodeLinkData
                {
                    BaseNodeGUID = outputNodeId,
                    TargetNodeGUID = inputNode.Id
                });
                if (!targetNodesByBaseNode.ContainsKey(outputNodeId))
                {
                    targetNodesByBaseNode[outputNodeId] = new List<int>();
                }
                targetNodesByBaseNode[outputNodeId].Add(inputNode.Id);
            }
            List<DialogueNodeData> newData = new List<DialogueNodeData>(dialogueContainerObject.DialogueNodeData);
            newData.Insert(0, StartNodeData);
            foreach (var obj in newData)
            {
                if (targetNodesByBaseNode.ContainsKey(obj.Id))
                {
                    obj.OutIds = targetNodesByBaseNode[obj.Id];
                }
            }

            DataContainer dataContainer = new DataContainer(newData);
            string json = JsonUtility.ToJson(dataContainer);
            File.WriteAllText(Path.Combine(Application.dataPath, "Resources/" + GRAPH_FILE_NAME + ".json"), json);
            AssetDatabase.ImportAsset("Assets/Resources/" + GRAPH_FILE_NAME + ".json");
            return true;
        }
        private void SaveExposedProperties(DialogueContainer dialogueContainer)
        {
            dialogueContainer.ExposedProperties.Clear();
            dialogueContainer.ExposedProperties.AddRange(_graphView.ExposedProperties);
        }

        public void LoadNarrative(string fileName)
        {
            _dialogueContainer = Resources.Load<DialogueContainer>(fileName);
            if (_dialogueContainer == null)
            {
                EditorUtility.DisplayDialog("File Not Found", "Target Narrative Data does not exist!", "OK");
                return;
            }

            _graphView.stipulationPool = _dialogueContainer.stipulationPool;
            ClearGraph();
            GenerateDialogueNodes();
            AddExposedProperties();
            ConnectDialogueNodes();
        }

        /// <summary>
        /// Set Entry point GUID then Get All Nodes, remove all and their edges. Leave only the entrypoint node. (Remove its edge too)
        /// </summary>
        private void ClearGraph()
        {
            Nodes.Find(x => x.EntryPoint).Id = _dialogueContainer.NodeLinks[0].BaseNodeGUID;
            foreach (var perNode in Nodes)
            {
                if (perNode.EntryPoint) continue;
                Edges.Where(x => x.input.node == perNode).ToList()
                    .ForEach(edge => _graphView.RemoveElement(edge));
                _graphView.RemoveElement(perNode);
            }
            _graphView.SetNextNodeIDToZero();
        }

        /// <summary>
        /// Create All serialized nodes and assign their guid and dialogue text to them
        /// </summary>
        private void GenerateDialogueNodes()
        {
            foreach (var perNode in _dialogueContainer.DialogueNodeData)
            {
                var tempNode = _graphView.CreateNode(perNode.Type, perNode.IsRepeatable, perNode.DialogueText, perNode.Stipulations.ToList(), perNode.Trial, perNode.Gift, perNode.Bg, perNode.Sound, perNode.Music, Vector2.zero);
                tempNode.Id = perNode.Id;
                _graphView.AddElement(tempNode);

                var nodePorts = _dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == perNode.Id).ToList();
                nodePorts.ForEach(x => _graphView.AddChoicePort(tempNode));
            }
        }

        private void ConnectDialogueNodes()
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                var k = i; //Prevent access to modified closure
                var connections = _dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == Nodes[k].Id).ToList();
                for (var j = 0; j < connections.Count(); j++)
                {
                    var targetNodeGUID = connections[j].TargetNodeGUID;
                    var targetNode = Nodes.First(x => x.Id == targetNodeGUID);
                    LinkNodesTogether(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);

                    targetNode.SetPosition(new Rect(
                        _dialogueContainer.DialogueNodeData.First(x => x.Id == targetNodeGUID).Position,
                        _graphView.DefaultNodeSize));
                }
            }
        }

        private void LinkNodesTogether(Port outputSocket, Port inputSocket)
        {
            var tempEdge = new Edge()
            {
                output = outputSocket,
                input = inputSocket
            };
            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            _graphView.Add(tempEdge);
        }

        private void AddExposedProperties()
        {
            _graphView.ClearBlackBoardAndExposedProperties();
            foreach (var exposedProperty in _dialogueContainer.ExposedProperties)
            {
                _graphView.AddPropertyToBlackBoard(exposedProperty);
            }
        }
    }
    
}