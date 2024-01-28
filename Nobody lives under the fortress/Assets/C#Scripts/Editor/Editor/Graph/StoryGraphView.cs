using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Subtegral.DialogueSystem.DataContainers;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Subtegral.DialogueSystem.Editor
{
    public class StoryGraphView : GraphView
    {
        StyleSheet TextNodeStyle = Resources.Load<StyleSheet>("TextNode");
        StyleSheet TrialNodeStyle = Resources.Load<StyleSheet>("TrialNode");
        StyleSheet TrialLoseNodeStyle = Resources.Load<StyleSheet>("TrialLoseNode");
        StyleSheet TrialWinNodeStyle = Resources.Load<StyleSheet>("TrialWinNode");

        public readonly Vector2 DefaultNodeSize = new Vector2(200, 150);
        public readonly Vector2 DefaultCommentBlockSize = new Vector2(300, 200);
        public DialogueNode EntryPointNode;
        public Blackboard Blackboard = new Blackboard();
        public List<ExposedProperty> ExposedProperties { get; private set; } = new List<ExposedProperty>();
        private NodeSearchWindow _searchWindow;
        private int NextNodeID = 0;

        List<string> TypesText = new List<string>();
        StoryGraph _editorWindow;

        public StipulationPool stipulationPool;
        public void SetNextNodeIDToZero()
        {
            // NextNodeID = 0;
        }
        public StoryGraphView(StoryGraph editorWindow)
        {
            for (int i = 0; i < Enum.GetNames(typeof(NodeType)).Length; i++)
            {
                TypesText.Add(Enum.GetName(typeof(NodeType), i));
            }

            styleSheets.Add(Resources.Load<StyleSheet>("NarrativeGraph"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddElement(GetEntryPointNodeInstance());

            AddSearchWindow(editorWindow);
            _editorWindow = editorWindow;
        }

        private void AddSearchWindow(StoryGraph editorWindow)
        {
            _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            _searchWindow.Configure(editorWindow, this);
            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }


        public void ClearBlackBoardAndExposedProperties()
        {
            ExposedProperties.Clear();
            Blackboard.Clear();
        }

        public Group CreateCommentBlock(Rect rect, CommentBlockData commentBlockData = null)
        {
            if (commentBlockData == null)
                commentBlockData = new CommentBlockData();
            var group = new Group
            {
                autoUpdateGeometry = true,
                title = commentBlockData.Title
            };
            AddElement(group);
            group.SetPosition(rect);
            return group;
        }

        public void AddPropertyToBlackBoard(ExposedProperty property, bool loadMode = false)
        {
            var localPropertyName = property.PropertyName;
            var localPropertyValue = property.PropertyValue;
            if (!loadMode)
            {
                while (ExposedProperties.Any(x => x.PropertyName == localPropertyName))
                    localPropertyName = $"{localPropertyName}(1)";
            }

            var item = ExposedProperty.CreateInstance();
            item.PropertyName = localPropertyName;
            item.PropertyValue = localPropertyValue;
            ExposedProperties.Add(item);

            var container = new VisualElement();
            var field = new BlackboardField { text = localPropertyName, typeText = "string" };
            container.Add(field);

            var propertyValueTextField = new TextField("Value:")
            {
                value = localPropertyValue
            };
            propertyValueTextField.RegisterValueChangedCallback(evt =>
            {
                var index = ExposedProperties.FindIndex(x => x.PropertyName == item.PropertyName);
                ExposedProperties[index].PropertyValue = evt.newValue;
            });
            var sa = new BlackboardRow(field, propertyValueTextField);
            container.Add(sa);
            Blackboard.Add(container);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            var startPortView = startPort;

            ports.ForEach((port) =>
            {
                var portView = port;
                if (startPortView != portView && startPortView.node != portView.node)
                    compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        public void CreateNewDialogueNode(NodeType nodeName, bool isRepeatable, string text, List<string> stipulations, int trial, string gift, string bg, string sound, string music, Vector2 position)
        {
            DialogueNode node = CreateNode(nodeName, isRepeatable, text, stipulations, trial, gift, bg, sound, music, position);
            AddElement(node);
        }

        public DialogueNode CreateNode(NodeType nodeName, bool isRepeatable, string text, List<string> stipulations, int trial, string gift, string bg, string sound, string music, Vector2 position)
        {
            var tempDialogueNode = new DialogueNode()
            {
                /*title = nodeName,*/
                Id = this.nodes.ToList().Cast<DialogueNode>().ToList().Count,
                IsRepeatable = isRepeatable,
                DialogueText = text,
                Type = nodeName,
                Stipulations = stipulations,
                Trial = trial,
                Gift = gift,
                Bg = bg,
                Sound = sound,
                Music = music
            };
            tempDialogueNode.title = tempDialogueNode.Id + " " + nodeName;
            NextNodeID += 1;
            SetStyle(tempDialogueNode);

            var inputPort = GetPortInstance(tempDialogueNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            tempDialogueNode.inputContainer.Add(inputPort);

            tempDialogueNode.RefreshExpandedState();
            tempDialogueNode.RefreshPorts();
            tempDialogueNode.SetPosition(new Rect(position,
                DefaultNodeSize)); //To-Do: implement screen center instantiation positioning

            CreateCheckboxElement(tempDialogueNode.IsRepeatable, tempDialogueNode, evt =>
            {
                tempDialogueNode.IsRepeatable = evt;
            });

            CreateTextElement("Text:", tempDialogueNode, tempDialogueNode.DialogueText, evt =>
            {
                tempDialogueNode.DialogueText = evt.newValue;
            });


            CreateChoiceElement("Выберите вариант:", tempDialogueNode, Enum.GetName(typeof(NodeType), tempDialogueNode.Type), evt =>
            {
                tempDialogueNode.Type = (NodeType)Enum.Parse(typeof(NodeType), evt.newValue);
                tempDialogueNode.title = tempDialogueNode.Id + " " + evt.newValue;
                //SetStyle(tempDialogueNode);
            });
            if (tempDialogueNode.Type == NodeType.Trial)
            {
                CreateTextElement("Trial:", tempDialogueNode, tempDialogueNode.Trial.ToString(), evt =>
                {
                    int trail;
                    tempDialogueNode.Trial = int.TryParse(evt.newValue, out trail) ? trail : -1;
                });
            }
            else if (tempDialogueNode.Type == NodeType.Stipulation)
            {
                CreateCheckboxElements(stipulations, tempDialogueNode, evt =>
                {
                    tempDialogueNode.Stipulations = evt;
                });
            }
            else if (tempDialogueNode.Type == NodeType.Gift)
            {
                CreateCheckboxElements(new List<string> { tempDialogueNode.Gift }, tempDialogueNode, evt =>
                {
                    tempDialogueNode.Gift = evt.ToArray()[0];
                });
            }

            CreateTextElement("BG:", tempDialogueNode, tempDialogueNode.Bg, evt =>
            {
                tempDialogueNode.Bg = evt.newValue;
            });
            CreateTextElement("Sound:", tempDialogueNode, tempDialogueNode.Sound, evt =>
            {
                tempDialogueNode.Sound = evt.newValue;
            });
            CreateTextElement("Music:", tempDialogueNode, tempDialogueNode.Music, evt =>
            {
                tempDialogueNode.Music = evt.newValue;
            });

            var button = new Button(() => { AddChoicePort(tempDialogueNode); })
            {
                text = "Add Choice"
            };
            tempDialogueNode.titleButtonContainer.Add(button);
            return tempDialogueNode;
        }
        private void SetStyle(DialogueNode tempDialogueNode)
        {
            // tempDialogueNode.styleSheets.Clear();
            switch (tempDialogueNode.Type)
            {
                case NodeType.Text:
                    tempDialogueNode.styleSheets.Add(TextNodeStyle);
                    break;
                case NodeType.Trial:
                    tempDialogueNode.styleSheets.Add(TrialNodeStyle);
                    break;
                case NodeType.TrialLose:
                    tempDialogueNode.styleSheets.Add(TrialLoseNodeStyle);
                    break;
                case NodeType.TrialWin:
                    tempDialogueNode.styleSheets.Add(TrialWinNodeStyle);
                    break;
                case NodeType.End:
                    tempDialogueNode.styleSheets.Add(TrialWinNodeStyle);
                    break;
            }
        }
        private void CreateCheckboxElement(bool isRepeatable, DialogueNode node, Action<bool> onCheckboxValueChanged)
        {
            var checkboxElement = new VisualElement();
            var checkbox = new Toggle("IsRepeatable");
            checkbox.style.fontSize = 14;

            checkbox.value = isRepeatable;

            checkboxElement.Add(checkbox);
            node.mainContainer.Add(checkboxElement);
            checkbox.RegisterValueChangedCallback(evt =>
            {
                onCheckboxValueChanged?.Invoke(evt.newValue);
            });
            
        }
        private void CreateCheckboxElements(List<string> existsPool, DialogueNode node, Action<List<string>> onCheckboxValueChanged)
        {
            var selectedElements = new List<string>();
            List<string> pool = stipulationPool.pool.ToList(); // Преобразование массива в список

            foreach (var currentElement in pool)
            {
                var checkboxElement = new VisualElement();
                var checkbox = new Toggle(currentElement);

                if (existsPool.Contains(currentElement))
                {
                    checkbox.value = true;
                    selectedElements.Add(currentElement);
                }

                checkboxElement.Add(checkbox);
                node.mainContainer.Add(checkboxElement);

                checkbox.RegisterValueChangedCallback(evt =>
                {
                    if (evt.newValue && !selectedElements.Contains(currentElement))
                    {
                        selectedElements.Add(currentElement);
                    }
                    else if (!evt.newValue)
                    {
                        selectedElements.Remove(currentElement);
                    }

                    onCheckboxValueChanged?.Invoke(selectedElements);
                });
            }
        }
        private void CreateTextElement(string labelText, DialogueNode node, string value, EventCallback<ChangeEvent<string>> callback)
        {
            var textElement = new VisualElement();
            var label = new Label(labelText);
            label.style.fontSize = 14;
            textElement.Add(label);
            node.mainContainer.Add(textElement);

            var textField = new TextField("");
            textField.multiline = true;
            textField.RegisterValueChangedCallback(callback);
            textField.SetValueWithoutNotify(value);
            node.mainContainer.Add(textField);
        }
        private void CreateChoiceElement(string labelText, DialogueNode node, string value, EventCallback<ChangeEvent<string>> callback)
        {
            var textElement = new VisualElement();
            var label = new Label(labelText);
            label.style.fontSize = 14;
            textElement.Add(label);
            node.mainContainer.Add(textElement);

            var popupField = new PopupField<string>(TypesText, Enum.GetName(typeof(NodeType), node.Type));
            popupField.RegisterValueChangedCallback(callback);
            popupField.SetValueWithoutNotify(value);
            node.mainContainer.Add(popupField);
        }
        public void AddChoicePort(DialogueNode nodeCache)
        {
            var generatedPort = GetPortInstance(nodeCache, Direction.Output);
            var portLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.contentContainer.Remove(portLabel);

            var outputPortCount = nodeCache.outputContainer.Query("connector").ToList().Count();
            var outputPortName = $"Option {outputPortCount + 1}";

            var textField = new Label(outputPortName.ToString());

            textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
            generatedPort.contentContainer.Add(new Label("  "));

            generatedPort.contentContainer.Add(textField);
            var deleteButton = new Button(() => RemovePort(nodeCache, generatedPort))
            {
                text = "X"
            };
            generatedPort.contentContainer.Add(deleteButton);
            generatedPort.portName = outputPortName;
            nodeCache.outputContainer.Add(generatedPort);
            nodeCache.RefreshPorts();
            nodeCache.RefreshExpandedState();
        }

        private void RemovePort(Node node, Port socket)
        {
            var targetEdge = edges.ToList()
                .Where(x => x.output.portName == socket.portName && x.output.node == socket.node);
            if (targetEdge.Any())
            {
                var edge = targetEdge.First();
                edge.input.Disconnect(edge);
                RemoveElement(targetEdge.First());
            }

            node.outputContainer.Remove(socket);
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        private Port GetPortInstance(DialogueNode node, Direction nodeDirection,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
        }

        private DialogueNode GetEntryPointNodeInstance()
        {
            var nodeCache = new DialogueNode()
            {
                Id = 0,
                title = "STARTNODE",
                IsRepeatable = true,
                DialogueText = "STARTNODE",
                Type = NodeType.Text,
                Trial = -1,
                Gift = "",
                Bg = "",
                Sound = "",
                Music = "",
                EntryPoint = true
            };

            var generatedPort = GetPortInstance(nodeCache, Direction.Output);
            generatedPort.portName = "Next";
            nodeCache.outputContainer.Add(generatedPort);

            nodeCache.capabilities &= ~Capabilities.Movable;
            nodeCache.capabilities &= ~Capabilities.Deletable;

            nodeCache.RefreshExpandedState();
            nodeCache.RefreshPorts();
            nodeCache.SetPosition(new Rect(100, 200, 100, 150));
            return nodeCache;
        }
    }
}