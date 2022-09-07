#pragma warning disable CS4014
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Actions
{
    public class GenericRunner<TAction> : IActionRunner where TAction : class, IAction
    {
        public bool IsRunned { get; private set; }
        public TAction CurrentAction { get => _currentAction; }

        private Queue<TAction> _queue = new Queue<TAction>();
        private CancellationTokenSource _token;
        private TAction _currentAction;

        public async UniTask RunAsync<T>(List<T> actions, int startedIndex = 0) where T : class, IAction
        {
            for (int i = startedIndex; i < actions.Count; i++)
            {
                _queue.Enqueue(actions[i] as TAction);
            }

            if (IsRunned)
                return;

            await Run();
        }

        public async UniTask RunAsync(List<TAction> actions, int startedIndex = 0)
        {
            await RunAsync<TAction>(actions, startedIndex);
        }

        private async UniTask Run()
        {
            _token = new CancellationTokenSource();
            IsRunned = true;

            do
            {
                _currentAction = this._queue.Dequeue();
                _currentAction.SetParent(this);
                try
                {
                    if (_currentAction.DontWait)
                        _currentAction.RunAsync(_token.Token);
                    else
                        await _currentAction.RunAsync(_token.Token);
                }
                catch (TaskCanceledException)
                {
                    Debug.Log($"Task stoped {_currentAction}");
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            while (this._queue.Count > 0 && !_token.Token.IsCancellationRequested);

            IsRunned = false;
        }


        public void Stop()
        {
            _token?.Cancel();
            _token?.Dispose();
            while (this._queue.Count > 0)
            {
                try
                {
                    var action = this._queue.Dequeue();
                    action.Stop();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
    }
}
