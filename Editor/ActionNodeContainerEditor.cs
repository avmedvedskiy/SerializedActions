using Actions.Editor.Graph;
using UnityEditor;
using UnityEngine;

namespace Actions.Editor
{
    [CustomEditor(typeof(ActionNodeContainer))]
    public class ActionNodeContainerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Open Action Graph"))
            {
                ActionGraphWindow.Open((ActionNodeContainer)target);
            }
        }
    }
}