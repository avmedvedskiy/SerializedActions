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
        public event Action<IAction, int> OnActionChanged;
        public bool IsRunning { get; private set; }

        private int _currentIndex;
        private readonly Queue<TAction> _queue = new();
        private CancellationTokenSource _token;
        private TAction _currentAction;

        public async UniTask RunAsync<T>(List<T> actions, int startedIndex = 0) where T : class, IAction
        {
            for (int i = startedIndex; i < actions.Count; i++)
            {
                _queue.Enqueue(actions[i] as TAction);
            }

            _currentIndex = startedIndex;
            //ошибка, при повторном запуске не будет ожидания
            if (IsRunning)
                return;

            await Run();
        }

        public UniTask RunAsync(List<TAction> actions, int startedIndex = 0) => RunAsync<TAction>(actions, startedIndex);

        private async UniTask Run()
        {
            _token = new CancellationTokenSource();
            IsRunning = true;

            do
            {
                _currentAction = _queue.Dequeue();
                _currentAction.SetParent(this);
                OnActionChanged?.Invoke(_currentAction, _currentIndex);
                _currentIndex++;
                try
                {
                    if (_currentAction.DontWait)
                        _currentAction.RunAsync(_token.Token);
                    else
                        await _currentAction.RunAsync(_token.Token);
                }
                catch (OperationCanceledException)
                {
                    Debug.Log($"Task stoped {_currentAction}");
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            } while (_queue.Count > 0 && !_token.Token.IsCancellationRequested);

            IsRunning = false;
        }


        public void Stop()
        {
            _token?.Cancel();
            _token?.Dispose();
            _currentAction.Stop();
            _queue.Clear();
        }
    }
}