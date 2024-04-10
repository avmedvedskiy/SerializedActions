using System;
using DialogueSystem.Editor.Graph.Save;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Graph
{
    public class ActionGraphWindow : EditorWindow
    {
        private ActionGraphView _graphView;
        private ActionSaveLoadUtility _actionSaveLoadUtility;
        private ActionNodeContainer _actionNodeContainer;
        private UnityEditor.Editor _editor;
        private EmptyNode _selectedNode;

        [MenuItem("Tools/Dialogue Graph")]
        public static void Open()
        {
            GetWindow<ActionGraphWindow>("Dialogue Graph");
        }
        
        public static void Open(ActionNodeContainer container)
        {
            var window = GetWindow<ActionGraphWindow>("Dialogue Graph");
            window.Load(container);
        }

        private void OnEnable()
        {
            AddGraphView();
            AddToolbar();
            AddDialogEditorGUI();
        }

        private void OnDestroy()
        {
            Save();
        }

        private void AddDialogEditorGUI()
        {
            VisualElement scroll = new ScrollView(ScrollViewMode.Vertical);

            scroll.style.position = Position.Absolute;
            //scroll.style.flexDirection = FlexDirection.Row;
            //scroll.style.flexWrap = Wrap.Wrap;
            scroll.style.top = 20;
            scroll.style.height = new Length(100, LengthUnit.Percent);
            scroll.style.width = new Length(35, LengthUnit.Percent);
            scroll.style.marginLeft = new Length(65, LengthUnit.Percent);

            IMGUIContainer container = new IMGUIContainer(OnInspectorGUI);
            scroll.style.backgroundColor = new StyleColor(Color.gray);
            scroll.Add(container);
            rootVisualElement.Add(scroll);
        }

        private void OnInspectorGUI()
        {
            if (_editor != null && _actionNodeContainer != null)
            {
                if (_editor.DrawDefaultInspector())
                {
                    _selectedNode?.OnValueChanged(); 
                }

            }
        }

        private void AddGraphView()
        {
            _graphView = new ActionGraphView(this);
            _graphView.StretchToParentSize();
            _graphView.style.width = new Length(65, LengthUnit.Percent);

            rootVisualElement.Add(_graphView);
            _actionSaveLoadUtility = new ActionSaveLoadUtility(_graphView);
        }

        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();
            Button saveButton = UIElementUtility.CreateButton("Save", Save);
            Button saveAsButton = UIElementUtility.CreateButton("Save As...", SaveAs);
            Button loadButton = UIElementUtility.CreateButton("Load", Load);
            Button clearButton = UIElementUtility.CreateButton("Clear", Clear);
            Button miniMapButton = UIElementUtility.CreateButton("Minimap", ToggleMiniMap);
            toolbar.Add(saveButton);
            toolbar.Add(saveAsButton);
            toolbar.Add(loadButton);
            toolbar.Add(clearButton);
            toolbar.Add(miniMapButton);
            rootVisualElement.Add(toolbar);
        }
        
        private void ToggleMiniMap()
        {
            _graphView.ToggleMiniMap();

            //miniMapButton.ToggleInClassList("ds-toolbar__button__selected");
        }

        private void Clear()
        {
            _graphView.ClearAll();
            _actionNodeContainer = null;
            _editor = null;
        }

        private void Load()
        {
            _actionNodeContainer = _actionSaveLoadUtility.Load();
            _editor = UnityEditor.Editor.CreateEditor(_actionNodeContainer);
        }

        private void Load(ActionNodeContainer container)
        {
            _actionNodeContainer = _actionSaveLoadUtility.Load(container);
            _editor = UnityEditor.Editor.CreateEditor(_actionNodeContainer);
        }

        private void Save()
        {
            _actionNodeContainer = _actionSaveLoadUtility.Save(_actionNodeContainer);
            _editor ??= UnityEditor.Editor.CreateEditor(_actionNodeContainer);
        }
        
        private void SaveAs()
        {
            _actionNodeContainer = _actionSaveLoadUtility.SaveAs(_actionNodeContainer);
            _editor ??= UnityEditor.Editor.CreateEditor(_actionNodeContainer);
        }

        public void OnSelectNode(EmptyNode node)
        {
            if (_actionNodeContainer == null)
            {
                SaveAs();
            }
            
            var nodeData = node.userData as NodeData;
            nodeData ??= _actionSaveLoadUtility.GetOrCreateData(_actionNodeContainer, node);
            _selectedNode = node;
            _editor = UnityEditor.Editor.CreateEditor(nodeData.SubContainer);
        }
    }
}