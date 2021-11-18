using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Actions
{
    [ManagedReference.ManagedReferenceGroup("Array")]
    [System.Serializable]
    public abstract class GenericParallelArrayAction<T> : BaseAction where T:IAction
    {
        private enum WaitType { WhenAll, WhenAny }

        [SerializeField]
        private WaitType _type;

        [SerializeReference, ManagedReference.ManagedReference]
        protected List<T> _actions;

        private List<UniTask> _tasks = new List<UniTask>();
        public override async UniTask RunAsync(CancellationToken cancellationToken)
        {
            _tasks.Clear();
            for (int i = 0; i < _actions.Count; i++)
            {
                _tasks.Add(_actions[i].RunAsync(cancellationToken));
            }
            switch (_type)
            {
                case WaitType.WhenAll:
                    await UniTask.WhenAll(_tasks);
                    break;
                case WaitType.WhenAny:
                    await UniTask.WhenAny(_tasks);
                    break;
            }
        }

        public override void Stop()
        {

        }
    }
}
