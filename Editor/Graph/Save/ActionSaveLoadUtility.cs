using System.Collections.Generic;
using System.Linq;
using Actions.Dialogues;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Actions.Editor.Graph.Save
{
    public class ActionSaveLoadUtility
    {
        private readonly ActionGraphView _graphView;

        private List<EmptyNode> Nodes => _graphView.Nodes;

        public ActionSaveLoadUtility(ActionGraphView graphView)
        {
            _graphView = graphView;
        }

        public ActionNodeContainer Save(ActionNodeContainer actionNodeContainer)
        {
            if (actionNodeContainer == null)
            {
                Debug.LogError($"Dialogue Container is null");
                return null;
            }

            RemoveOldNodes(actionNodeContainer);
            foreach (var node in Nodes)
            {
                GetOrCreateData(actionNodeContainer, node);
            }

            LinkDialogues(actionNodeContainer);

            EditorUtility.SetDirty(actionNodeContainer);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return actionNodeContainer;
        }


        public ActionNodeContainer SaveAs(ActionNodeContainer actionNodeContainer)
        {
            var path = EditorUtility.SaveFilePanelInProject("Save dialogue", "DialogueContainer", "asset",
                "Enter a name for the new dialogue asset");
            if (string.IsNullOrEmpty(path))
                return actionNodeContainer;

            if (actionNodeContainer != null)
            {
                actionNodeContainer = actionNodeContainer.Clone(); //create a copy
                AssetDatabase.CreateAsset(actionNodeContainer, path);
                foreach (var storage in actionNodeContainer.ActionNodes)
                {
                    AssetDatabase.AddObjectToAsset(storage.SubContainer, actionNodeContainer);
                }

                RemoveOldNodes(actionNodeContainer);
                foreach (var node in Nodes)
                {
                    GetOrCreateData(actionNodeContainer, node);
                }

                LinkDialogues(actionNodeContainer);
            }
            else
            {
                //create new
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ActionNodeContainer>(), path);
                actionNodeContainer = AssetDatabase.LoadAssetAtPath<ActionNodeContainer>(path);
            }

            EditorUtility.SetDirty(actionNodeContainer);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return actionNodeContainer;
        }


        private void LinkDialogues(ActionNodeContainer actionNodeContainer)
        {
            foreach (var data in actionNodeContainer.ActionNodes)
            {
                if (data.SubContainer == null)
                {
                    Debug.LogError($"Actions is null {data.Name}");
                    continue;
                }
                
                var choices = data.SubContainer.Actions.OfType<IChoiceNode>().SelectMany(x => x.Choices).ToArray();
                for (int i = 0; i < data.Links.Length; i++)
                {
                    string link = data.Links[i];
                    ActionContainer nextStorage = actionNodeContainer.ActionNodes.Find(x => x.ID == link)?.SubContainer;
                    
                    choices[i].Node = nextStorage;
                }
            }
        }

        private string[] GetLinks(EmptyNode node)
        {
            var outputEdges = _graphView.edges
                .Where(x => x.output.node == node).ToList(); //get all output edges

            var ports = node.outputContainer
                .Children()
                .OfType<Port>()
                .ToList();

            string[] links = new string[ports.Count];

            for (int i = 0; i < ports.Count; i++)
            {
                var port = ports[i];
                var edge = outputEdges.Find(x => x.output == port); // find edge with this port
                if (edge != null)
                    links[i] = (edge.input.node as EmptyNode)?.ID;
            }

            return links;
        }

        public NodeData GetOrCreateData(ActionNodeContainer container, EmptyNode node)
        {
            var data = container.ActionNodes.Find(x => x.ID == node.ID);
            if (data == null)
            {
                data = new NodeData();
                var storage = ScriptableObject.CreateInstance<ActionContainer>();
                storage.name = node.Name;
                AssetDatabase.AddObjectToAsset(storage, container);
                data.SubContainer = storage;
                container.ActionNodes.Add(data);

                EditorUtility.SetDirty(container);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            data.ID = node.ID;
            data.Position = node.GetPosition().position;
            data.Name = node.Name;
            if(data.SubContainer != null)
                data.SubContainer.name = node.Name;
            data.Links = GetLinks(node);
            node.userData = data;
            return data;
        }

        private void RemoveOldNodes(ActionNodeContainer container)
        {
            for (int i = 0; i < container.ActionNodes.Count; i++)
            {
                var data = container.ActionNodes[i];
                if (!Nodes.Exists(x => x.ID == data.ID))
                {
                    AssetDatabase.RemoveObjectFromAsset(data.SubContainer);
                    container.ActionNodes.RemoveAt(i);
                    i--;
                }
            }
        }

        public ActionNodeContainer Load()
        {
            _graphView.ClearAll();

            var path = EditorUtility.OpenFilePanel("Load dialogue", Application.dataPath, "asset");
            if (string.IsNullOrEmpty(path))
                return null;
            var dialogueContainer = LoadAssetAtFullPath<ActionNodeContainer>(path);
            return Load(dialogueContainer);
        }

        private T LoadAssetAtFullPath<T>(string path) where T : Object
        {
            string assetPath = "Assets" + path.Replace(Application.dataPath, "");
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }

        public ActionNodeContainer Load(ActionNodeContainer actionNodeContainer)
        {
            _graphView.ClearAll();
            foreach (NodeData t in actionNodeContainer.ActionNodes)
            {
                _graphView.CreateNode(t);
            }

            LoadConnection(actionNodeContainer);
            return actionNodeContainer;
        }

        private void LoadConnection(ActionNodeContainer actionNodeContainer)
        {
            foreach (var storage in actionNodeContainer.ActionNodes)
            {
                var outputNode = _graphView.FindNode(storage.ID);
                var outputPorts = outputNode.outputContainer.Children().OfType<Port>().ToList();
                for (var i = 0; i < outputPorts.Count; i++)
                {
                    if (i >= storage.Links.Length)
                        break;

                    var link = storage.Links[i];

                    if (string.IsNullOrEmpty(link))
                        continue;

                    var port = outputPorts[i];
                    var inputNode = _graphView.FindNode(link);
                    var edge = port.ConnectTo(inputNode.inputContainer.Children().First() as Port);
                    _graphView.AddElement(edge);

                    inputNode.RefreshPorts();
                }

                outputNode.RefreshPorts();
            }
        }

        private static T CreateNewAsset<T>(string fullPath) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(fullPath);
            if (asset != null)
                return asset;

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, fullPath);
            return asset;
        }
    }
}