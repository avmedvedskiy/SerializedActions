using System;
using System.Collections.Generic;
using System.Linq;
using Actions.Dialogues;
using UnityEngine;

namespace Actions
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Nodes/ActionsContainer", fileName = "ActionsContainer",
        order = 0)]
    public class ActionNodeContainer : ScriptableObject
    {
        [SerializeField] private List<NodeData> _actionNodes;
        public List<NodeData> ActionNodes => _actionNodes;

        
        /*
        [ContextMenu(nameof(Repair))]
        private void Repair()
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
            foreach (var a in assets)
            {
                var node = ActionNodes.Find(x => x.Name == a.name);
                if (node != null)
                {
                    node.SubContainer = a as ActionContainer;
                }
                EditorUtility.SetDirty(a);
            }
                        
            foreach (var data in ActionNodes)
            {
                if (data.SubContainer == null)
                {
                    Debug.LogError($"Container null {data.Name}");
                    continue;
                }
                
                var choices = data.SubContainer.Actions.OfType<IChoiceNode>().SelectMany(x => x.Choices).ToArray();
                for (int i = 0; i < data.Links.Length; i++)
                {
                    string link = data.Links[i];
                    ActionContainer nextStorage = ActionNodes.Find(x => x.ID == link)?.SubContainer;
                    
                    if(i >= choices.Length)
                        continue;
                    
                    choices[i].Node = nextStorage;
                }
            }
            EditorUtility.SetDirty(this);
            
        }
        */
        

        public ActionNodeContainer Clone()
        {
#if UNITY_EDITOR
            //думаю такое только для редактора и фаст мода актуально, с домейн релоадом все ок
            var clone = Instantiate(this);

            foreach (var nodeData in clone.ActionNodes)
            {
                var clonedDialogue = Instantiate(nodeData.SubContainer);
                clonedDialogue.name = nodeData.SubContainer.name;
                nodeData.SubContainer = clonedDialogue;
            }

            foreach (var nodeData in clone.ActionNodes)
            {
                foreach (var choice in nodeData.SubContainer.Actions.OfType<IChoiceNode>()
                             .SelectMany(x => x.Choices))
                {
                    if (choice.Node == null)
                        throw new Exception($"Not linked dialogues in node {nodeData.Name}");

                    var linkNode = clone.ActionNodes.Find(x => x.Name == choice.Node.name);
                    if (linkNode != null)
                        choice.Node = linkNode.SubContainer;
                }
            }

            return clone;
#else
            return this;
#endif
        }
    }
}