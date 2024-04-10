using UnityEngine;

namespace Actions
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Nodes/ActionStorage")]
    public class ActionContainer : GenericNodeContainer<IAction>
    {
        [ContextMenu("RunAsync")]
        async void RunAsync()
        {
            await Actions.RunAsync();
            Debug.Log("Complete Debug Run Async");
        }

#if UNITY_EDITOR
        [ContextMenu("RunAsync", true, -1000)]
        private bool RunAsyncValidateFunction() => UnityEngine.Application.isPlaying;
#endif
    }
}
