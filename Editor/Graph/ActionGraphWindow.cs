using Actions.Editor.Graph.Save;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Actions.Editor.Graph
{
    public class ActionGraphWindow : EditorWindow
    {
        private ActionGraphView _graphView;
        private SaveLoadUtility _saveLoadUtility;
        private ActionNodeContainer _container;
        private UnityEditor.Editor _editor;
        private ActionNode _selectedNode;

        [MenuItem("Tools/Action Graph")]
        public static void Open()
        {
            GetWindow<ActionGraphWindow>("Action Graph");
        }
        
        public static void Open(ActionNodeContainer container)
        {
            var window = GetWindow<ActionGraphWindow>("Action Graph");
            window.Load(container);
        }

        private void OnEnable()
        {
            _saveLoadUtility = new SaveLoadUtility();
            AddGraphView();
            AddToolbar();
            AddEditorGUI();
            _graphView.Load(_saveLoadUtility.CreateContainer());
            Undo.undoRedoPerformed += UndoRedoPerformed;
        }

        private void UndoRedoPerformed()
        {
            _graphView.Refresh();
        }

        private void OnDestroy()
        {
            Save();
            Undo.undoRedoPerformed -= UndoRedoPerformed;
        }

        private void AddEditorGUI()
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
            if (_editor != null)
            {
                if (_editor.DrawDefaultInspector())
                {
                    _selectedNode?.Update(); 
                }

            }
        }

        private void AddGraphView()
        {
            _graphView = new ActionGraphView(this, _saveLoadUtility);
            _graphView.StretchToParentSize();
            _graphView.style.width = new Length(65, LengthUnit.Percent);

            rootVisualElement.Add(_graphView);
        }

        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();
            Button saveButton = UIElementUtility.CreateButton("Save", Save);
            Button loadButton = UIElementUtility.CreateButton("Load", Load);
            Button clearButton = UIElementUtility.CreateButton("Clear", Clear);
            Button miniMapButton = UIElementUtility.CreateButton("Minimap", ToggleMiniMap);
            toolbar.Add(saveButton);
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
            _container = null;
            _editor = null;
        }

        private void Load()
        {
            var newContainer = _saveLoadUtility.Load();
            if (newContainer != null)
            {
                _container = newContainer;
                _graphView.Load(newContainer);
            }
            _editor = UnityEditor.Editor.CreateEditor(_container);
        }

        private void Load(ActionNodeContainer container)
        {
            _container = container;
            _graphView.Load(container);
            _editor = UnityEditor.Editor.CreateEditor(_container);
        }
        
        private void Save()
        {
            _graphView.Save();
            _editor ??= UnityEditor.Editor.CreateEditor(_container);
        }

        public void OnSelectNode(ActionNode node)
        {
            _selectedNode = node;
            _editor = _selectedNode != null
                ? UnityEditor.Editor.CreateEditor(node.Data.SubContainer)
                : null;
        }
    }
}