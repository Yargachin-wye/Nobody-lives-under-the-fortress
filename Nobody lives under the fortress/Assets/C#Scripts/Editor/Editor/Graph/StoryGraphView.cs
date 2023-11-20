using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Subtegral.DialogueSystem.DataContainers;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
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

        List<string> TextTypesList = new List<string> { "text", "trial", "trial_lose", "trial_win", "end" };
        Dictionary<string, int> TextTypesDict = new Dictionary<string, int> { { "text", 0 }, { "trial", 1 }, { "trial_lose", 2 }, {"trial_win", 3}, { "end", 4 } };

        public readonly Vector2 DefaultNodeSize = new Vector2(200, 150);
        public readonly Vector2 DefaultCommentBlockSize = new Vector2(300, 200);
        public DialogueNode EntryPointNode;
        public Blackboard Blackboard = new Blackboard();
        public List<ExposedProperty> ExposedProperties { get; private set; } = new List<ExposedProperty>();
        private NodeSearchWindow _searchWindow;

        public StoryGraphView(StoryGraph editorWindow)
        {
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

        public void CreateNewDialogueNode(string nodeName, string text, int trial, string bg, string sound, string music, Vector2 position)
        {
            DialogueNode node = CreateNode(nodeName, text, trial, bg, sound, music, position);
            AddElement(node);
        }

        public DialogueNode CreateNode(string nodeName, string text, int trial, string bg, string sound, string music, Vector2 position)
        {
            var tempDialogueNode = new DialogueNode()
            {
                title = nodeName,
                DialogueText = text,
                GUID = Guid.NewGuid().ToString(),
                Type = nodeName,
                Trial = trial,
                Bg = bg,
                Sound = sound,
                Music = music
            };

            SetStyle(tempDialogueNode);

            var inputPort = GetPortInstance(tempDialogueNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            tempDialogueNode.inputContainer.Add(inputPort);

            tempDialogueNode.RefreshExpandedState();
            tempDialogueNode.RefreshPorts();
            tempDialogueNode.SetPosition(new Rect(position,
                DefaultNodeSize)); //To-Do: implement screen center instantiation positioning

            CreateTextElement("Text:", tempDialogueNode, tempDialogueNode.DialogueText, evt =>
            {
                tempDialogueNode.DialogueText = evt.newValue;
            });

            
            CreateChoiceElement("Выберите вариант:", tempDialogueNode, tempDialogueNode.Type, evt =>
            {
                tempDialogueNode.Type = evt.newValue;
                tempDialogueNode.title = evt.newValue;
                //SetStyle(tempDialogueNode);
            });
            CreateTextElement("Trial:", tempDialogueNode,  tempDialogueNode.Trial.ToString(), evt =>
            {
                int trail;
                tempDialogueNode.Trial = int.TryParse(evt.newValue, out trail) ? trail : -1;
            });
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
                case "text":
                    tempDialogueNode.styleSheets.Add(TextNodeStyle);
                    break;
                case "trial":
                    tempDialogueNode.styleSheets.Add(TrialNodeStyle);
                    break;
                case "trial_lose":
                    tempDialogueNode.styleSheets.Add(TrialLoseNodeStyle);
                    break;
                case "trial_win":
                    tempDialogueNode.styleSheets.Add(TrialWinNodeStyle);
                    break;
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

            var popupField = new PopupField<string>(TextTypesList, TextTypesDict[node.Type]);
            popupField.RegisterValueChangedCallback(callback);
            popupField.SetValueWithoutNotify(value);
            node.mainContainer.Add(popupField);
        }
        public void AddChoicePort(DialogueNode nodeCache, string overriddenPortName = "")
        {
            var generatedPort = GetPortInstance(nodeCache, Direction.Output);
            var portLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.contentContainer.Remove(portLabel);

            var outputPortCount = nodeCache.outputContainer.Query("connector").ToList().Count();
            var outputPortName = string.IsNullOrEmpty(overriddenPortName)
                ? $"Option {outputPortCount + 1}"
                : overriddenPortName;

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
                title = "START",
                GUID = Guid.NewGuid().ToString(),
                DialogueText = "ENTRYPOINT",
                EntyPoint = true
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