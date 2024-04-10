using System.Collections.Generic;
using System.Linq;
using Actions;
using Actions.Dialogues;
using UnityEditor.Experimental.GraphView;

namespace DialogueSystem.Editor.Graph
{
    public sealed class ActionNode : EmptyNode
    {
        private NodeData Data => userData as NodeData;
        private ActionContainer ActionStorage => Data.SubContainer;
        
        private readonly Dictionary<IChoiceModel, Port> _ports = new();

        public override bool expanded { get; set; }

        public ActionNode(string nodeName, ActionGraphView graphView) : base(nodeName, graphView)
        {
            this.expanded = true;
        }
        
        protected override void OnDraw()
        {
            UpdatePorts();
            
            Port inputPort = this.CreatePort("input", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputContainer.Add(inputPort);
            
            //Port outputPort = this.CreatePort("output");
            //Port output2Port = this.CreatePort("output2");

            //outputContainer.Add(outputPort);
            //outputContainer.Add(output2Port);

            
            //VisualElement customDataContainer = new VisualElement();
            //customDataContainer.AddToClassList("ds-node__custom-data-container");
            //extensionContainer.Add(customDataContainer);

        }

        private void UpdatePorts()
        {
            if(ActionStorage == null)
                return;
            //add new
            List<IChoiceModel> choices = ActionStorage.Actions.OfType<IChoiceNode>().SelectMany(x => x.Choices).ToList();
            foreach (var choice in choices)
            {
                if(choice == null)
                    continue;

                if (!_ports.ContainsKey(choice))
                {
                    Port port = this.CreatePort(choice.ToString());
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

        public override void OnValueChanged()
        {
            //recalculate ports
            UpdatePorts();
        }
    }
}