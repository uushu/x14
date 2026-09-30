using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace GameMain.Garden
{
    public partial class FlowerGridsMap
    {
        //高亮显示格子
        private Dictionary<Vector2Int, FlowerMask> _flowerMasks = new();
        //高亮显示格子 前后对比 _flowerMasks 
        private Dictionary<Vector2Int, FlowerMask> _flowerMasks2 = new();

        public Action<Vector3> GeneratorButterfly { get; set; }

        //强制拖拽到地图上的位置
        public List<Vector2Int> ConstraintDragToGrid { get; private set; } = new();

        public void SetConstraintDragToGrids(List<Vector2Int> grids)
        {
            ConstraintDragToGrid.AddRange(grids);
        }

        public void OnDrag_FlowerShape(FlowerShape flower)
        {
            if (flowerRoot.IsForbidenInput) return;

            //高亮
            var runtimeGrids = ListPool<Vector2Int>.Get();
            var valid = flower.GetRuntimeGridXY(ref runtimeGrids);

            //1.显示可以放置的提示
            ShowCanPutInMapMask(flower, runtimeGrids);

            //2.显示高亮可以消除的提示
            var removeResult = JudgeRemove();
            ShowRemoveRowColMask(removeResult.rowList, removeResult.colList, runtimeGrids);

            ListPool<Vector2Int>.Release(runtimeGrids);

            var isSame = UtilityTool.DictIsSame(_flowerMasks, _flowerMasks2);

            if (isSame == false && removeResult.HasRowColRemove)
            {
                AudioMgr.Instance.PlayAudioOneShot(AudioName.CanRemoveAudio);
            }

            //3.对比移除
            foreach (var item in _flowerMasks2)
            {
                _flowerMasks.Remove(item.Key);
            }
            foreach (var mask in _flowerMasks)
            {
                if (_inMapFlowers.TryGetValue(mask.Key, out var flower2))
                {
                    flower2?.Flower_StopDragLoop();
                }
                //剩下的全部影藏
                flowerRoot.flowerPool.PushToFlowerMaskPool(mask.Value);
            }
            ExchangeFlowerMask();
        }

        public bool OnPointerUp_TryEliminateFlowers(FlowerShape flower)
        {
            if (flowerRoot.IsForbidenInput)
            {
                Debugger.Log("处于禁止输入状态", Debugger.ELogColor.Red);
                return false;
            }

            //2.清理高亮的提示
            ClearAllFlowerMask();

            //1.清理临时放置的格子数据
            DragFlowerTempPlaceToMap(null, flower);

            if (FlowerCanPlaceShapeToMap(flower) == false)
            {
                return false;
            }

            var runtimeGrids = ListPool<Vector2Int>.Get();
            var valid = flower.GetRuntimeGridXY(ref runtimeGrids);

            if (ConstraintDragToGrid.Count > 0)
            {
                if (UtilityTool.ListIsSame(ConstraintDragToGrid, runtimeGrids) == false)
                {
                    return false;
                }

                Debugger.Log("强制拖拽到目标格子成功");
                ConstraintDragToGrid.Clear();
            }

            _removeResult.Clear();

            var activeFlowers = flower.ActiveFlowers;
            for (int i = 0; i < runtimeGrids.Count; i++)
            {
                PutFlowerToMap(runtimeGrids[i], activeFlowers[i], true);
                _removeResult.putInMapFlowers.Add(activeFlowers[i]);
            }
            flower.ActiveFlowers.Clear();

            flower.gameObject.SetActive(false);
            ListPool<Vector2Int>.Release(runtimeGrids);

            //消除的结果
            var result = JudgeRemove();

            if (result.HasRowColRemove == false)
            {
                //放下音效
                AudioMgr.Instance.PlayAudioOneShot(AudioName.FlowerPutInMapAudio);
            }

            //开始消除
            var screenPoint = RectTransformUtility.WorldToScreenPoint(UIWindowMgr.Instance.uiCamera, flower.transform.position);
            _removeResult.putInMapScreenPoint = screenPoint;
            TryBloomFlowers(_removeResult).Forget();

            return true;
        }

        private void DoFlowerAnimation(Garden_FlowerSingle gridFlower)
        {
            switch (gridFlower.FlowerType)
            {
                case Garden_FlowerSingle.EFlowerType.NormalFlower:

                    DoFlower_BloomAnimation(gridFlower);
                    break;
                case Garden_FlowerSingle.EFlowerType.BubbleButterFly:

                    var collectRule = UIWindowMgr.Instance.GetWindow<GardenWindow>().mCollectFlowerRule;
                    var flyTarget = collectRule.GetButterflyBy(gridFlower.FlowerConfigId);
                    DoButterfly_FlyAnimation(gridFlower, flyTarget != null ? flyTarget.transform : null, true, (butterfly) =>
                    {
                        collectRule.SubLastCollectCount(butterfly.FlowerConfigId);
                    });
                    break;
                default:
                    Debugger.LogError($"未处理的鲜花类型:{gridFlower.FlowerType}");
                    break;
            }
        }

        private void DoFlower_BloomAnimation(Garden_FlowerSingle flower)
        {
            var randomScale = UtilityTool.RandomNumber(0.8f, 1f);
            flower.transform.localScale = Vector3.one * randomScale;

            var bloomEffect = flowerRoot.flowerPool.flowerBloomPool.GetFromPoolSync().Item1;
            bloomEffect.transform.SetParent(flower.transform);
            bloomEffect.transform.localScale = Vector3.one;
            bloomEffect.transform.localPosition = Vector3.zero;

            flower.Flower_PlayBloom(() =>
            {
                flowerRoot.flowerPool.PushToFlowerPool(flower);
                flowerRoot.flowerPool.PushBloomEffectToPool(bloomEffect);
            });
        }

        public void DoButterfly_FlyAnimation(Garden_FlowerSingle butterfly, Transform flyTarget, bool showHitEffect, Action<Garden_FlowerSingle> onFlyTarget)
        {
            if (flyTarget != null)
            {
                butterfly.transform.SetParent(GardenGameRoot.Instance.flowerPool.butterflyPoolRoot);

                //播放破冰的动画
                butterfly.Butterfly_BubbleBreak(() =>
                {
                    butterfly.Butterfly_Fly();

                    //蝴蝶的飞行特效
                    var flyEffect = GardenGameRoot.Instance.flowerPool.butterFlyEffectPool.GetFromPoolSync().Item1;
                    flyEffect.transform.SetParent(butterfly.transform);
                    flyEffect.transform.SetAsFirstSibling();
                    flyEffect.transform.localPosition = Vector3.zero;
                    flyEffect.transform.localScale = Vector3.one;

                    //飞到目标点
                    var distance = Vector3.Distance(flyTarget.transform.position, butterfly.transform.position);
                    var moveTime = distance / 5f;

                    var sequence = DOTween.Sequence();
                    var t1 = butterfly.transform.DOScale(Vector3.one * 2f, 0.3f);
                    var t2 = butterfly.transform.DOScale(Vector3.one, moveTime);

                    sequence.Append(t1);
                    sequence.AppendInterval(0.7f);
                    sequence.AppendCallback(() =>
                    {
                        StartCoroutine(UtilityTool.DoBezierMove(butterfly.transform, flyTarget.transform.position, moveTime, 0.5f, -0.5f, () =>
                        {
                            flowerRoot.flowerPool.PushToFlowerPool(butterfly);
                            GardenGameRoot.Instance.flowerPool.butterFlyEffectPool.PushToPool(flyEffect);

                            if (showHitEffect)
                            {
                                StartCoroutine(ShowButterflyHitEffect(butterfly.transform.position));
                            }

                            onFlyTarget?.Invoke(butterfly);
                        }));
                    });
                    sequence.Join(t2);
                });
            }
            else
            {
                flowerRoot.flowerPool.PushToFlowerPool(butterfly);
            }
        }

        private IEnumerator ShowButterflyHitEffect(Vector3 worldPosition) 
        {
           var rotateEffect = GardenGameRoot.Instance.flowerPool.rotateEffectPool.GetFromPoolSync().Item1;
            rotateEffect.transform.SetParent(GardenGameRoot.Instance.flowerPool.rotateEffectPoolRoot);

            rotateEffect.transform.localScale = Vector3.one * 0.3f;
            rotateEffect.transform.position = worldPosition;

            yield return new WaitForSeconds(1.5f);

            GardenGameRoot.Instance.flowerPool.rotateEffectPool.PushToPool(rotateEffect);
        }

        private async UniTask TryBloomFlowers(FlowerRemoveResult removeResult)
        {
            var rows = removeResult.rowList;
            var cols = removeResult.colList;
            removeResult.removeFlowers.Clear();

            _removeInMapFlowers.Clear();
            //消除的逻辑
            if (rows.Count > 0 || cols.Count > 0)
            {
                for (int y = 0; y < rows.Count; y++)
                {
                    var yIndex = rows[y];
                    for (int x = 0; x < customGrids.colCount; x++)
                    {
                        var gridXY = new Vector2Int(x, yIndex);
                        var flower = _inMapFlowers[gridXY];
                        SetGridDataNull(gridXY, false);
                        _removeInMapFlowers.Add(gridXY, flower);
                        TryAddToRemoveTemp(removeResult.removeFlowers, flower);
                    }
                }

                for (int x = 0; x < cols.Count; x++)
                {
                    var xIndex = cols[x];
                    for (int y = 0; y < customGrids.rowCount; y++)
                    {
                        var gridXY = new Vector2Int(xIndex, y);
                        var flower = _inMapFlowers[gridXY];
                        SetGridDataNull(gridXY, false);
                        if (_removeInMapFlowers.TryGetValue(gridXY, out var flowers) == false)
                        {
                            _removeInMapFlowers.Add(gridXY, flower);
                        }
                      
                        TryAddToRemoveTemp(removeResult.removeFlowers, flower);
                    }
                }
            }

            OnPlaceToMapBeforeCallBack?.Invoke(removeResult);

            //开花的动画
            if (rows.Count > 0 || cols.Count > 0)
            {
                flowerRoot.StartAnimation();

                await DoRandomBloom(rows, cols);

                flowerRoot.StopAnimation();
            }

            OnPlaceToMapAfterCallBack?.Invoke();
        }

        private void ShowCanPutInMapMask(FlowerShape flower, List<Vector2Int> runtimeGrids)
        {
            //清理上次动态放入地图的格子数据
            DragFlowerTempPlaceToMap(null, flower);

            if (FlowerCanPlaceShapeToMap(flower))
            {
                //放入新的临时的格子数据
                DragFlowerTempPlaceToMap(runtimeGrids, flower);
            }
        }

        private void ShowRemoveRowColMask(List<int> rows, List<int> cols, List<Vector2Int> runtimeGrids)
        {
            for (int y = 0; y < rows.Count; y++)
            {
                var yIndex = rows[y];
                for (int x = 0; x < customGrids.colCount; x++)
                {
                    var gridXY = new Vector2Int(x, yIndex);
                    if (runtimeGrids.Contains(gridXY))
                    {
                        continue;
                    }
                    TryAddToFlowerMask2(gridXY, false);
                }
            }

            for (int x = 0; x < cols.Count; x++)
            {
                var xIndex = cols[x];
                for (int y = 0; y < customGrids.rowCount; y++)
                {
                    var gridXY = new Vector2Int(xIndex, y);
                    if (runtimeGrids.Contains(gridXY))
                    {
                        continue;
                    }
                    TryAddToFlowerMask2(gridXY, false);
                }
            }
        }

        private async void DoColRandom(int rowIndex, List<Garden_FlowerSingle> hashSet, float beforeDelayTime, float delayTime)
        {
            await UniTask.WaitForSeconds(beforeDelayTime);

            var list = ListPool<int>.Get();
            var list2 = ListPool<int>.Get();

            FullListNumber(list, customGrids.colCount);
            var yIndex = rowIndex;

            //随机生成2个蝴蝶飞到进度条
            for (int i = 0; i < 2; i++)
            {
                var xIndex = UtilityTool.RandomNumber(0, customGrids.colCount);
                list2.Add(xIndex);
            }

            while (list.Count > 0)
            {
                var xIndex = UtilityTool.RandomNumber(list);
                list.Remove(xIndex);
                var gridXY = new Vector2Int(xIndex, yIndex);
                var flower = _removeInMapFlowers[gridXY];

                if (flower == null)
                {
                    Debugger.LogError($"消除格子错误 gridXY:{gridXY}");
                    continue;
                }

                if (list2.Contains(xIndex))
                {
                    GeneratorButterfly?.Invoke(flower.transform.position);
                }

                if (hashSet.Contains(flower) == false)
                {
                    hashSet.Add(flower);
                    DoFlowerAnimation(flower);

                    await UniTask.WaitForSeconds(delayTime);
                }
            }

            ListPool<int>.Release(list);
            ListPool<int>.Release(list2);
        }

        private async void DoRowRandom(int colIndex, List<Garden_FlowerSingle> hashSet, float beforeDelayTime, float delayTime)
        {
            await UniTask.WaitForSeconds(beforeDelayTime);

            var list = ListPool<int>.Get();
            var list2 = ListPool<int>.Get();

            FullListNumber(list, customGrids.rowCount);
            var xIndex = colIndex;

            //随机生成2个蝴蝶飞到进度条
            for (int i = 0; i < 2; i++)
            {
                var yIndex = UtilityTool.RandomNumber(0, customGrids.rowCount);
                list2.Add(yIndex);
            }

            while (list.Count > 0)
            {
                var yIndex = UtilityTool.RandomNumber(list);
                list.Remove(yIndex);
                var gridXY = new Vector2Int(xIndex, yIndex);
                var flower = _removeInMapFlowers[gridXY];

                if (flower == null)
                {
                    Debugger.LogError($"消除格子错误 gridXY:{gridXY}");
                    continue;
                }

                if (list2.Contains(yIndex))
                {
                    GeneratorButterfly?.Invoke(flower.transform.position);
                }

                if (hashSet.Contains(flower) == false)
                {
                    hashSet.Add(flower);
                    DoFlowerAnimation(flower);
                    await UniTask.WaitForSeconds(delayTime);
                }
            }

            ListPool<int>.Release(list);
            ListPool<int>.Release(list2);
        }

        private async UniTask DoRandomBloom(List<int> rows, List<int> cols)
        {
            float beforeDelayTime = 0.2f;
            float singleDelayTime = 0.1f;
            int maxCount = customGrids.rowCount;

            var hashSet = ListPool<Garden_FlowerSingle>.Get();
            hashSet.Clear();

            for (int x = 0; x < cols.Count; x++)
            {
                DoRowRandom(cols[x], hashSet, beforeDelayTime * x, singleDelayTime);
            }

            for (int y = 0; y < rows.Count; y++)
            {
                DoColRandom(rows[y], hashSet, beforeDelayTime * y, singleDelayTime);
            }

            ListPool<Garden_FlowerSingle>.Release(hashSet);

            var animationTime = singleDelayTime * maxCount + Mathf.Max(rows.Count, cols.Count) * beforeDelayTime;
            //动画播放完毕 在进行消除的逻辑判断
            await UniTask.WaitForSeconds(animationTime);
        }

        private void TryDropOutGrid(Vector2Int gridXY, float dropDownTime)
        {
            if (HasFlower(gridXY) == false) return;

            var gridX = gridXY.x;
            Vector2Int downTo = gridXY;
            for (int y = gridXY.y + 1; y < customGrids.rowCount; y++)
            {
                var downGridXY = new Vector2Int(gridX, y);
                if (HasFlower(downGridXY))
                {
                    break;
                }

                downTo = downGridXY;
            }

            if (downTo != gridXY)
            {
                var flower = _inMapFlowers[gridXY];
                _inMapFlowers[gridXY] = null;
                _inMapFlowers[downTo] = flower;

                var targetMoveTo = customGrids.GridXYToLocalPosition(downTo);
                flower.transform.DOLocalMove(targetMoveTo, dropDownTime).SetEase(Ease.OutBounce);
            }
        }

        //复活道具的掉落
        public async void AllColsDropOut(Action complete)
        {
            float dropDownTime = 1f;

            do
            {
                for (int y = customGrids.rowCount - 1; y >= 0; y--)
                {
                    for (int x = 0; x < customGrids.colCount; x++)
                    {
                        var gridXY = new Vector2Int(x, y);

                        TryDropOutGrid(gridXY, dropDownTime);
                    }
                }

                await UniTask.WaitForSeconds(dropDownTime + 0.2f);

                //开始消除
                //消除的结果
                _removeResult.Clear();
                var result = JudgeRemove();
                result.isReliveRemove = true;

                if (_removeResult.HasRowColRemove)
                {
                    //开始消除
                    var screenPoint = RectTransformUtility.WorldToScreenPoint(UIWindowMgr.Instance.uiCamera, transform.position);
                    _removeResult.putInMapScreenPoint = screenPoint;

                    await TryBloomFlowers(result);

                    await UniTask.WaitForSeconds(1f);
                }
            } while (_removeResult.HasRowColRemove);

            complete?.Invoke();
        }
    }
}