using UnityEditor;
using UnityEngine.UIElements;

namespace Actions.Editor.Graph
{
    public static class GraphExtensions
    {
        public static VisualElement AddClasses(this VisualElement element, params string[] classNames)
        {
            foreach (string className in classNames)
            {
                element.AddToClassList(className);
            }

            return element;
        }

        public static VisualElement AddStyleSheets(this VisualElement element, params string[] styleSheetNames)
        {
            foreach (string styleSheetName in styleSheetNames)
            {
                StyleSheet styleSheet = (StyleSheet) EditorGUIUtility.Load(styleSheetName);
                if (styleSheet == null)
                {
                    //load from package
                    EditorGUIUtility.Load($"Packages/com.avmedvedskiy.serializedactions/Editor/{styleSheetName}");
                }

                element.styleSheets.Add(styleSheet);
            }

            return element;
        }
    }
}