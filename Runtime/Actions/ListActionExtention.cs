using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Actions
{
    public static class ListActionExtenstion
    {
        public static ActionRunner Run<T>(this List<T> actions) where T : class, IAction
        {
            ActionRunner runner = new ActionRunner();
            runner.RunAsync(actions);
            return runner;
        }

        public static void Run<T>(this List<T> actions, ActionRunner runner) where T : class, IAction
        {
            runner.RunAsync(actions).Forget();
        }

        public static async UniTask RunAsync<T>(this List<T> actions, ActionRunner runner) where T : class, IAction
        {
            await runner.RunAsync(actions);
        }

        public static async UniTask RunAsync<T>(this List<T> actions) where T : class, IAction
        {
            ActionRunner runner = new ActionRunner();
            await runner.RunAsync(actions);
        }
    }
}

