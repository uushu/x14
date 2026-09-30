using cfg;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain.FlowerLine
{
    public class FlowerLineEditor : MonoBehaviour
    {
        public const string floderPath = "Assets/GameMain/AssetBundle/Prefab/Window/FlowerLine/Level";

#if UNITY_EDITOR
        public Button enableAll;

        public Button disableAll;

        void Start()
        {
            enableAll.onClick.AddListener(() =>
            {
                EnableAllGrids(true);
            });

            disableAll.onClick.AddListener(() =>
            {
                EnableAllGrids(false);
            });

            var childCount = hexagonGrids.gridsRoot.childCount;

            for (int i = 0; i < childCount; i++)
            {
                var child = hexagonGrids.gridsRoot.GetChild(i);

                if (child.gameObject.activeSelf == false) continue;

                var localPoint = child.localPosition;
                var gridXY = hexagonGrids.LocalPositionToGridXY(localPoint);
            }
        }

        private void EnableAllGrids(bool isEnable)
        {
            var childCount = hexagonGrids.gridsRoot.childCount;

            for (int i = 0; i < childCount; i++)
            {
                hexagonGrids.gridsRoot.GetChild(i).gameObject.SetActive(isEnable);
            }
        }

        private void Update()
        {
            if (hexagonGrids != null)
            {
                if (Input.GetMouseButton(0))
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(hexagonGrids.gridsRoot, Input.mousePosition, Camera.main, out var localPoint);

                    var gridXY = hexagonGrids.LocalPositionToGridXY(localPoint);
                    var childGrid = hexagonGrids.GetChidByGridXY(gridXY);

                    if (childGrid != null)
                    {
                        childGrid.gameObject.SetActive(true);
                    }
                }
                else if (Input.GetMouseButton(1))
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(hexagonGrids.gridsRoot, Input.mousePosition, Camera.main, out var localPoint);

                    var gridXY = hexagonGrids.LocalPositionToGridXY(localPoint);
                    var childGrid = hexagonGrids.GetChidByGridXY(gridXY);

                    if (childGrid != null)
                    {
                        childGrid.gameObject.SetActive(false);
                    }
                }
            }
         
        }

        [FoldoutGroup("编辑器")]
        [LabelText("编辑的预制体")]
        [ReadOnly]
        public GameObject editorPrefab;

        [FoldoutGroup("编辑器")]
        [LabelText("编辑的连线实例化")]
        [ReadOnly]
        public UIHexagonGrids hexagonGrids;

        [FoldoutGroup("编辑器")]
        [LabelText("连线关卡Id")]
        public LineLevelId lineLevelId;

        private void ReloadLinePrefab(int lineId)
        {
            gameObject.DestroyAllChildren();

            var bytes = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameMain/AssetBundle/LubanData/Bytes/LineLevelCategory.bytes");
            var category = AssetLoader.GetCategory_Editor<LineLevelCategory>();
            var config = category.GetOrDefault(lineId);
            if (config != null)
            {
                var prefab = AssetLoader.LoadGameObject(floderPath, config.LevelPrefab);

                if (prefab != null)
                {
                    editorPrefab = prefab;

                    var instance = (UnityEditor.PrefabUtility.InstantiatePrefab(prefab) as GameObject);
                    hexagonGrids = instance.GetComponent<UIHexagonGrids>();
                    UnityEditor.PrefabUtility.UnpackPrefabInstance(instance, UnityEditor.PrefabUnpackMode.Completely, UnityEditor.InteractionMode.UserAction);
                    hexagonGrids.transform.SetParent(transform, false);
                    hexagonGrids.name = config.LevelPrefab;
                }
            }

        }

        [FoldoutGroup("编辑器")]
        [Button("刷新关卡")]
        private void OnLineIdChange()
        {
            ReloadLinePrefab(lineLevelId);
        }

        [FoldoutGroup("编辑器")]
        [Button("保存关卡")]
        private void SaveLevelPrefab()
        {
            if (hexagonGrids != null)
            {
                var savePath = $"{floderPath}/{hexagonGrids.gameObject.name}.prefab";
                UnityEditor.PrefabUtility.SaveAsPrefabAsset(hexagonGrids.gameObject, savePath, out var isSuccess);

                if (isSuccess)
                {
                    Debug.Log($"保存预制体成功:{hexagonGrids.gameObject.name}");
                }
                else
                { 
                    Debug.LogError($"保存预制失败:{hexagonGrids.gameObject.name}");
                }
            }
            else
            {
                Debug.LogError("保存关卡失败, editorLinePrefab=null");
            }
        }

        private void OnValidate()
        {
        }
#endif
    }
}