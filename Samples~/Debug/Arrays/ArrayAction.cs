using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using UnityEngine;

namespace Actions
{
    [Category("Debug/Array")]
    [Serializable]
    public class ArrayAction : BaseAction
    {
        [SerializeReference, ManagedReference.ManagedReference]
        protected List<IAction> _actions;

        private ActionRunner _runner;
        public override async UniTask RunAsync(CancellationToken cancellationToken)
        {
            //maybe need cancelation token, not sure - need to test
            _runner = new ActionRunner();
            await _runner.RunAsync(_actions);
        }

        public override void Stop()
        {
            _runner.Stop();
        }
    }
}
