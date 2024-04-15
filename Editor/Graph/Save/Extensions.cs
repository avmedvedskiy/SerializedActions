using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Actions.Editor.Graph.Save
{
    internal static class Extensions
    {
        internal static bool HasAsset(this ActionNodeContainer nodeContainer) => string.IsNullOrEmpty(AssetDatabase.GetAssetPath(nodeContainer)) == false;

        internal static void SaveAsset(this ActionNodeContainer nodeContainer)
        {
            if (nodeContainer.HasAsset())
            {
                EditorUtility.SetDirty(nodeContainer);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        internal static NodeData AddNewData(this ActionNodeContainer container, string name)
        {
            var data = new NodeData
            {
                ID = Guid.NewGuid().ToString(),
                Name = name
            };
            var storage = CreateNewActionContainer(container, name);
            Undo.RegisterCompleteObjectUndo(container, container.name);
            data.SubContainer = storage;
            container.ActionNodes.Add(data);
            //container.SaveAsset();
            return data;
        }
        
        private static ActionContainer CreateNewActionContainer(ActionNodeContainer container, string name)
        {
            var storage = ScriptableObject.CreateInstance<ActionContainer>();
            Undo.RegisterCreatedObjectUndo(storage, name);
            storage.name = name;
            if(container.HasAsset())
                AssetDatabase.AddObjectToAsset(storage, container);
            return storage;
        }

        internal static void RemoveNode(this ActionNodeContainer nodeContainer, string id)
        {
            //Undo.RegisterCompleteObjectUndo(nodeContainer, nodeContainer.name);
            var node = nodeContainer.ActionNodes.Find(x => x.ID == id);
            nodeContainer.ActionNodes.Remove(node);
            if (nodeContainer.HasAsset())
            {
                Undo.DestroyObjectImmediate(node.SubContainer);
                //Object.DestroyImmediate(node.SubContainer, true);
                //nodeContainer.SaveAsset();
            }
        }

        internal static void SaveAsset(this ActionNodeContainer nodeContainer, string path)
        {
            AssetDatabase.CreateAsset(nodeContainer, path);
            foreach (var node in nodeContainer.ActionNodes)
            {
                AssetDatabase.AddObjectToAsset(node.SubContainer, nodeContainer);
            }
            EditorUtility.SetDirty(nodeContainer);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}