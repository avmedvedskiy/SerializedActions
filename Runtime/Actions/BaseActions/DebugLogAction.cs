using Cysharp.Threading.Tasks;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace Actions
{
    [Category("Debug")]
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
