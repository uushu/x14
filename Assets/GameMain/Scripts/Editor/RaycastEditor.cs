using TMPro;
using UnityEditor;
using UnityEngine.UI;

namespace GameMain.Editor
{
    public static class RaycastEditor
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private static void OnHierarchyChanged()
        {
            var activeGameObject = Selection.activeGameObject;

            if (activeGameObject != null)
            {
                if (activeGameObject.name.Equals("Image"))
                {
                    var imgComponent = activeGameObject.GetComponent<Image>();
                    if (imgComponent != null)
                    {
                        imgComponent.raycastTarget = false;
                    }
                }
                else if (activeGameObject.name.Contains("Text"))
                {
                    var txtComponent = activeGameObject.GetComponent<TextMeshProUGUI>();

                    if (txtComponent != null)
                    {
                        txtComponent.raycastTarget = false;
                    }
                }
            }
        }
    }
}