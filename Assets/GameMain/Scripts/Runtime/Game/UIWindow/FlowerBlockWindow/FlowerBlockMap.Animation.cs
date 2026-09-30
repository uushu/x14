using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameMain.Garden;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace GameMain.FlowerBlock
{
    public partial class FlowerBlockMap
    {
        //记录滑块格子下落的层数
        private Dictionary<ItemUI_FlowerBlock, int> _recordBlockItemDownSteps = new();

        private async UniTask AllMoveUpAnimation(int moveUpGridCount)
        {
            var moveUpHeight = moveUpGridCount * (uicustomGrids.gridHeight + uicustomGrids.rowSpace);
            float currentMoveY = 0f;
            await DOTween.To(() => currentMoveY, (value) =>
            {
                var deltaValue = value - currentMoveY;
                currentMoveY = value;

                for (int i = 0; i < inMapBlockItems.Count; i++)
                {
                    inMapBlockItems[i].transform.localPosition += new Vector3(0, deltaValue, 0);
                }

            }, moveUpHeight, 0.5f);
        }

        private void AutoMoveDownAnimation(ItemUI_FlowerBlock blockItem, int moveDownGridCount, float moveTime, Action onmoveDownComplete)
        {
            var moveDownHeight = moveDownGridCount * (uicustomGrids.gridHeight + uicustomGrids.rowSpace);

            float currentMoveY = 0f;
            DOTween.To(() => currentMoveY, (value) =>
            {
                var deltaValue = value - currentMoveY;
                currentMoveY = value;

                blockItem.transform.localPosition -= new Vector3(0, deltaValue, 0);

            }, moveDownHeight, moveTime).OnComplete(() =>
            {
                onmoveDownComplete?.Invoke();
            }).SetEase(Ease.OutBounce);

            blockItem.Flower_PlayMove(EFlowerBlockMoveDirection.Up);
        }

        private async UniTask<bool> RecusionEliminateBlocks()
        {
            //1.根据格子的位置刷新格子的数据
            UpdateBlockMapData();

            //1.记录格子掉落的层数
            _recordBlockItemDownSteps.Clear();
            RecordGridDownStep(ref _recordBlockItemDownSteps);

            //下落的动画
            if (_recordBlockItemDownSteps.Count > 0)
            {
                float moveTime = 0.28f;

                //3.自动掉的动画
                int maxDownCount = 0;

                AudioMgr.Instance.PlayAudioOneShot(AudioName.Block_FlowerDropDown);

                foreach (var blockItem in _recordBlockItemDownSteps)
                {
                    AutoMoveDownAnimation(blockItem.Key, blockItem.Value, moveTime, null);

                    if (blockItem.Value > maxDownCount)
                    {
                        maxDownCount = blockItem.Value;
                    }
                }

                //等滑块掉落
                await UniTask.WaitForSeconds(moveTime + 0.02f);
            }

            //4.开始消除
            var removeRowsIndex = GetRemoveBlockRows();
            if (removeRowsIndex.Count > 0)
            {
                _removeResult.Clear();
                _removeResult.removeRowsIndex.AddRange(removeRowsIndex);

                for (int i = 0; i < removeRowsIndex.Count; i++)
                {
                    var yIndex = removeRowsIndex[i];
                    for (int x = 0; x < uicustomGrids.colCount; x++)
                    {
                        var gridXY = new Vector2Int(x, yIndex);

                        var blockItem = _gridXY2Block[gridXY];
                        _gridXY2Block.Remove(gridXY);

                        if (_block2GridsXY.ContainsKey(blockItem))
                        {
                            //如果当前的滑块是特殊(自己的上下左右的滑块也要消除)
                            if (blockItem.IsSpecicalBlockItem)
                            {
                                var result = ListPool<ItemUI_FlowerBlock>.Get();
                                
                                //获取周围的滑块
                                GetAroundBlockItems(blockItem, ref result);

                                foreach (var blockItem2 in result)
                                {
                                    _removeResult.TryAddRemoveBlockItem(blockItem2);
                                }

                                ListPool<ItemUI_FlowerBlock>.Release(result);
                            }

                            _removeResult.TryAddRemoveBlockItem(blockItem);

                            if (_recordBlockItemDownSteps.ContainsKey(blockItem))
                            {
                                //判断是否是下落的滑块完成的消除
                                _removeResult.TryAddDownBlockItem(yIndex, blockItem);
                            }

                            var gridXYs = _block2GridsXY[blockItem];
                            _block2GridsXY.Remove(blockItem);
                            ListPool<Vector2Int>.Release(gridXYs);
                        }
                    }
                }

                OnBeforeRemoveFlowerBlockCallBack?.Invoke(_removeResult);

                //消除动画
                for (int i = 0; i < _removeResult.removeBlockItems.Count; i++)
                {
                    //await UniTask.WaitForSeconds(0.1f);
                    var blockItem = _removeResult.removeBlockItems[i];

                    //鲜花动画
                    DoBloomAnimation(blockItem);
                    RemoveBlockItem(blockItem, false);
                }

                OnAfterRemoveFlowerBlockCallBack?.Invoke(_removeResult);

                //等待动画
                await UniTask.WaitForSeconds(1.22f);

                //消除成功，在判断是否滑块可以掉落
                await RecusionEliminateBlocks();
                return true;
            }
            return false;
        }

        //开始判断消除格子
        private async void TryEliminateBlock(bool newLastRow, Action complete)
        {
            var isRemove = await RecusionEliminateBlocks();

            if (TopIsFull())
            {
                OnFlowerBlockTopFullCallBack?.Invoke();
                complete?.Invoke();
                return;
            }

            if (newLastRow)
            {
                //顶一排
                AudioMgr.Instance.PlayAudioOneShot(AudioName.Block_FlowerGenerator);
                await AllMoveUpAnimation(1);
                await GeneratorByRule(uicustomGrids.rowCount - 1);
            }

            do
            {
                isRemove = await RecusionEliminateBlocks();

                var maxYIndex = GetMaxBlockYIndex();
                if (maxYIndex <= 0)
                {
                    //不足2排,固定上顶2排
                    await AllMoveUpAnimation(2);

                    //生成2排
                    await GeneratorByRule(uicustomGrids.rowCount - 2);
                    await GeneratorByRule(uicustomGrids.rowCount - 1);

                    isRemove = await RecusionEliminateBlocks();
                }

            } while (isRemove);

            complete?.Invoke();
        }

        public async void DoBloomAnimation(ItemUI_FlowerBlock blockItem)
        {
            blockItem.transform.SetAsLastSibling();

            for (int i = 0; i < blockItem.flowers.Count; i++)
            {
                var flowerUI = blockItem.flowers[i];

                switch (flowerUI.FlowerType)
                {
                    case Block_FlowerSingle.EFlowerType.SpecailFlower:
                    case Block_FlowerSingle.EFlowerType.NormalFlower:

                        flowerUI.Flower_PlayBloom(null);

                        break;
                    case Block_FlowerSingle.EFlowerType.BubbleButterFly:

                        var collectFlowerRule = UIWindowMgr.Instance.GetWindow<FlowerBlockWindow>().mCollectFlowerRule;
                        var collectButterfly = collectFlowerRule.GetFlowerBy(flowerUI.FlowerConfigId);

                        if (collectButterfly != null)
                        {
                            flowerUI.canvas.sortingOrder += 10;
                            DoButterfly_FlyAnimation(flowerUI, collectButterfly.transform, true, (blockFlower) =>
                            {
                                flowerUI.canvas.sortingOrder -= 10;
                                collectFlowerRule.SubFlowerCollect(flowerUI.FlowerConfigId, 1);
                            });
                        }
                        else
                        {
                            flowerUI.gameObject.SetActive(false);
                        }

                        //顶替一朵花
                        var insteadFlower = await blockItem.GetFlowerFromPool(blockItem.mainFlowerId);
                        insteadFlower.transform.SetParent(flowerUI.transform.parent);
                        insteadFlower.transform.localPosition = flowerUI.transform.localPosition;
                        blockItem.flowers.Add(insteadFlower);

                        //播放开花动画
                        insteadFlower.Flower_PlayBloom(null);

                        break;
                    default:
                        Debugger.LogError($"未处理的鲜花类型:{flowerUI.FlowerType}");
                        break;
                }
            }

            StartCoroutine(DelayDisableEnable(blockItem.imgBlockBg, 0.3f));
            StartCoroutine(DelayPushToPool(blockItem, 2.5f));
        }

        private IEnumerator DelayDisableEnable(Image image, float delayTime)
        { 
            yield return new WaitForSeconds(delayTime);
            image.enabled = false;
        }

        private IEnumerator DelayPushToPool(ItemUI_FlowerBlock blockItem, float delayTime)
        {
            //开始的时长
            yield return new WaitForSeconds(delayTime);

            BlockGameRoot.Instance.pool.PushGridTypeToPool(blockItem);
        }

        public void DoButterfly_FlyAnimation(Block_FlowerSingle butterfly, Transform flyTarget, bool showHitEffect, Action<Block_FlowerSingle> onFlyTarget)
        {
            if (flyTarget != null)
            {
               // butterfly.transform.SetParent(BlockGameRoot.Instance.pool.flowerPoolRoot);

                //播放破冰的动画
                butterfly.Butterfly_BubbleBreak(() =>
                {
                    butterfly.Butterfly_Fly();

                    //蝴蝶的飞行特效
                    var flyEffect = BlockGameRoot.Instance.pool.butterFlyEffectPool.GetFromPoolSync().Item1;
                    flyEffect.transform.SetParent(butterfly.transform);
                    flyEffect.transform.SetAsFirstSibling();
                    flyEffect.transform.localPosition = butterfly.flowerSpine.transform.localPosition;
                    flyEffect.transform.localScale = Vector3.one;

                    //飞到目标点
                    var distance = Vector3.Distance(flyTarget.transform.position, butterfly.transform.position);
                    var randomSpeed = UtilityTool.RandomNumber(5f, 8f);
                    var moveTime = distance / randomSpeed;

                    var sequence = DOTween.Sequence();
                    var t2 = butterfly.transform.DOScale(Vector3.one, moveTime);

                    var t3 = butterfly.transform.DOMove(flyTarget.transform.position, moveTime).SetEase(Ease.Linear);
                    sequence.Append(t3);
                    sequence.Join(t2);
                    sequence.OnComplete(() =>
                    {
                        butterfly.gameObject.SetActive(false);
                        BlockGameRoot.Instance.pool.butterFlyEffectPool.PushToPool(flyEffect);

                        if (showHitEffect)
                        {
                            StartCoroutine(ShowButterflyHitEffect(butterfly.transform.position));
                        }

                        onFlyTarget?.Invoke(butterfly);
                    }).SetDelay(0.5f);
                });
            }
            else
            {
                butterfly.gameObject.SetActive(false);
            }
        }

        private IEnumerator ShowButterflyHitEffect(Vector3 worldPosition)
        {
            var rotateEffect = BlockGameRoot.Instance.pool.rotateEffectPool.GetFromPoolSync().Item1;
            rotateEffect.transform.SetParent(BlockGameRoot.Instance.pool.rotateEffectPoolRoot);

            rotateEffect.transform.localScale = Vector3.one * 0.3f;
            rotateEffect.transform.position = worldPosition;

            yield return new WaitForSeconds(1.5f);

            BlockGameRoot.Instance.pool.rotateEffectPool.PushToPool(rotateEffect);
        }
    }
}