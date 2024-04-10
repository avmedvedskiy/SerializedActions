using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Actions.Editor.Graph
{
    public class ActionGraphView : GraphView
    {
        private readonly ActionGraphWindow _actionGraphWindow;
        private readonly List<EmptyNode> _nodes;
        private MiniMap _miniMap;

        public List<EmptyNode> Nodes => graphElements.OfType<EmptyNode>().ToList();

        public ActionGraphView(ActionGraphWindow actionGraphWindow)
        {
            _actionGraphWindow = actionGraphWindow;
            _nodes = new List<EmptyNode>();

            AddGridBackGround();
            AddManipulators();
            AddMiniMap();
            AddStyles();
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

        public void OnSelectNode(EmptyNode node)
        {
            _actionGraphWindow.OnSelectNode(node);
        }

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
            //this.AddManipulator(CreateNodeContextualMenu("Add Start Node", CreateStartNode));
            //this.AddManipulator(CreateNodeContextualMenu("Add End Node", CreateEndNode));
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

        private EmptyNode CreateNode(Vector2 position)
        {
            var node = new ActionNode("DialogueNode", this);
            node.SetPosition(new Rect(position, Vector2.zero));
            OnSelectNode(node);
            node.Draw();
            _nodes.Add(node);
            return node;
        }

        public EmptyNode CreateNode(NodeData data)
        {
            var node = new ActionNode("DialogueNode", this)
            {
                ID = data.ID,
                userData = data,
                Name = string.IsNullOrEmpty(data.Name) ? data.SubContainer.name : data.Name
            };
            node.SetPosition(new Rect(data.Position, Vector2.zero));
            node.Draw();
            _nodes.Add(node);
            AddElement(node);
            return node;
        }


        public EmptyNode FindNode(string id) => _nodes.Find(x => x.ID == id);

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
            _nodes.Clear();
            graphElements.ForEach(RemoveElement);
        }
    }
}