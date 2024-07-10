using System;
using System.Collections.Generic;
using System.Linq;
using Actions.Dialogues;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Actions.Editor.Graph
{
    public class ActionNode : Node
    {
        public string ID => Data.ID;
        private ActionContainer ActionStorage => Data.SubContainer;
        public NodeData Data { get; }

        private readonly ActionGraphView _graphView;
        
        private readonly Dictionary<IChoiceModel, Port> _ports = new();
        public ActionNode(NodeData data, ActionGraphView graphView)
        {
            Data = data;
            _graphView = graphView;

            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
        }
        

        private void AddInputPort()
        {
            Port inputPort = this.CreatePort("input", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputContainer.Add(inputPort);
        }

        private void UpdatePorts()
        {
            List<IChoiceModel> choices = ActionStorage.Actions.OfType<IChoiceNode>().SelectMany(x => x.Choices).ToList();
            foreach (var choice in choices)
            {
                if(choice == null)
                    continue;

                var port = this.CreatePort(choice.ToString());
                if (!_ports.ContainsKey(choice))
                {
                    outputContainer.Add(port);
                    _ports.Add(choice,port);
                }
                else
                {
                    _ports[choice].portName = choice.ToString();
                }
            }

            //remove old
            List<IChoiceModel> keys = new List<IChoiceModel>();
            foreach (var port in _ports)
            {
                if (!choices.Contains(port.Key))
                {
                    keys.Add(port.Key);
                }
            }
            
            foreach (var key in keys)
            {
                outputContainer.Remove(_ports[key]);
                _ports.Remove(key);
            }

            RefreshPorts();
            RefreshExpandedState();
        }

        private void RefreshLinks()
        {
            Data.Links = GetLinks();
            foreach (var port in _ports)
            {
                var outputNode = port.Value.connections.FirstOrDefault()?.input.node;
                port.Key.Node = (outputNode as ActionNode)?.Data.SubContainer;
            }
        }

        private string[] GetLinks() =>
            _ports
                .SelectMany(x => x.Value.connections)
                .Select(edge => (edge.input.node as ActionNode)?.ID)
                .ToArray();

        public override void OnSelected()
        {
            base.OnSelected();
            _graphView.OnSelectNode(this);
            UpdatePorts();
        }

        public void Draw()
        {
            TextField dialogueNameTextField = UIElementUtility.CreateTextField(Data.Name, null, callback =>
            {
                TextField target = (TextField)callback.target;
                Data.Name = target.value;
            });

            dialogueNameTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__filename-text-field");

            titleContainer.Insert(0, dialogueNameTextField);

            
            expanded = true;
            AddInputPort();
            UpdatePorts();
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Data.Position = newPos.position;
        }

        public void Update()
        {
            UpdatePorts();
            RefreshLinks();
        }
    }
}