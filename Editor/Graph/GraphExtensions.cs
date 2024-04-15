using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Actions.Editor.Graph
{
    public static class GraphExtensions
    {
        public static MiniMap CreateMiniMap()
        {
            var miniMap = new MiniMap
            {
                anchored = true,
                maxHeight = 100,
                maxWidth = 200
            };
            miniMap.SetPosition(new Rect(0, 0, 200, 100));
            miniMap.visible = false;

            StyleColor backgroundColor = new StyleColor(new Color32(29, 29, 30, 255));
            StyleColor borderColor = new StyleColor(new Color32(51, 51, 51, 255));

            miniMap.style.backgroundColor = backgroundColor;
            miniMap.style.borderTopColor = borderColor;
            miniMap.style.borderRightColor = borderColor;
            miniMap.style.borderBottomColor = borderColor;
            miniMap.style.borderLeftColor = borderColor;
            return miniMap;

        }
        
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