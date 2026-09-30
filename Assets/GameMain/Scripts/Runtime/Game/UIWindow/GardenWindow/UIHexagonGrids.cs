using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameMain
{
    [LabelText("六边形格子方向")]
    public enum EARoundType
    {
        [LabelText("上")]
        Up,
        [LabelText("右上")]
        RightUp,
        [LabelText("右下")]
        RightDown,
        [LabelText("下")]
        Down,
        [LabelText("左下")]
        LeftDown,
        [LabelText("左上")]
        LeftUp,
    }

    public class UIHexagonGrids : MonoBehaviour
    {
        [LabelText("生成根节点")]
        public RectTransform gridsRoot;

        [LabelText("格子预制体")]
        public GameObject gridItemPrefab;

        [LabelText("行数")]
        public int rowCount;

        [LabelText("列数")]
        public int colCount;

        [LabelText("单个格子宽度")]
        public float gridWidth;

        [LabelText("单个格子高度")]
        public float gridHeight;

        [LabelText("行")]
        public float rowSpace;

        [LabelText("列间距")]
        public float colSpace;

        private float _startX => -(colCount - 1) * 0.5f * (gridWidth + colSpace);
        private float _startY => (rowCount - 1) * 0.5f * (gridHeight + rowSpace);

        public float Heigth => (gridHeight + rowSpace) * (rowCount + 1);
        public float Width => (gridWidth + colSpace) * colCount;

        private bool IsOddNumber(int number)
        {
            return number % 2 == 1;
        }

        public void GeneratorGrids(Action<Vector2Int, GameObject> onCreateGrid, List<Vector2Int> gridXYs)
        {
            gridsRoot.gameObject.DestroyAllChildren();

            Dictionary<int, List<GameObject>> rowMap = new();
            for (int Y = 0; Y < rowCount; Y++)
            {
                for (int X = 0; X < colCount; X++)
                {
                    GameObject gridInstance = default;
                    var gridXY = new Vector2Int(X, Y);
                    if (gridXYs != null && gridXYs.Contains(gridXY) == false)
                    {
                        continue;
                    }

                    gridInstance = GameObject.Instantiate(gridItemPrefab, gridsRoot);

                    var localPosition = GridXYToLocalPosition(gridXY);

                    gridInstance.transform.SetParent(gridsRoot);
                    gridInstance.transform.localScale = Vector3.one;
                    gridInstance.gameObject.SetActive(true);
                    gridInstance.transform.localPosition = localPosition;
                    gridInstance.transform.name = GetChildName(gridXY);
                    onCreateGrid?.Invoke(gridXY, gridInstance);

                    if (rowMap.TryGetValue(Y, out var list) == false)
                    {
                        rowMap.Add(Y, list = new());
                    }
                    list.Add(gridInstance);
                }
            }

            RefreshRectSize();
        }

        private void RefreshRectSize()
        {
            GetComponent<RectTransform>().sizeDelta = gridsRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(Width, Heigth);
        }

        public Vector2Int LocalPositionToGridXY(Vector3 localPoint)
        {
            var itemWidth = gridWidth;
            var itemHeight = gridHeight;

            var startX = -colCount * 0.5f * (itemWidth + colSpace);
            var startY = rowCount * 0.5f * (itemHeight + rowSpace);

            localPoint -= new Vector3(startX, startY, 0);
            var xIndex = Mathf.FloorToInt(localPoint.x / (itemWidth + colSpace));

            if (IsOddNumber(xIndex))
            {
                localPoint.y += (itemHeight + rowSpace) * 0.5f;
            }

            var yIndex = Mathf.FloorToInt(-(localPoint.y) / (itemHeight + rowSpace));

            return new Vector2Int(xIndex, yIndex);
        }

        public Vector3 GridXYToLocalPosition(Vector2Int gridXY)
        {
            var itemWidth = gridWidth;
            var itemHeight = gridHeight;
            var startX = _startX;
            var startY = _startY;

            var x = startX + gridXY.x * (itemWidth + colSpace);
            var y = startY - gridXY.y * (itemHeight + rowSpace);

            if (IsOddNumber(gridXY.x))
            {
                y -= (itemHeight + rowSpace) * 0.5f;
            }

            return new Vector3(x, y);
        }

        public bool GridIndexIsValid(Vector2Int gridXY)
        {
            if (gridXY.x < 0 || gridXY.x >= colCount)
            {
                return false;
            }

            if (gridXY.y < 0 || gridXY.y >= rowCount)
            {
                return false;
            }

            return true;
        }

        private string GetChildName(Vector2Int gridXY)
        {
            return $"{gridItemPrefab.name}-{gridXY.x}-{gridXY.y}";
        }
        
        public Transform GetChidByGridXY(Vector2Int gridXY)
        {
            if (GridIndexIsValid(gridXY) == false)
            {
                return null;
            }

            return gridsRoot.Find(GetChildName(gridXY));
        }

        public Vector2Int GridIndex2XY(int gridIndex)
        {
            return new Vector2Int(gridIndex % colCount, gridIndex / colCount);
        }

        public List<Vector2Int> GetActiveGridsXY()
        {
            var list = new List<Vector2Int>();

            var childCount = gridsRoot.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var childTrans = gridsRoot.GetChild(i);
                if (childTrans.gameObject.activeSelf)
                {
                    var gridXY = GridIndex2XY(i);
                    list.Add(gridXY);
                }
            }

            return list;
        }

        public Vector2Int GetARoundGridXY(Vector2Int gridXY, EARoundType around)
        {
            switch (around)
            {
                case EARoundType.Up:

                    return new Vector2Int(gridXY.x, gridXY.y - 1);

                case EARoundType.RightUp:

                    return IsOddNumber(gridXY.x) ? new Vector2Int(gridXY.x + 1, gridXY.y) : new Vector2Int(gridXY.x + 1, gridXY.y - 1);

                case EARoundType.RightDown:

                    return IsOddNumber(gridXY.x) ? new Vector2Int(gridXY.x + 1, gridXY.y + 1) : new Vector2Int(gridXY.x + 1, gridXY.y);

                case EARoundType.Down:

                    return new Vector2Int(gridXY.x, gridXY.y + 1);

                case EARoundType.LeftDown:

                    return IsOddNumber(gridXY.x) ? new Vector2Int(gridXY.x - 1, gridXY.y + 1) : new Vector2Int(gridXY.x - 1, gridXY.y);

                case EARoundType.LeftUp:

                    return IsOddNumber(gridXY.x) ? new Vector2Int(gridXY.x - 1, gridXY.y) : new Vector2Int(gridXY.x - 1, gridXY.y - 1);
                default:
                    break;
            }

            Debugger.LogError($"未处理的六边形周围的格子类型:{around}");
            return Vector2Int.zero;
        }

#if UNITY_EDITOR

        public void SetActiveGridsXY(List<Vector2Int> activeGridsXY)
        {
            var childCount = gridsRoot.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var childTrans = gridsRoot.GetChild(i);
                var gridXY = GridIndex2XY(i);
                childTrans.gameObject.SetActive(activeGridsXY.Contains(gridXY));
            }
        }

        [FoldoutGroup("编辑器")]
        [Button("刷新")]
        public void Refresh()
        {
            GeneratorGrids(null, null);
        }
#endif
    }
}