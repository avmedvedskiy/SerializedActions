using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace Actions
{
    [ManagedReference.ManagedReferenceGroup("Debug")]
    [Serializable]
    public class DebugLogAction : BaseAction
    {
        [SerializeField]

        private string _value;

        public override void Stop()
        {

        }

        public override UniTask RunAsync(CancellationToken cancellationToken)
        {
            Debug.Log(_value);
            return UniTask.CompletedTask;
        }
    }
}
