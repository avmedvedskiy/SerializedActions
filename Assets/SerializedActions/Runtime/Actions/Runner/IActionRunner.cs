#pragma warning disable CS4014
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Actions
{
    public interface IActionRunner
    {
        UniTask RunAsync<T>(List<T> actions, int startedIndex = 0) where T : class, IAction;
    }
}
