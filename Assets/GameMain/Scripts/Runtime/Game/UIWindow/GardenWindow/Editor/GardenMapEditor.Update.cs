using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain.Garden.Editor
{
#if UNITY_EDITOR

    public partial class GardenMapEditor
    {
        [FoldoutGroup("运行时")]
        public Button btnClearAll;

        [FoldoutGroup("运行时")]
        public Button btnCopy;

        private void Start()
        {
            btnClearAll.onClick.AddListener(() =>
            {
                ClearAll();
            });

            btnCopy.onClick.AddListener(() =>
            {
                OnCopyClick();
            });

            flowerRoot.gameObject.SetActive(false);
            ClearAll();
        }

        private void ClearAll()
        {
            var childCount = customGrids.generatorRoot.childCount;
            for (int i = 0; i < childCount; i++)
            {
                customGrids.generatorRoot.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
        }

        private void OnCopyClick()
        {
            _gridXY2Obj.Clear();
            NewRefreshRule();
            _refresh.RefreshFlowerTypeWeight();

            for (int y = 0; y < customGrids.rowCount; y++)
            {
                for (int x = 0; x < customGrids.colCount; x++)
                {
                    var gridXY = new Vector2Int(x, y);

                    var childIndex = customGrids.GridXY2Index(gridXY);
                    var childObj = customGrids.generatorRoot.GetChild(childIndex).gameObject;

                    if (childObj.transform.GetChild(0).gameObject.activeSelf)
                    {
                        _gridXY2Obj.Add(gridXY, childObj);
                    }
                }
            }

            //地图上的格子 转换成 格子类型
            var initGrids = MapGridExchangeToFlowerGrid(false);
            CopyInitGridsConfig();
        }

        private void Update()
        {
            if (customGrids != null)
            {
                if (Input.GetMouseButton(0))
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(customGrids.generatorRoot.GetComponent<RectTransform>(), 
                        Input.mousePosition, Camera.main, out var localPoint);

                    var gridXY = customGrids.LocalPosToGridXY(localPoint);
                    var childGrid = customGrids.GetChildByIndex(gridXY);

                    if (childGrid != null)
                    {
                        childGrid.GetChild(0).gameObject.SetActive(true);
                    }
                }
                else if (Input.GetMouseButton(1))
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(customGrids.generatorRoot.GetComponent<RectTransform>(), 
                        Input.mousePosition, Camera.main, out var localPoint);

                    var gridXY = customGrids.LocalPosToGridXY(localPoint);
                    var childGrid = customGrids.GetChildByIndex(gridXY);

                    if (childGrid != null)
                    {
                        childGrid.GetChild(0).gameObject.SetActive(false);
                    }
                }
            }
        }
    }

#endif
}