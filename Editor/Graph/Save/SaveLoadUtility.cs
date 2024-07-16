using System;
using System.Linq;
using Actions.Dialogues;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Actions.Editor.Graph.Save
{
    public class SaveLoadUtility
    {
        public ActionNodeContainer CreateContainer() => ScriptableObject.CreateInstance<ActionNodeContainer>();

        public ActionNodeContainer Load()
        {
            var path = EditorUtility.OpenFilePanel("Load dialogue", Application.dataPath, "asset");
            return !string.IsNullOrEmpty(path) 
                ? LoadAssetAtFullPath<ActionNodeContainer>(path) 
                : null;
        }

        public void Save(ActionNodeContainer actionNodeContainer)
        {
            LinkNodes(actionNodeContainer);
            if (actionNodeContainer.HasAsset())
            {
                actionNodeContainer.SaveAsset();
            }
            else if (AskNewPath(out var assetPath))
            {
                LinkNodes(actionNodeContainer);
                actionNodeContainer.SaveAsset(assetPath);
            }
        }

        private bool AskNewPath(out string path)
        {
            path = EditorUtility.SaveFilePanelInProject("Save dialogue", "DialogueContainer", "asset",
                "Enter a name for the new dialogue asset");
            return string.IsNullOrEmpty(path) == false;
        }

        private void LinkNodes(ActionNodeContainer actionNodeContainer)
        {
            foreach (var data in actionNodeContainer.ActionNodes)
            {
                if (data.SubContainer == null)
                {
                    Debug.LogError($"Actions is null {data.Name}");
                    continue;
                }

                data.SubContainer.name = data.Name;
                EditorUtility.SetDirty(data.SubContainer);
                
                var choices = data.SubContainer.Actions.OfType<IChoiceNode>().SelectMany(x => x.Choices).ToArray();
                for (int i = 0; i < data.Links.Length; i++)
                {
                    string link = data.Links[i];
                    var nextStorage = actionNodeContainer.ActionNodes.Find(x => x.ID == link)?.SubContainer;
                    
                    choices[i].Node = nextStorage;
                }
            }
        }

        private T LoadAssetAtFullPath<T>(string path) where T : Object
        {
            string assetPath = "Assets" + path.Replace(Application.dataPath, "");
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }
    }
}