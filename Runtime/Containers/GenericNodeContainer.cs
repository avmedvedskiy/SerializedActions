using System.Collections.Generic;
using UnityEngine;

namespace Actions
{
    //[CreateAssetMenu(menuName = "Scriptable Objects/Nodes/NodeStorage")]
    public class GenericNodeContainer<T> : ScriptableObject where T: class
    {
        [ManagedReference.ManagedReference,SerializeReference]
        private List<T> _actions = new List<T>();

        public List<T> Actions => _actions;
    }
}
