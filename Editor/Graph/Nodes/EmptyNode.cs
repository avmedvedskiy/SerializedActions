using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Actions.Editor.Graph
{
    public class EmptyNode : Node
    {
        public string ID { get; set; }
        public string Name { get; set; }

        private readonly ActionGraphView _graphView;

        public EmptyNode(string nodeName, ActionGraphView graphView)
        {
            ID = Guid.NewGuid().ToString();
            Name = nodeName;
            _graphView = graphView;

            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
        }

        public override void OnSelected()
        {
            base.OnSelected();
            _graphView.OnSelectNode(this);
        }

        public void Draw()
        {
            TextField dialogueNameTextField = UIElementUtility.CreateTextField(Name, null, callback =>
            {
                TextField target = (TextField)callback.target;
                Name = target.value;
            });

            dialogueNameTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__filename-text-field");

            titleContainer.Insert(0, dialogueNameTextField);

            OnDraw();
            
            expanded = true;
            RefreshExpandedState();
        }

        protected virtual void OnDraw()
        {
            
        }

        public virtual void OnValueChanged()
        {
            
        }
    }
}