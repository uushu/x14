using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace GameMain.FlowerBlock
{
    //底部的格子排在前面
    public class ItemUI_FlowerBlockSort : IComparer<ItemUI_FlowerBlock>
    {
        public int Compare(ItemUI_FlowerBlock x, ItemUI_FlowerBlock y)
        {
            if (x.transform.localPosition.y == y.transform.localPosition.y)
            {
                return x.transform.localPosition.x.CompareTo(y.transform.localPosition.x);
            }
            else if (x.transform.localPosition.y > y.transform.localPosition.y)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }

    public class FlowerBlockRemoveResult
    {
        //消除的行
        public List<int> removeRowsIndex = new();

        //消除的滑块
        public List<ItemUI_FlowerBlock> removeBlockItems = new();

        //落下的方块,完成的消除(动画需求)
        public Dictionary<int, ItemUI_FlowerBlock> downBlockItems = new();

        public void Clear()
        {
            removeRowsIndex.Clear();
            removeBlockItems.Clear();
            downBlockItems.Clear();
        }

        public void TryAddRemoveBlockItem(ItemUI_FlowerBlock removeBlockItem)
        {
            if (removeBlockItems.Contains(removeBlockItem)) return;

            removeBlockItems.Add(removeBlockItem);
        }

        public void TryAddDownBlockItem(int rowIndex, ItemUI_FlowerBlock downBlockItem)
        {
            if (downBlockItems.ContainsKey(rowIndex))
            {
                return;
            }

            downBlockItems.Add(rowIndex, downBlockItem);
        }
    }

    public partial class FlowerBlockMap
    {
        private Dictionary<Vector2Int, ItemUI_FlowerBlock> _gridXY2Block = new();
        private Dictionary<ItemUI_FlowerBlock, List<Vector2Int>> _block2GridsXY = new();

        [LabelText("在地图上的格子")]
        public List<ItemUI_FlowerBlock> inMapBlockItems;

        private FlowerBlockRemoveResult _removeResult = new();

        private List<int> _rows;

        public Action<FlowerBlockRemoveResult> OnBeforeRemoveFlowerBlockCallBack { get; set; }
        public Action<FlowerBlockRemoveResult> OnAfterRemoveFlowerBlockCallBack { get; set; }

        public Action OnFlowerBlockTopFullCallBack { get; set; }

        public void Initialize_Data()
        {
            _gridXY2Block = new();
            _rows = new();
        }

        private void UpdateBlockMapData()
        {
            _gridXY2Block.Clear();
            foreach (var item in _block2GridsXY)
            {
                ListPool<Vector2Int>.Release(item.Value);
            }
            _block2GridsXY.Clear();

            foreach (var blockItem in inMapBlockItems)
            {
                var blockGridXY = ListPool<Vector2Int>.Get();
                blockGridXY.Clear();

                foreach (var flower in blockItem.flowers)
                {
                    var gridXY = uicustomGrids.TransformToGridXY(flower.transform);

                    if (_gridXY2Block.ContainsKey(gridXY))
                    {
                        Debugger.LogError($"blockItem={blockItem.name} gridXY={gridXY}");
                    }
                    else
                    {
                        _gridXY2Block.Add(gridXY, blockItem);
                    }

                    blockGridXY.Add(gridXY);
                }

                _block2GridsXY.Add(blockItem, blockGridXY);
            }
        }

        private void SortInMapBlockItemsByY()
        {
            inMapBlockItems.Sort(new ItemUI_FlowerBlockSort());
        }

        private List<int> GetRemoveBlockRows()
        {
            _rows.Clear();

            //每一行是否已满
            for (int y = 0; y < uicustomGrids.rowCount; y++)
            {
                bool isRemove = true;

                for (int x = 0; x < uicustomGrids.colCount; x++)
                {
                    var gridXY = new Vector2Int(x, y);
                    if (_gridXY2Block.ContainsKey(gridXY) == false)
                    {
                        //未满
                        isRemove = false;
                        break;
                    }
                }

                if (isRemove)
                {
                    _rows.Add(y);
                }
            }

            return _rows;
        }

        private void GetAroundBlockItems(ItemUI_FlowerBlock center, ref List<ItemUI_FlowerBlock> result)
        {
            result.Clear();

            if (_block2GridsXY.TryGetValue(center, out var list))
            {
                foreach (var gridXY in list)
                {
                    var aroundXYs = ListPool<Vector2Int>.Get();

                    var upGridXY = gridXY + new Vector2Int(0, 1);
                    aroundXYs.Add(upGridXY);
                    var downGridXY = gridXY + new Vector2Int(0, -1);
                    aroundXYs.Add(downGridXY);
                    var leftGridXY = gridXY + new Vector2Int(-1, 0);
                    aroundXYs.Add(leftGridXY);
                    var rightGridXY = gridXY + new Vector2Int(1, 0);
                    aroundXYs.Add(rightGridXY);

                    for (int i = 0; i < aroundXYs.Count; i++)
                    {
                        if (_gridXY2Block.TryGetValue(aroundXYs[i], out var blockItem))
                        {
                            if (blockItem != center && result.Contains(blockItem) == false)
                            {
                                result.Add(blockItem);
                            }
                        }
                    }

                    ListPool<Vector2Int>.Release(aroundXYs);
                }
            }
            else
            {
                Debugger.LogError($"不存在的滑块:{center.name}");
            }
        }

        public void RemoveBlockItem(ItemUI_FlowerBlock removeBlockItem, bool pushToPool)
        {
            var removeResult = inMapBlockItems.Remove(removeBlockItem);

            if (pushToPool)
            {
                BlockGameRoot.Instance.pool.PushGridTypeToPool(removeBlockItem);
            }

            if (removeResult == false)
            {
                Debugger.LogError($"地图中,不存在的滑块:{removeBlockItem.name}");
            }
        }

        public void AddBlockItem(ItemUI_FlowerBlock addBlockItem)
        {
            if (inMapBlockItems.Contains(addBlockItem))
            {
                Debugger.LogError($"地图中已经存在的滑块:{addBlockItem.name}");
                return;
            }
            inMapBlockItems.Add(addBlockItem);
            addBlockItem.transform.SetAsLastSibling();
        }

        private Vector2Int GetFlowerBlockMinX(ItemUI_FlowerBlock blockItem)
        {
            Vector2Int result = new Vector2Int(int.MaxValue, int.MaxValue);
            
            foreach (var item in _gridXY2Block)
            {
                if (item.Value == blockItem)
                {
                    if (item.Key.x < result.x)
                    {
                        result = item.Key;
                    }
                }
            }

            return result;
        }

        //滑块的X滑动的范围
        private (int, int) GetBlockItemClampX(ItemUI_FlowerBlock blockItem)
        {
            int resultXMin = 0, resultXMax = 0;

            var gridCount = blockItem.flowers.Count;
            var headGridXY = GetFlowerBlockMinX(blockItem);
            var leftX = headGridXY.x;
            leftX = Mathf.Clamp(leftX, 0, leftX + gridCount);

            var rightX = leftX + gridCount;

            resultXMin = leftX;
            resultXMax = leftX;

            //判断最左边是否有滑块
            for (int i = leftX - 1; i >= 0; i--)
            {
                var gridXY = new Vector2Int(i, headGridXY.y);
                if (_gridXY2Block.ContainsKey(gridXY) == false)
                {
                    resultXMin--;
                }
                else
                {
                    break;
                }
            }

            //判断最右边是否有滑块
            for (int i = rightX; i < uicustomGrids.colCount; i++)
            {
                var gridXY = new Vector2Int(i, headGridXY.y);
                if (_gridXY2Block.ContainsKey(gridXY) == false)
                {
                    resultXMax++;
                }
                else
                {
                    break;
                }
            }

            resultXMin = Mathf.Max(resultXMin, 0);
            resultXMax = Mathf.Min(resultXMax, uicustomGrids.colCount - 1);

            return (resultXMin, resultXMax);
        }

        //判断游戏滑块的累计高度 0开始
        public int GetMaxBlockYIndex()
        {
            if (inMapBlockItems.Count <= 0)
            {
                return 0;
            }

            SortInMapBlockItemsByY();
            var topBlockItem = inMapBlockItems.LastOrDefault();
            var gridXY = uicustomGrids.TransformToGridXY(topBlockItem.transform);

            return uicustomGrids.rowCount - gridXY.y - 1;
        }

        private void DictMergeTo(Dictionary<ItemUI_FlowerBlock, int> fromDict, Dictionary<ItemUI_FlowerBlock, int> toDict)
        {
            foreach (var item in fromDict)
            {
                if (toDict.ContainsKey(item.Key))
                {
                    var finalValue = item.Value + toDict[item.Key];

                    toDict[item.Key] += item.Value;
                }
                else
                {
                    toDict.Add(item.Key, item.Value);
                }
            }
        }

        private bool TopIsFull()
        {
            //5.判断游戏是否失败
            var maxYIndex = GetMaxBlockYIndex();
            if (maxYIndex >= uicustomGrids.rowCount - 1)
            {
                return true;
            }
            return false;
        }

        private bool MapIsEmpty()
        {
            return inMapBlockItems.Count <= 0;
        }

        private void UpdateGridDataDownOneRow(ItemUI_FlowerBlock blockItem)
        {
            var gridXYs = _block2GridsXY[blockItem];

            var tempExchange = ListPool<Vector2Int>.Get();
            tempExchange.Clear();
            foreach (var item in gridXYs)
            {
                tempExchange.Add(new Vector2Int(item.x, item.y + 1));
            }

            foreach (var gridXY in gridXYs)
            {
                _gridXY2Block.Remove(gridXY);
            }

            foreach (var gridXY in tempExchange)
            {
                if (_gridXY2Block.ContainsKey(gridXY))
                {
                    Debugger.LogError($"更新格子数据错误 gridXY:{gridXY} 当前位置有格子");
                    _gridXY2Block.Remove(gridXY);
                }

                _gridXY2Block.Add(gridXY, blockItem);
            }

            gridXYs.Clear();
            gridXYs.AddRange(tempExchange);
            ListPool<Vector2Int>.Release(tempExchange);
        }

        private void RecordGridDownStep(ref Dictionary<ItemUI_FlowerBlock, int> downDict)
        {
            //最下面的开始掉落判断
            SortInMapBlockItemsByY();

            foreach (var blockItem in inMapBlockItems)
            {
                //下落最底部
                bool isSuccess = false;
                int maxRecusionCount = uicustomGrids.rowCount;
                do
                {
                    isSuccess = BlockItemCanMoveDownOneRow(blockItem);

                    if (isSuccess)
                    {
                        //更新格子数据 下落一行
                        UpdateGridDataDownOneRow(blockItem);

                        //记录下降次数
                        if (downDict.TryGetValue(blockItem, out var downCount) == false)
                        {
                            downDict.Add(blockItem, 1);
                        }
                        else
                        {
                            downDict[blockItem] = downCount + 1;
                        }
                    }

                } while (isSuccess && downDict[blockItem] <= maxRecusionCount);
            }
        }

        //判断滑块是否可以下落一行
        private bool BlockItemCanMoveDownOneRow(ItemUI_FlowerBlock blockItem)
        {
            var gridXYs = _block2GridsXY[blockItem];

            foreach (var gridXY in gridXYs)
            {
                if (gridXY.y >= (uicustomGrids.rowCount - 1))
                {
                    return false;
                }

                var downGrixXY = new Vector2Int(gridXY.x, gridXY.y + 1);
                if (_gridXY2Block.ContainsKey(downGrixXY))
                {
                    return false;
                }
            }

            return true;
        }

        public void UnInitialize_Data() 
        {
            _gridXY2Block.Clear();
            _gridXY2Block = null;
        }

        public void ClearMapData()
        {
            _gridXY2Block.Clear();
            inMapBlockItems.Clear();
        }
    }
}