using Cysharp.Threading.Tasks;
using System.Threading;

namespace Actions
{
    public interface IAction
    {
        bool DontWait { get; }

        /// <summary>
        /// Run Action async
        /// </summary>
        /// <returns></returns>
        UniTask RunAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Called when action should be stoped
        /// </summary>
        void Stop();

        internal void SetParent(IActionRunner runner);
    }
}
