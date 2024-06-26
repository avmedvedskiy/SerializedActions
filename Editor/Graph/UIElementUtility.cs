using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Actions.Editor.Graph
{
    public static class UIElementUtility
    {
        public static Button CreateButton(string text, Action onClick = null) =>
            new(onClick)
            {
                text = text
            };

        public static Foldout CreateFoldout(string title, bool collapsed = false) =>
            new()
            {
                text = title,
                value = !collapsed
            };


        public static Port CreatePort(this Node node, string portName = "", Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single)
        {
            Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));

            port.portName = portName;

            return port;
        }
        

        public static TextField CreateTextField(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textField = new TextField()
            {
                value = value,
                label = label
            };

            if (onValueChanged != null)
            {
                textField.RegisterValueChangedCallback(onValueChanged);
            }

            return textField;
        }

        public static TextField CreateTextArea(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textArea = CreateTextField(value, label, onValueChanged);

            textArea.multiline = true;

            return textArea;
        }
    }
}