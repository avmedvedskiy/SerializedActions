using System;
using System.Collections.Generic;
using System.Linq;
using Actions.Editor.Graph.Save;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Actions.Editor.Graph
{
    public class ActionGraphView : GraphView
    {
        private ActionNodeContainer ActionNodeContainer { get; set; }
        private readonly ActionGraphWindow _actionGraphWindow;
        private readonly SaveLoadUtility _saveLoadUtility;
        private MiniMap _miniMap;

        private List<ActionNode> Nodes => graphElements.OfType<ActionNode>().ToList();
        private int NodeCount => Nodes.Count;

        public ActionGraphView(
            ActionGraphWindow actionGraphWindow,
            SaveLoadUtility saveLoadUtility)
        {
            _actionGraphWindow = actionGraphWindow;
            _saveLoadUtility = saveLoadUtility;

            AddGridBackGround();
            AddManipulators();
            AddMiniMap();
            AddStyles();
            graphViewChanged += OnGraphViewChanged;
        }
        
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphviewchange)
        {
            if (graphviewchange.elementsToRemove is {Count:>0})
                foreach (var element in graphviewchange.elementsToRemove)
                {
                    if (element is ActionNode actionNode)
                    {
                        ActionNodeContainer.RemoveNode(actionNode.ID);
                        OnSelectNode(null);
                    }
                }
            
            Undo.RegisterFullObjectHierarchyUndo(ActionNodeContainer,ActionNodeContainer.name);

            return graphviewchange;
        }

        public void Save()
        {
            foreach (var node in Nodes)
            {
                node.Update();
            }

            _saveLoadUtility.Save(ActionNodeContainer);
        }

        public void Load(ActionNodeContainer actionNodeContainer)
        {
            ActionNodeContainer = actionNodeContainer;
            ClearAll();
            foreach (var t in actionNodeContainer.ActionNodes)
            {
                CreateNode(t);
            }

            LoadConnection(actionNodeContainer);
        }

        private void LoadConnection(ActionNodeContainer actionNodeContainer)
        {
            foreach (var storage in actionNodeContainer.ActionNodes)
            {
                var outputNode = FindNode(storage.ID);
                var outputPorts = outputNode.outputContainer.Children().OfType<Port>().ToList();
                for (var i = 0; i < outputPorts.Count; i++)
                {
                    if (i >= storage.Links.Length)
                        break;

                    var link = storage.Links[i];

                    if (string.IsNullOrEmpty(link))
                        continue;

                    var port = outputPorts[i];
                    var inputNode = FindNode(link);
                    var inputPort = inputNode.inputContainer.Children().First() as Port;
                    var edge = port.ConnectTo(inputPort);
                    AddElement(edge);

                    inputNode.RefreshPorts();
                }

                outputNode.RefreshPorts();
            }
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (evt.target is GraphView || evt.target is Node || evt.target is Group || evt.target is Edge)
            {
                evt.menu.AppendSeparator();
                evt.menu.AppendAction("Delete",
                    _ => DeleteSelectionCallback(AskUser.DontAskUser),
                    _ => canDeleteSelection
                        ? DropdownMenuAction.Status.Normal
                        : DropdownMenuAction.Status.Disabled);
            }
        }

        public void OnSelectNode(ActionNode node) => _actionGraphWindow.OnSelectNode(node);

        private void AddGridBackGround()
        {
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(CreateNodeContextualMenu("Add Node", CreateNode));
        }

        private IManipulator CreateNodeContextualMenu(string actionTitle, Func<Vector2, GraphElement> addElementFunc)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle,
                    actionEvent =>
                        AddElement(addElementFunc(GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
            );
            return contextualMenuManipulator;
        }

        private ActionNode CreateNode(Vector2 position)
        {
            var data = ActionNodeContainer.AddNewData($"NewNode {NodeCount}");
            var node = new ActionNode(data, this);
            node.SetPosition(new Rect(position, Vector2.zero));
            //OnSelectNode(node);
            node.Draw();
            return node;
        }

        public ActionNode CreateNode(NodeData data)
        {
            var node = new ActionNode(data, this);
            node.SetPosition(new Rect(data.Position, Vector2.zero));
            node.Draw();
            AddElement(node);
            return node;
        }


        public ActionNode FindNode(string id) => Nodes.Find(x => x.ID == id);

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList()!.Where(endPort =>
                    endPort.direction != startPort.direction &&
                    endPort.node != startPort.node &&
                    endPort.portType == startPort.portType)
                .ToList();
        }

        private Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = mousePosition;

            if (isSearchWindow)
            {
                worldMousePosition = _actionGraphWindow.rootVisualElement.ChangeCoordinatesTo(
                    _actionGraphWindow.rootVisualElement.parent,
                    mousePosition - _actionGraphWindow.position.position);
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

            return localMousePosition;
        }

        private void AddMiniMap()
        {
            _miniMap = new MiniMap
            {
                anchored = true,
                maxHeight = 100,
                maxWidth = 200
            };
            _miniMap.SetPosition(new Rect(0, 0, 200, 100));
            _miniMap.visible = false;

            AddMiniMapStyles();
            Add(_miniMap);
        }

        public void ToggleMiniMap()
        {
            _miniMap.visible = !_miniMap.visible;
        }

        private void AddMiniMapStyles()
        {
            StyleColor backgroundColor = new StyleColor(new Color32(29, 29, 30, 255));
            StyleColor borderColor = new StyleColor(new Color32(51, 51, 51, 255));

            _miniMap.style.backgroundColor = backgroundColor;
            _miniMap.style.borderTopColor = borderColor;
            _miniMap.style.borderRightColor = borderColor;
            _miniMap.style.borderBottomColor = borderColor;
            _miniMap.style.borderLeftColor = borderColor;
        }

        private void AddStyles()
        {
            this.AddStyleSheets(
                "Style/DSGraphViewStyles.uss",
                "Style/DSNodeStyles.uss"
            );
        }

        public void ClearAll()
        {
            graphElements.ForEach(RemoveElement);
        }

        public void Refresh()
        {
            Load(ActionNodeContainer);
        }
    }
}