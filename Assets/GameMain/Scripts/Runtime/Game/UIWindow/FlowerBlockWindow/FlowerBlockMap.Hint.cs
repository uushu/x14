using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameMain.FlowerBlock
{
    public class HintResult
    {
        //提示的滑块对象
        public ItemUI_FlowerBlock HintBlockItemUI;

        //横向移动的步数
        public int MoveStepCount;
    }

    public class ItemData_FlowerBlock
    {
        public List<Vector2Int> gridsXY;

        public ItemUI_FlowerBlock flowerBlockUI;
    }

    //底部的格子排在前面
    public class ItemData_FlowerBlockSort_DescY : IComparer<ItemData_FlowerBlock>
    {
        public int Compare(ItemData_FlowerBlock x, ItemData_FlowerBlock y)
        {
            if (x.gridsXY[0].y == y.gridsXY[0].y)
            {
                return x.gridsXY[0].x.CompareTo(y.gridsXY[0].y);
            }
            else if (x.gridsXY[0].y > y.gridsXY[0].y)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }

    //底部的格子排在后面
    public class ItemData_FlowerBlockSort_AsceY : IComparer<ItemData_FlowerBlock>
    {
        public int Compare(ItemData_FlowerBlock x, ItemData_FlowerBlock y)
        {
            if (x.gridsXY[0].y == y.gridsXY[0].y)
            {
                return x.gridsXY[0].x.CompareTo(y.gridsXY[0].y);
            }
            else if (x.gridsXY[0].y > y.gridsXY[0].y)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }

    //滑块消除预测提示
    public class FlowerBlockHintCore
    {
        private Dictionary<Vector2Int, ItemData_FlowerBlock> _gridXY2Block = new();
        private Dictionary<ItemData_FlowerBlock, List<Vector2Int>> _block2GridsXY = new();
        private List<ItemData_FlowerBlock> _inMapBlockDatas;

        private Dictionary<Vector2Int, ItemData_FlowerBlock> _gridXY2BlockSnapShot = new();
        private Dictionary<ItemData_FlowerBlock, List<Vector2Int>> _block2GridsXYSnapShot = new();

        private int MAX_GRID_Y_INDEX = 10;
        private int MAX_GRID_X_COUNT = 8;

        private void BuildBlockMapData(List<ItemUI_FlowerBlock> inMapBlockItems,
            List<RefreshBlockItemData> readyBlockItems, FlowerBlockMap blockMap)
        {
            _gridXY2Block.Clear();
            _block2GridsXY.Clear();

            _inMapBlockDatas = new List<ItemData_FlowerBlock>();
            foreach (var itemUI in inMapBlockItems)
            {
                _inMapBlockDatas.Add(new ItemData_FlowerBlock()
                {
                    flowerBlockUI = itemUI,
                });
            }

            //放入地图中,可检测移动的滑块
            foreach (var blockItemData in _inMapBlockDatas)
            {
                var blockGridXY = new List<Vector2Int>();

                foreach (var flower in blockItemData.flowerBlockUI.flowers)
                {
                    var gridXY = blockMap.uicustomGrids.TransformToGridXY(flower.transform);

                    if (_gridXY2Block.ContainsKey(gridXY))
                    {
                        Debugger.LogError($"blockItem={blockItemData.flowerBlockUI.name} gridXY={gridXY}");
                    }
                    else
                    {
                        _gridXY2Block.Add(gridXY, blockItemData);
                    }

                    blockGridXY.Add(gridXY);
                }

                blockItemData.gridsXY = blockGridXY;
                _block2GridsXY.Add(blockItemData, blockGridXY);
            }

            //不放入地图中,不可移动的滑块
            foreach (var readyBlock in readyBlockItems)
            {
                var gridsList = new List<Vector2Int>();
                var startX = readyBlock.startGridX;

                var readyBlockItemData = new ItemData_FlowerBlock() { flowerBlockUI = null };
                for (int i = 0; i < readyBlock.Config.GridData.Count; i++)
                {
                    var readyGridXY = new Vector2Int(startX + i, MAX_GRID_Y_INDEX);
                    gridsList.Add(readyGridXY);
                    _gridXY2Block.Add(readyGridXY, readyBlockItemData);
                }

                readyBlockItemData.gridsXY = gridsList;
                _block2GridsXY.Add(readyBlockItemData, gridsList);
            }

            _inMapBlockDatas.Sort(new ItemData_FlowerBlockSort_DescY());
        }

        private void DeepCopyMapData(ref Dictionary<Vector2Int, ItemData_FlowerBlock> gridXY2BlockSnapShot,
            ref Dictionary<ItemData_FlowerBlock, List<Vector2Int>> block2GridsXYSnapShot)
        {
            gridXY2BlockSnapShot.Clear();
            block2GridsXYSnapShot.Clear();

            foreach (var item in _gridXY2Block)
            {
                gridXY2BlockSnapShot.Add(item.Key, item.Value);
            }

            foreach (var item in _block2GridsXY)
            {
                var list = new List<Vector2Int>();
                list.AddRange(item.Value);
                block2GridsXYSnapShot.Add(item.Key, list);
            }
        }

        private bool IsCanLeftMoveTo(ItemData_FlowerBlock blockData, int xLeftStepCount, out int moveStepCount)
        {
            var gridsXY = _block2GridsXY[blockData];
            var headGridXY = gridsXY[0];

            for (int i = 1; i <= xLeftStepCount; i++)
            {
                var leftStep = new Vector2Int(i, 0);
                var moveToGridXY = headGridXY - leftStep;
                if (moveToGridXY.x < 0)
                {
                    moveStepCount = -1;
                    return false;
                }

                if (_gridXY2Block.TryGetValue(moveToGridXY, out var _))
                {
                    moveStepCount = -1;
                    //左边有滑块了
                    return false;
                }
            }

            moveStepCount = -xLeftStepCount;
            return true;
        }

        private bool IsCanRightMoveTo(ItemData_FlowerBlock blockData, int xRightStepCount, out int moveStepCount)
        {
            var gridsXY = _block2GridsXY[blockData];
            var lastGridXY = gridsXY.LastOrDefault();

            for (int i = 1; i <= xRightStepCount; i++)
            {
                var rightStep = new Vector2Int(i, 0);
                var moveToGridXY = lastGridXY + rightStep;

                if (moveToGridXY.x >= MAX_GRID_X_COUNT)
                {
                    moveStepCount = -1;
                    return false;
                }

                if (_gridXY2Block.TryGetValue(moveToGridXY, out var _))
                {
                    moveStepCount = -1;
                    //有滑块了
                    return false;
                }
            }

            moveStepCount = xRightStepCount;
            return true;
        }

        private void MoveBlockData(ItemData_FlowerBlock blockData, int moveStepCount,
            Dictionary<Vector2Int, ItemData_FlowerBlock> gridXY2BlockSnapShot,
            Dictionary<ItemData_FlowerBlock, List<Vector2Int>> block2GridsXYSnapShot)
        {
            var blockGridXYs = block2GridsXYSnapShot[blockData];

            //清理旧数据
            foreach (var grid in blockGridXYs)
            {
                gridXY2BlockSnapShot.Remove(grid);
            }

            //新的格子数据
            var moveStepGridIndex = new Vector2Int(moveStepCount, 0);
            for (int i = 0; i < blockGridXYs.Count; i++)
            {
                blockGridXYs[i] += moveStepGridIndex;
                gridXY2BlockSnapShot.Add(blockGridXYs[i], blockData);
            }
        }

        private bool RecursionEliminateBlocks(int maxGridIndex, Dictionary<Vector2Int, ItemData_FlowerBlock> gridXY2BlockSnapShot,
            Dictionary<ItemData_FlowerBlock, List<Vector2Int>> block2GridsXYSnapShot)
        {
            //开始下移格子,底部的格子开始判断
            foreach (var blockItemData in _inMapBlockDatas)
            {
                TryMoveToBottom(blockItemData, maxGridIndex, gridXY2BlockSnapShot, block2GridsXYSnapShot);
            }

            //判断是否可以消除
            for (int rowY = maxGridIndex; rowY >= 0; rowY--)
            {
                bool isRemoveRow = true;
                for (int rowX = 0; rowX < MAX_GRID_X_COUNT; rowX++)
                {
                    var gridXY = new Vector2Int(rowX, rowY);
                    if (gridXY2BlockSnapShot.ContainsKey(gridXY) == false)
                    {
                        isRemoveRow = false;
                        break;
                    }
                }

                if (isRemoveRow)
                {
                    return true;
                }
            }

            return false;
        }

        private void TryMoveToBottom(ItemData_FlowerBlock blockItemData, int maxGridIndex,
            Dictionary<Vector2Int, ItemData_FlowerBlock> gridXY2BlockSnapShot,
            Dictionary<ItemData_FlowerBlock, List<Vector2Int>> block2GridsXYSnapShot)
        {
            var gridXYs = block2GridsXYSnapShot[blockItemData];

            while (true)
            {
                for (int i = 0; i < gridXYs.Count; i++)
                {
                    var downGridXY = gridXYs[i] + new Vector2Int(0, 1);

                    if (downGridXY.y > maxGridIndex)
                    {
                        //已经是最底部
                        return;
                    }

                    if (gridXY2BlockSnapShot.ContainsKey(downGridXY))
                    {
                        return;
                    }
                }

                //全部下移动一个格子
                for (int i = 0; i < gridXYs.Count; i++)
                {
                    gridXY2BlockSnapShot.Remove(gridXYs[i]);
                    gridXYs[i] += new Vector2Int(0, 1);
                    gridXY2BlockSnapShot.Add(gridXYs[i], blockItemData);
                }
            }
        }

        private bool ReadyDataEliminateBlocks(int maxGridIndexY, ItemData_FlowerBlock blockData, int xMoveStepCount)
        {
            //当前地图的格子数据快照
            DeepCopyMapData(ref _gridXY2BlockSnapShot, ref _block2GridsXYSnapShot);

            //移动格子
            MoveBlockData(blockData, xMoveStepCount, _gridXY2BlockSnapShot, _block2GridsXYSnapShot);

            //开始消除
            var isRemove = RecursionEliminateBlocks(maxGridIndexY, _gridXY2BlockSnapShot, _block2GridsXYSnapShot);

            if (isRemove)
            {
                return true;
            }

            return false;
        }

        private bool CheckMoveBlockItemData(ItemData_FlowerBlock blockData, out int xMoveStepCount)
        {
            int maxMoveCount = 8;

            //判断左边移动
            for (int i = 1; i < maxMoveCount; i++)
            {
                var isCanMoveLeft = IsCanLeftMoveTo(blockData, i, out xMoveStepCount);

                if (isCanMoveLeft)
                {
                    //移动之后判断是否可以消除
                    var isRemove = ReadyDataEliminateBlocks(MAX_GRID_Y_INDEX - 1, blockData, xMoveStepCount);

                    if (isRemove)
                    {
                        return true;
                    }

                    //向上顶一个在判断
                    isRemove = ReadyDataEliminateBlocks(MAX_GRID_Y_INDEX, blockData, xMoveStepCount);

                    if (isRemove)
                    {
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }

            //判断右边移动
            for (int i = 1; i < maxMoveCount; i++)
            {
                var isCanMoveRight = IsCanRightMoveTo(blockData, i, out xMoveStepCount);

                if (isCanMoveRight)
                {
                    //移动之后判断是否可以消除
                    var isRemove = ReadyDataEliminateBlocks(MAX_GRID_Y_INDEX - 1, blockData, xMoveStepCount);

                    if (isRemove)
                    {
                        return true;
                    }

                    //向上顶一个在判断
                    isRemove = ReadyDataEliminateBlocks(MAX_GRID_Y_INDEX, blockData, xMoveStepCount);

                    if (isRemove)
                    {
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }

            xMoveStepCount = -1;
            return false;
        }

        public HintResult GetHintResult(List<ItemUI_FlowerBlock> inMapBlockItems,
            List<RefreshBlockItemData> readyBlockItems, FlowerBlockMap blockMap)
        {
            //更新UI数据 => 逻辑数据
            BuildBlockMapData(inMapBlockItems, readyBlockItems, blockMap);

            //顶部开始判断
            var cloneInMapBlockDatas = new List<ItemData_FlowerBlock>();
            cloneInMapBlockDatas.AddRange(_inMapBlockDatas);

            //Y越大的在后面
            cloneInMapBlockDatas.Sort(new ItemData_FlowerBlockSort_AsceY());

            //左上开始移动位置判断是否可以消除
            foreach (var blockItemData in cloneInMapBlockDatas)
            {
                var isCanRemove = CheckMoveBlockItemData(blockItemData, out int moveStepCount);

                if (isCanRemove)
                {
                    return new HintResult()
                    {
                        HintBlockItemUI = blockItemData.flowerBlockUI,
                        MoveStepCount = moveStepCount
                    };
                }
            }

            return null;
        }
    }

    public partial class FlowerBlockMap
    {
        private FlowerBlockHintCore _hintCore;
        public FlowerBlockHintCore HintCore
        {
            get
            {
                if (_hintCore == null)
                {
                    _hintCore = new();
                }

                return _hintCore;
            }
        }

        //根据当前的滑块信息,计算玩家可以移动一步达成消除
        public HintResult CalcFlowerBlockHintResult()
        {
            var inMapBlockItemUI = inMapBlockItems;
            var readyBlockItem = BlockGameRoot.Instance.waitBlock.NextRefreshResult;

            return HintCore.GetHintResult(inMapBlockItemUI, readyBlockItem, this);
        }
    }
}