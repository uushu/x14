using Codice.CM.Common;
using Cysharp.Threading.Tasks;
using GameMain.Garden;
using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.FlowerBlock
{
    public partial class FlowerBlockMap
    {
        private void OnAfterUseBagPopEliminateBlock()
        {
            //消除逻辑
            BlockGameRoot.Instance.StartAnimation();
            TryEliminateBlock(false, () =>
            {
                BlockGameRoot.Instance.StopAnimation();
            });
        }

        private List<ItemUI_FlowerBlock> GetTopRowBlockItems(int topRowCount)
        {
            var result = new List<ItemUI_FlowerBlock>();

            SortInMapBlockItemsByY();

            int lastAddGridY = -1;
            int alreadyAddRowCount = 0;

            for (int i = inMapBlockItems.Count - 1; i >= 0; i--)
            {
                var blockItem = inMapBlockItems[i];
                _block2GridsXY.TryGetValue(blockItem, out var list);

                if (lastAddGridY == -1)
                {
                    result.Add(blockItem);

                    //更新Y
                    lastAddGridY = list[0].y;
                    alreadyAddRowCount++;
                }
                else
                {
                    if (lastAddGridY == list[0].y)
                    {
                        result.Add(blockItem);
                    }
                    else
                    {
                        alreadyAddRowCount++;
                        if (alreadyAddRowCount > topRowCount)
                        {
                            break;
                        }

                        //更新Y
                        lastAddGridY = list[0].y;
                        result.Add(blockItem);
                    }
                }
            }

            return result;
        }

        public async void OnUseCurshBagProp()
        {
            var splits = new List<ItemUI_FlowerBlock>();

            //规则；击碎3,4格子的长方块 3=1+2 4=2+2
            foreach (var blockItem in _block2GridsXY)
            {
                if (blockItem.Value.Count <= 2)
                {
                    continue;
                }

                splits.Add(blockItem.Key);
            }

            if (splits.Count <= 0) return;
            BlockGameRoot.Instance.StartAnimation();

            foreach (var blockItem in splits)
            {
                //播放击碎的动画
                PlayAnimation(blockItem, BlockGameRoot.Instance.pool.crushPrefab, () =>
                {
                    SpliteBlockItem(blockItem);
                });
            }

            await UniTask.WaitForSeconds(1.2f);
            BlockGameRoot.Instance.StopAnimation();

            //消除逻辑
            OnAfterUseBagPopEliminateBlock();
        }

        private async void PlayAnimation(ItemUI_FlowerBlock itemUIBlock, GameObject animationSpinePrefab, Action complete)
        {
            var animationInstance = GameObject.Instantiate(animationSpinePrefab, itemUIBlock.transform);
            animationInstance.transform.localScale = Vector3.one;
            animationInstance.transform.localPosition = Vector3.zero;

            var skeleton = animationInstance.GetComponentInChildren<SkeletonGraphic>();
            skeleton.AnimationState.SetAnimation(0, "idle", false);
            await UniTask.WaitForSeconds(skeleton.AnimationState.GetCurrent(0).Animation.Duration);
            GameObject.Destroy(animationInstance);
            complete?.Invoke();
        }

        private int GetGridTypeConfig(int colCount)
        {
            var configList = ConfigMgr.Instance.Tables.BlockGridTypeCategory.DataList;

            foreach (var item in configList)
            {
                if (item.GridData.Count == colCount)
                {
                    return item.Id;
                }
            }

            Debugger.LogError($"未配置的格子列数 colCount:{colCount}");
            return configList[0].Id;
        }

        private void SpliteBlockItem(ItemUI_FlowerBlock blockItem)
        {
            var list = _block2GridsXY[blockItem];

            if (list.Count == 3)
            {
                // 1 + 2
                var gridType1Id = GetGridTypeConfig(1);
                var refresh1 = new RefreshBlockItemData()
                {
                    startGridX = list[0].x,
                    blockTypeId = gridType1Id,
                    mainFlowerId = blockItem.mainFlowerId,
                    flowersId = new List<int>() { blockItem.flowersId[0] },
                };
                CreateBlockItem(refresh1, list[0].y).Forget();

                var gridType2Id = GetGridTypeConfig(2);
                var refresh2 = new RefreshBlockItemData()
                {
                    startGridX = list[1].x,
                    blockTypeId = gridType2Id,
                    mainFlowerId = blockItem.mainFlowerId,
                    flowersId = new List<int>() { blockItem.flowersId[1], blockItem.flowersId[2] },
                };  
                CreateBlockItem(refresh2, list[1].y).Forget();
            }
            else if (list.Count == 4)
            {
                // 2 + 2
                var gridType2Id = GetGridTypeConfig(2);
                var refresh1 = new RefreshBlockItemData()
                {
                    startGridX = list[0].x,
                    blockTypeId = gridType2Id,
                    mainFlowerId = blockItem.mainFlowerId,
                    flowersId = new List<int>() { blockItem.flowersId[0], blockItem.flowersId[1] },
                };
                CreateBlockItem(refresh1, list[0].y).Forget();

                var refresh2 = new RefreshBlockItemData()
                {
                    startGridX = list[2].x,
                    blockTypeId = gridType2Id,
                    mainFlowerId = blockItem.mainFlowerId,
                    flowersId = new List<int>() { blockItem.flowersId[2], blockItem.flowersId[3] },
                };
                CreateBlockItem(refresh2, list[0].y).Forget();
            }
            else
            {
                Debugger.LogError($"未处理的格子列数:{list.Count}");
            }

            //地图上移除这个方块
            RemoveBlockItem(blockItem, true);
        }

        public void OnUseRefreshBagProp()
        {
            var collectFlowerRule = UIWindowMgr.Instance.GetWindow<FlowerBlockWindow>().mCollectFlowerRule;

            ItemUI_CollectButterfly collectFlower = default;

            //顺序找到未收集完成的鲜花
            foreach (var flower in collectFlowerRule.flowers)
            {
                if (flower.LogicComplete)
                {
                    continue;
                }

                collectFlower = flower;
                break;
            }

            if (collectFlower != null)
            {
                //给格子全部的鲜花变成需要收集的鲜花
                foreach (var blockItem in _block2GridsXY)
                {
                    if (blockItem.Key.IsCanChangeToFlowerId(collectFlower.flowerId))
                    {
                        //播放刷新的动画
                        PlayAnimation(blockItem.Key, BlockGameRoot.Instance.pool.refreshPrefab, () =>
                        {
                            blockItem.Key.RandomButterflyId(collectFlower.flowerId);
                        });
                    }
                }
            }
        }

        public async void OnUseRemoveBagProp()
        {
            //随机移除场上50%的方块
            var removeCount = inMapBlockItems.Count / 2;

            for (int i = 0; i < removeCount; i++)
            {
                var randomIndex = UtilityTool.RandomNumber(0, inMapBlockItems.Count);
                var removeBlockItem = inMapBlockItems[randomIndex];
                RemoveBlockItem(removeBlockItem, false);

                //播放移除的动画
                PlayAnimation(removeBlockItem, BlockGameRoot.Instance.pool.removePrefab, () =>
                {
                    BlockGameRoot.Instance.pool.PushGridTypeToPool(removeBlockItem);
                });
            }

            await UniTask.WaitForSeconds(1f);

            //消除逻辑
            OnAfterUseBagPopEliminateBlock();
        }

        public void OnUseReliveBagProp()
        {
            //移除顶部3行
            var removeLst = GetTopRowBlockItems(3);
            for (int i = removeLst.Count - 1; i >= 0; i--)
            {
                var removeBlockItem = removeLst[i];
                RemoveBlockItem(removeBlockItem, false);

                //播放移除的动画
                PlayAnimation(removeBlockItem, BlockGameRoot.Instance.pool.removePrefab, () =>
                {
                    BlockGameRoot.Instance.pool.PushGridTypeToPool(removeBlockItem);
                });
            }
        }
    }
}