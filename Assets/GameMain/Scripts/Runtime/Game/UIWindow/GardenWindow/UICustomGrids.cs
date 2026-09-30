using cfg;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace GameMain.Garden
{
    public class UICustomGrids : MonoBehaviour
    {
        [LabelText("生成根节点")]
        public Transform generatorRoot;

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

        public float Heigth => (gridHeight + rowSpace) * rowCount - rowSpace;
        public float Width => (gridWidth + colSpace) * colCount - colSpace;
        public int MaxIndex => rowCount * colCount - 1;
        private float _startX => -(colCount - 1) * 0.5f * (gridWidth + colSpace);
        private float _startY => (rowCount - 1) * 0.5f * (gridHeight + rowSpace);

        //是否正在旋转
        private bool _isRotating = false;

        private Dictionary<GameObject, Vector2Int> _obj2XY = new();

        public void GetGridXY(ref List<Vector2Int> gridXYs)
        {
            gridXYs.Clear();

            foreach (var item in _obj2XY)
            {
                gridXYs.Add(item.Value);
            }
        }

        public async UniTask GeneratorGrids(List<int> gridsIndex, int gridRowCount, int gridColCount,
            List<int> flowersId, Func<int, UniTask<GameObject>> createGridObj)
        {
            _isRotating = false;
            _obj2XY.Clear();

            rowCount = gridRowCount;
            colCount = gridColCount;
            var gridsXY = ListToGridXY(gridsIndex, rowCount, colCount);

            int flowerIndex = 0;
            for (int Y = 0; Y < rowCount; Y++)
            {
                for (int X = 0; X < colCount; X++)
                {
                    var gridXY = new Vector2Int(X, Y);
                    if (gridsXY.Contains(gridXY) == false)
                    {
                        continue;
                    }

                    var flowerId = flowersId[flowerIndex];
                    flowerIndex++;

                    var gridInstance = await createGridObj.Invoke(flowerId);
                    var localPosition = GridXYToLocalPosition(gridXY);
                    gridInstance.transform.SetParent(generatorRoot);
                    gridInstance.transform.localScale = Vector3.one;
                    gridInstance.transform.localPosition = localPosition;
                    gridInstance.transform.name = $"{gridInstance.name}-{X}-{Y}";

                    _obj2XY.Add(gridInstance, gridXY);
                }
            }

            RefreshRectSize();
        }

        public void GeneratorGrids(Action<Vector2Int, GameObject> onCreateGrid, List<Vector2Int> gridXYs)
        {
            generatorRoot.gameObject.DestroyAllChildren();

            _obj2XY.Clear();

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

                    gridInstance = GameObject.Instantiate(gridItemPrefab, generatorRoot);

                    _obj2XY.Add(gridInstance, gridXY);
                    var localPosition = GridXYToLocalPosition(gridXY);

                    gridInstance.transform.SetParent(generatorRoot);
                    gridInstance.transform.localScale = Vector3.one;
                    gridInstance.gameObject.SetActive(true);
                    gridInstance.transform.localPosition = localPosition;
                    gridInstance.transform.name = $"{gridItemPrefab.name}-{X}-{Y}";
                    onCreateGrid?.Invoke(gridXY, gridInstance);
                }
            }

            RefreshRectSize();
        }

        private void RefreshRectSize()
        {
            generatorRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(Width, Heigth);
        }

        public GameObject GetObjByGridXY(Vector2Int gridXY)
        {
            foreach (var item in _obj2XY)
            {
                if (item.Value == gridXY)
                { 
                    return item.Key;
                }
            }

            return null;
        }

        //必须每个格子都有数据
        public Transform GetObjByGridXY2(Vector2Int gridXY)
        {
            var childName = $"{gridItemPrefab.name}-{gridXY.x}-{gridXY.y}";
            return generatorRoot.Find(childName);
        }

        public void RefreshRowColSpace(float _rowSpace, float _colSpace)
        {
            //先获取运行时的格子数据
            var list = ListPool<Vector2Int>.Get();
            GetGridXY(ref list);

            //在设置,行列的间隔
            this.rowSpace = _rowSpace;
            this.colSpace = _colSpace;

            //右下角为起点
            var startX = _startX;
            var startY = _startY;

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    var x = startX + j * (gridWidth + colSpace);
                    var y = startY - i * (gridHeight + rowSpace);
                    var gridXY = new Vector2Int(j, i);
                    if (list.Contains(gridXY) == false) continue;

                    var gridObj = GetObjByGridXY(gridXY);
                    gridObj.transform.localPosition = new Vector3(x, y);
                }
            }

            RefreshRectSize();
            ListPool<Vector2Int>.Release(list);
        }

        public Vector2Int TransformToGridXY(Transform trans)
        {
            var localPoint = generatorRoot.InverseTransformPoint(trans.position);
            return LocalPosToGridXY(localPoint);
        }

        public Vector2Int LocalPosToGridXY(Vector3 localPoint)
        {
            var itemWidth = gridWidth;
            var itemHeight = gridHeight;

            var startX = -colCount * 0.5f * (itemWidth + colSpace);
            var startY = rowCount * 0.5f * (itemHeight + rowSpace);

            localPoint -= new Vector3(startX, startY, 0);
            var xIndex = Mathf.FloorToInt(localPoint.x / (itemWidth + colSpace));
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

        public Vector2Int GridIndex2XY(int gridIndex)
        {
            return new Vector2Int(gridIndex % colCount, gridIndex / colCount);
        }

        public int GridXY2Index(Vector2Int xy)
        {
            return xy.y * colCount + xy.x;
        }

        //左下角起点
        public Transform GetChildByIndex(Vector2Int gridXY)
        {
            if (GridIndexIsValid(gridXY) == false)
            {
                Debugger.LogError($"超出了地图范围:{gridXY}");
                return null;
            }

            var childIndex = GridXY2Index(gridXY);
            return generatorRoot.GetChild(childIndex);
        }

        [Obsolete]
        public void ClockwiseRotate90(bool clockwise)
        {
            var tempObj2XY = new Dictionary<GameObject, Vector2Int>(_obj2XY.Count);

            foreach (var item in _obj2XY)
            {
                var beforeXY = item.Value;
                var gridObj = item.Key;

                Vector2Int rotateAfterXY = default;
                if (clockwise)
                {
                    rotateAfterXY = new Vector2Int(rowCount - beforeXY.y - 1, beforeXY.x);
                }
                else
                {
                    rotateAfterXY = new Vector2Int(beforeXY.y, colCount - beforeXY.x - 1);
                }

                tempObj2XY.Add(gridObj, rotateAfterXY);
            }

            _obj2XY.Clear();
            _obj2XY = tempObj2XY;

            //交换行列 
            var tempRowCount = this.rowCount;
            this.rowCount = colCount;
            this.colCount = tempRowCount;

            foreach (var item in _obj2XY)
            {
                item.Key.transform.localPosition = GridXYToLocalPosition(item.Value);
            }

            RefreshRectSize();
        }

        public void RoateSelf90(bool clockwise, float animationTime, Action onComplete)
        {
            if (_isRotating) return;

            float rotateAngleZ = -90f * (clockwise ? 1 : -1);
            _isRotating = true;

            float currentValue = 0;
            DOTween.To(() => currentValue, (value) =>
            {
                var delta = value - currentValue;
                currentValue = value;

                foreach (var item in _obj2XY)
                {
                    item.Key.transform.RotateAround(transform.position, Vector3.forward, delta);
                    item.Key.transform.localEulerAngles = Vector3.zero;
                }

            }, rotateAngleZ, animationTime).OnComplete(() =>
            {
                _isRotating = false;
                onComplete?.Invoke();
            });

            var tempObj2XY = new Dictionary<GameObject, Vector2Int>(_obj2XY.Count);

            foreach (var item in _obj2XY)
            {
                var beforeXY = item.Value;
                var gridObj = item.Key;

                Vector2Int rotateAfterXY = default;
                if (clockwise)
                {
                    rotateAfterXY = new Vector2Int(rowCount - beforeXY.y - 1, beforeXY.x);
                }
                else
                {
                    rotateAfterXY = new Vector2Int(beforeXY.y, colCount - beforeXY.x - 1);
                }

                tempObj2XY.Add(gridObj, rotateAfterXY);
            }

            _obj2XY.Clear();
            _obj2XY = tempObj2XY;

            //交换行列 
            var tempRowCount = this.rowCount;
            this.rowCount = colCount;
            this.colCount = tempRowCount;
        }

        public List<Vector2Int> ListToGridXY(List<int> gridIndexs, int rowCount, int colCount)
        {
            var gridsXY = new List<Vector2Int>();
            ListToGridXYNonAlloc(gridIndexs, rowCount, colCount, ref gridsXY);
            return gridsXY;
        }

        public void ListToGridXYNonAlloc(List<int> gridIndexs, int rowCount, int colCount, ref List<Vector2Int> gridsXY)
        {
            gridsXY.Clear();

            for (int y = 0; y < rowCount; y++)
            {
                for (int x = 0; x < colCount; x++)
                {
                    var gridIndex = colCount * y + x;
                    if (gridIndexs[gridIndex] <= 0)
                    {
                        continue;
                    }

                    var gridXY = new Vector2Int(x, y);
                    gridsXY.Add(gridXY);
                }
            }
        }

#if UNITY_EDITOR

        public Action<Vector2Int, GameObject> OnCreateGridObj { get; set; }

        [FoldoutGroup("编辑器")]
        [Button("刷新")]
        private void Refresh()
        {
            GeneratorGrids(OnCreateGridObj, null);
        }

        [FoldoutGroup("编辑器")]
        [Button("清除全部格子")]
        private void ClearAll()
        {
            generatorRoot.gameObject.DestroyAllChildren();
        }

        [FoldoutGroup("编辑器")]
        public Transform p1;

        [FoldoutGroup("编辑器")]
        [Button("测试-TransformToGridIndex")]
        private void Test1()
        {
            var index = TransformToGridXY(p1);
            Debug.Log($"index = {index}");
        }

        [FoldoutGroup("编辑器")]
        public Vector2Int testGridXY;

        [FoldoutGroup("编辑器")]
        [Button("测试-GridIndexToLocalPosition")]
        private void Test2()
        {
            var localPosition = GridXYToLocalPosition(testGridXY);
            Debug.Log($"localPosition = {localPosition}");
        }

        [FoldoutGroup("编辑器")]
        [Button("测试-刷新间距")]
        private void TestRefreshSpace()
        {
            RefreshRowColSpace(-20, -20);
        }
#endif
    }
}