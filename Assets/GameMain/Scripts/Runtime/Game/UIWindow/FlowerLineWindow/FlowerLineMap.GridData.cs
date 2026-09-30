using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace GameMain.FlowerLine
{
    public enum EGridType
    {
        [LabelText("障碍物")]
        Obstacle,
        [LabelText("空格子")]
        Empty,
        [LabelText("鲜花")]
        Flower,
    }

    public class GridData
    { 
        public EGridType gridType;

        public Line_FlowerSingle flower;

        public Vector2Int gridXY;
    }

    //升序Y排序
    public class GridDataSortBy_AsceY : IComparer<GridData>
    {
        //降序排序
        public int Compare(GridData x, GridData y)
        {
            if (x.gridXY.y == y.gridXY.y)
            {
                if (x.flower.transform.localPosition.y > y.flower.transform.localPosition.y)
                {
                    return 1;
                }
                else
                {
                    return x.gridXY.x.CompareTo(y.gridXY.x);
                }
            }
            else if (x.gridXY.y > y.gridXY.y)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }

    //降序Y排序
    public class GridDataSortBy_DescY : IComparer<GridData>
    {
        //降序排序
        public int Compare(GridData x, GridData y)
        {
            if (x.flower.transform.localPosition.y == y.flower.transform.localPosition.y)
            {
                return x.gridXY.x.CompareTo(y.gridXY.x);
            }
            else
            {
                return y.flower.transform.localPosition.y.CompareTo(x.flower.transform.localPosition.y);
            }
        }
    }

    public partial class FlowerLineMap
    {
        private Dictionary<Vector2Int, GridData> _xy2GridData;

        //激活的格子数量
        public int EnableGridCount { get; private set; }

        //鲜花连线可移除的路径
        private List<List<Vector2Int>> _lineFlowerRemovePath = new();

        //创建格子的数据
        private void Initizlie_MapData()
        {
            _xy2GridData = new();

            var childCount = hexagonGrids.gridsRoot.childCount;

            for (int i = 0; i < childCount; i++)
            {
                var gridChild = hexagonGrids.gridsRoot.GetChild(i);
                var gridXY = hexagonGrids.GridIndex2XY(i);

                if (gridChild.gameObject.activeSelf)
                {
                    _xy2GridData.Add(gridXY, new GridData()
                    {
                        gridType = EGridType.Empty,
                        flower = null,
                        gridXY = gridXY,
                    });

                    EnableGridCount++;
                }
                else
                {
                    //暂时是障碍物
                    _xy2GridData.Add(gridXY, new GridData()
                    {
                        gridType = EGridType.Obstacle,
                        flower = null,
                        gridXY = gridXY,
                    });
                }
            }
        }

        public Line_FlowerSingle TryGetFlower(Vector2Int gridXY) 
        {
            if (_xy2GridData.TryGetValue(gridXY, out var gridData))
            {
                if (gridData.gridType == EGridType.Obstacle)
                {
                    return null;
                }
                else
                {
                    return gridData.flower;
                }
            }

            return null;
        }

        public bool GridHasFlower(Vector2Int gridXY)
        {
            return TryGetFlower(gridXY) != null;
        }

        public bool IsInMap(Vector2Int gridXY)
        {
            if (_xy2GridData.TryGetValue(gridXY, out var gridData))
            {
                if (gridData.gridType == EGridType.Obstacle)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        private bool CalcFlowerNextMovePoint(Vector2Int currentPoint, out Vector2Int nextPoint)
        {
            //1.先判断下降的
            var downPoint = hexagonGrids.GetARoundGridXY(currentPoint, EARoundType.Down);
            if (MapGridXYIsEmpty(downPoint))
            {
                nextPoint = downPoint;
                return true;
            }

            //2.在判断左下边
            var leftDown = hexagonGrids.GetARoundGridXY(currentPoint, EARoundType.LeftDown);
            if (MapGridXYIsEmpty(leftDown))
            {
                var up = hexagonGrids.GetARoundGridXY(leftDown, EARoundType.Up);
                if (GridHasFlower(up) == false)
                {
                    nextPoint = leftDown;
                    return true;
                }
            }

            //3.在判断右下边
            var rightDown = hexagonGrids.GetARoundGridXY(currentPoint, EARoundType.RightDown);
            if (MapGridXYIsEmpty(rightDown))
            {
                var up = hexagonGrids.GetARoundGridXY(rightDown, EARoundType.Up);
                if (GridHasFlower(up) == false)
                {
                    nextPoint = rightDown;
                    return true;
                }
            }

            nextPoint = Vector2Int.zero;
            return false;
        }

        public bool IsAronudPoint(Vector2Int currentGridXY, Vector2Int checkPoint)
        {
            var list = ListPool<Vector2Int>.Get();

            GetAroundGridsXY(currentGridXY, ref list);

            bool isAroundPoint = false;
            foreach (var gridXY in list)
            {
                if (gridXY == checkPoint)
                {
                    isAroundPoint = true;
                    break;
                }
            }

            ListPool<Vector2Int>.Release(list);
            return isAroundPoint;
        }

        public void GetAroundGridsXY(Vector2Int currentPoint, ref List<Vector2Int> aroundGridsXY)
        {
            aroundGridsXY.Clear();

            var list = ListPool<EARoundType>.Get();

            list.Add(EARoundType.Up);
            list.Add(EARoundType.Down);
            list.Add(EARoundType.LeftDown);
            list.Add(EARoundType.RightDown);
            list.Add(EARoundType.LeftUp);
            list.Add(EARoundType.RightUp);

            foreach (var direction in list)
            {
                var aroundGridXY = hexagonGrids.GetARoundGridXY(currentPoint, direction);
                if (IsInMap(aroundGridXY))
                {
                    aroundGridsXY.Add(aroundGridXY);
                }
            }

            ListPool<EARoundType>.Release(list);
        }

        private bool MapGridXYIsEmpty(Vector2Int gridXY)
        {
            //障碍物
            var isValid = IsInMap(gridXY);
            if (isValid == false)
            {
                return false;
            }

            //这个位置已经有鲜花
            if (GridHasFlower(gridXY))
            {
                return false;
            }

            return true;
        }

        private void SetGridFlowerNull(Vector2Int gridXY, bool hideUI)
        {
            if (_xy2GridData.TryGetValue(gridXY, out var gridData))
            {
                if (gridData.flower != null)
                {
                    if (hideUI)
                    {
                        FlowerLineGameRoot.Instance.pool.multiPools.PushToPool(gridData.flower);
                    }
                    gridData.flower = null;
                }
                else
                {
                    Debugger.LogError($"当前格子没有鲜花 gridXY:{gridXY}");
                }
            }
            else
            {
                Debugger.LogError($"当前格子不是鲜花格子 gridXY:{gridXY}");
            }
        }

        private void GeneratorNewFlowers(int flowersCount)
        {
            Debugger.Log($"生成新的花朵 flowersCount={flowersCount}");
            TryAddFlowerToMap(flowersCount).Forget();
        }

        private void TryRecordDragLinePath(Line_FlowerSingle startFlower, HashSet<Vector2Int> path)
        {
            var list = ListPool<Vector2Int>.Get();
            _lineFlowerRemovePath.Add(list);
            list.AddRange(path);
        }

        private void ClearRecordDragLinePath()
        {
            foreach (var list in _lineFlowerRemovePath)
            {
                ListPool<Vector2Int>.Release(list);
            }

            _lineFlowerRemovePath.Clear();
        }

        private void ListSub<T>(List<T> list1, HashSet<T> list2)
        {
            foreach (var item in list2)
            {
                list1.Remove(item);
            }
        }

        private void TryAddLineFlower(Vector2Int gridXY, Line_FlowerSingle rootFlower, ref HashSet<Vector2Int> path)
        {
            path.Add(gridXY);

            var aroundGridsXY = ListPool<Vector2Int>.Get();
            GetAroundGridsXY(gridXY, ref aroundGridsXY);
            ListSub(aroundGridsXY, path);

            for (int i = aroundGridsXY.Count - 1; i >= 0; i--)
            {
                var aroundGridXY = aroundGridsXY[i];

                var aroundFlower = _xy2GridData[aroundGridXY].flower;
                if (rootFlower.FlowerConfigId != aroundFlower.FlowerConfigId)
                {
                    aroundGridsXY.RemoveAt(i);
                    //格子类型不一致
                    continue;
                }

                path.Add(aroundGridXY);
                TryAddLineFlower(aroundGridXY, rootFlower, ref path);
                path.Remove(aroundGridXY);
            }

            if (aroundGridsXY.Count == 0)
            {
                //最后一个了
                TryRecordDragLinePath(rootFlower, path);
            }

            ListPool<Vector2Int>.Release(aroundGridsXY);
        }

        private List<List<Vector2Int>> GetCanDragLineFlowers(Vector2Int startGridXY, Line_FlowerSingle startFlower)
        {
            ClearRecordDragLinePath();
            var aroundGridsXY = ListPool<Vector2Int>.Get();
            var path = HashSetPool<Vector2Int>.Get();

            TryAddLineFlower(startGridXY, startFlower, ref path);

            ListPool<Vector2Int>.Release(aroundGridsXY);
            HashSetPool<Vector2Int>.Release(path);
            return _lineFlowerRemovePath;
        }

        private List<Vector2Int> GetMaxPath(List<List<Vector2Int>> paths)
        {
            int maxIndex = 0;
            for (int i = 0; i < paths.Count; i++)
            {
                if (paths[i].Count > paths[maxIndex].Count)
                { 
                    maxIndex = i;
                }
            }
            return paths[maxIndex];
        }

        private bool GameIsFailed()
        {
            var flowerGrids = GetAllFlowerGrids();
            flowerGrids.Sort(new GridDataSortBy_DescY());

            foreach (var item in flowerGrids)
            {
                var flower = item.flower;
                var linePath = GetCanDragLineFlowers(item.flower.currentGridXY, flower);
                var maxPath = GetMaxPath(linePath);

                if (maxPath.Count >= DragLineListener.MIN_LINE_GRID_COUNT)
                {
                    return false;
                }
            }
            return true;
        }
    }
}