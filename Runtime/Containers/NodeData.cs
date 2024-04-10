using System;
using Actions;
using Actions.Dialogues;
using UnityEngine;

namespace DialogueSystem
{
    [Serializable]
    public class NodeData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public Vector3 Position { get; set; }
        [field: SerializeField] public string[] Links { get; set; }
        [field: SerializeField] public ActionContainer SubContainer { get; set; }
    }
}