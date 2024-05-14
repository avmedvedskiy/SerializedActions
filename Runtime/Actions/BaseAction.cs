using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Actions
{
    public abstract class BaseAction : IAction
    {
        protected IActionRunner Runner { get; private set; }

        [SerializeField]
        private bool _dontWait;

        public bool DontWait => _dontWait;

        public abstract UniTask RunAsync(CancellationToken cancellationToken);

        public virtual void Stop()
        {
            
        }

        void IAction.SetParent(IActionRunner runner)
        {
            Runner = runner;
        }
    }
}
