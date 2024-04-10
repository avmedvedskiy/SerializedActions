using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Actions.Dialogues;
using ManagedReference;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Actions.DebugActions
{
    [Serializable]
    public class ChoiceModel :IChoiceModel
    {
        [SerializeField] private ActionContainer _node;
        public bool IsAvailable()
        {
            return true;
        }

        public ActionContainer Node
        {
            get => _node;
            set => _node = value;
        }
    }
    
    [Category("Debug")]
    [Serializable]
    public class NextNode : BaseAction, IChoiceNode
    {
        [SerializeField] private ChoiceModel _node;
        
        public override UniTask RunAsync(CancellationToken cancellationToken)
        {
            Runner.RunAsync(_node.Node.Actions);
            return UniTask.CompletedTask;
            
        }

        public List<IChoiceModel> Choices => new() { _node };
    }

    [Category("Debug")]
    [Serializable]
    public class RandomChoiceNode : BaseAction, IChoiceNode
    {
        public List<IChoiceModel> Choices => _choiceDatas;

        [SerializeReference, ManagedReference] private List<IChoiceModel> _choiceDatas;

        public override UniTask RunAsync(CancellationToken cancellationToken)
        {
            var random = Choices[Random.Range(0, Choices.Count)];
            
            Runner.RunAsync(random.Node.Actions);
            return UniTask.CompletedTask;

        }

        public override void Stop()
        {

        }

    }

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
