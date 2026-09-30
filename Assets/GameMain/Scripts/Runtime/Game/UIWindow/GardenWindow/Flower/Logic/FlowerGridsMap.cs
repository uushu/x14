using cfg;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace GameMain.Garden
{
    public partial class FlowerGridsMap : ILogicBase
    {
        [LabelText("游戏入口")]
        public GardenGameRoot flowerRoot;

        [LabelText("自定义格子")]
        public UICustomGrids customGrids;

        [LabelText("鲜花格子根节点")]
        public Transform flowerGridRoot;
        public Action<FlowerRemoveResult> OnPlaceToMapBeforeCallBack { get; set; }
        public Action OnPlaceToMapAfterCallBack { get; set; }

        private Dictionary<Vector2Int, Garden_FlowerSingle> _inMapFlowers;
        private Dictionary<Vector2Int, Garden_FlowerSingle> _removeInMapFlowers;

        //临时放置的XY
        private List<Vector2Int> _lastTempPlaceXY = new();

        //消除结果显示
        private FlowerRemoveResult _removeResult;

        public override void Initialize()
        {
            _inMapFlowers = new();
            _removeInMapFlowers = new();
            _removeResult = new();

            for (int y = 0; y < customGrids.rowCount; y++)
            {
                for (int x = 0; x < customGrids.colCount; x++)
                {
                    _inMapFlowers.Add(new Vector2Int(x, y), null);
                }
            }
        }

        public override void OnStart()
        {
            base.OnStart();

        }

        public void ClearMapData()
        {
            flowerRoot.flowerPool.DisableAllEntities();

            _inMapFlowers.Clear();
            _flowerMasks.Clear();
            _flowerMasks2.Clear();
            for (int y = 0; y < customGrids.rowCount; y++)
            {
                for (int x = 0; x < customGrids.colCount; x++)
                {
                    _inMapFlowers.Add(new Vector2Int(x, y), null);
                }
            }
            ConstraintDragToGrid.Clear();
        }

        public void ClearRows(List<int> rowIndex)
        {
            for (int y = 0; y < rowIndex.Count; y++)
            {
                var yIndex = rowIndex[y];
                for (int x = 0; x < customGrids.colCount; x++)
                {
                    var gridXY = new Vector2Int(x, yIndex);
                    SetGridDataNull(gridXY, true);
                }
            }
        }

        public bool MapIsEmpty()
        {
            foreach (var item in _inMapFlowers)
            {
                if (item.Value != null)
                {
                    return false;
                }
            }

            return true;
        }

        public List<Vector2Int> GetRandomClearRowCols(int clearRowCount, int clearColCount)
        {
            var tempList = new List<Vector2Int>();

            foreach (var mapFlower in _inMapFlowers)
            {
                if (mapFlower.Value != null)
                {
                    tempList.Add(mapFlower.Key);
                }
            }

            if (tempList.Count <= 0)
            {
                Debugger.LogError("随机清理格子行列错误 地图上没有鲜花");
                tempList.Clear();
                return tempList;
            }

            //随机格子
            var randomGridXY = UtilityTool.RandomNumber(tempList);

            var maxStartX = Mathf.Min(customGrids.colCount - clearColCount, randomGridXY.x);
            var maxStartY = Mathf.Min(customGrids.rowCount - clearRowCount, randomGridXY.y);
            var startXY = new Vector2Int(maxStartX, maxStartY);

            tempList.Clear();
            Debugger.Log($"随机清理格子数据起点 randomGridXY:{randomGridXY} startXY:{startXY} clearRowCount={clearRowCount} clearColCount={clearColCount}");
            for (int y = 0; y < clearRowCount; y++)
            {
                for (int x = 0; x < clearColCount; x++)
                {
                    var gridXY = new Vector2Int(x, y) + startXY;
                    var flower = GetFlowerBy(gridXY);

                    tempList.Add(gridXY);
                }
            }

            return tempList;
        }

        //当前关卡的默认的格子在地图上
        public async void GeneratorInitGridsInMap()
        {
            var initGridType = flowerRoot.GardenRefreshRule.InitMapGridType;

            var mainFlowerId = flowerRoot.GardenRefreshRule.RefreshFlowerId();
            foreach (var list in initGridType)
            {
                //list:格子类型 格子起点(x,y)
                var gridType = list[0];
                var gridStartXY = new Vector2Int(list[1], list[2]);

                var gridTypeConfig = ConfigMgr.Instance.Tables.GardenFlowerGridTypeCategory.GetOrDefault(gridType);

                for (int i = 0; i < gridTypeConfig.GridData.Count; i++)
                {
                    var flowerId = mainFlowerId;

                    if (gridTypeConfig.GridData[i] == 0) continue;
                    var inMapGridXY = new Vector2Int(i % gridTypeConfig.Col, i / gridTypeConfig.Col) + gridStartXY;

                    var isButterfly = flowerRoot.GardenRefreshRule.RefreshBuffterFly(out int butterflyId);
                    if (isButterfly)
                    {
                        flowerId = butterflyId;
                    }

                    var flowerTypeConfig = ConfigMgr.Instance.Tables.GardenFlowerTypeCategory.GetOrDefault(flowerId);
                    var flowerComponent = await flowerRoot.flowerPool.multiPools.GetEntityAsync(flowerTypeConfig.GardenSpinePrefab);
                    flowerComponent.Initialize_Flower(flowerTypeConfig, false);
                    PutFlowerToMap(inMapGridXY, flowerComponent, false);
                }
            }
        }

        public override void UnInitialize()
        {
            base.UnInitialize();
        }

        private bool HasFlower(Vector2Int gridXY)
        {
            return GetFlowerBy(gridXY) != null;
        }

        public Garden_FlowerSingle GetFlowerBy(Vector2Int gridIndex)
        {
            if (customGrids.GridIndexIsValid(gridIndex) == false)
            {
                Debugger.LogError($"不合法的下标 gridIndex:{gridIndex}");
                return null;
            }

            return _inMapFlowers[gridIndex];
        }

        //随机待机的花
        private void PutFlowerToMap(Vector2Int gridXY, Garden_FlowerSingle flower, bool doAnimation)
        {
            if (flower == null)
            {
                Debugger.LogError($"不合法的下标 flag=null");
                return;
            }

            if (customGrids.GridIndexIsValid(gridXY) == false)
            {
                Debugger.LogError($"不合法的下标 gridIndex:{gridXY}");
                return;
            }

            if (_inMapFlowers[gridXY] != null)
            {
                Debugger.LogError($"当前格子已经有花朵 gridXY:{gridXY}");
                return;
            }

            _inMapFlowers[gridXY] = flower;

            var localPosition = customGrids.GridXYToLocalPosition(gridXY);
            flower.transform.SetParent(flowerGridRoot);
            flower.transform.localPosition = localPosition;
            flower.transform.localScale = Vector3.one;

            flower.Flower_PutInMapRadndomLevel(doAnimation);
            flower.name = $"{flower.FlowerTypeConfig.GardenSpinePrefab}-{gridXY.x}-{gridXY.y}";
        }

        public void SetGridDataNull(Vector2Int gridXY, bool hideFlowerUI)
        {
            var flower = _inMapFlowers[gridXY];
            if (flower == null) return;

            _inMapFlowers[gridXY] = null;

            if (hideFlowerUI)
            {
                flowerRoot.flowerPool.PushToFlowerPool(flower);
            }
        }
    
        //1.判断形状格子是否超出地图范围
        //2.判断格子当前是否在地图中是空
        private bool FlowerCanPlaceShapeToMap(FlowerShape flower)
        {
            var list = ListPool<Vector2Int>.Get();
            var isValid = flower.GetRuntimeGridXY(ref list);

            if (isValid)
            {
                foreach (var gridXY in list)
                {
                    if (HasFlower(gridXY))
                    {
                        ListPool<Vector2Int>.Release(list);
                        return false;
                    }
                }

                ListPool<Vector2Int>.Release(list);
                return true;
            }

            ListPool<Vector2Int>.Release(list);
            return false;
        }

        private void ClearAllFlowerMask()
        {
            foreach (var mask in _flowerMasks)
            {
                flowerRoot.flowerPool.PushToFlowerMaskPool(mask.Value);

                if (_inMapFlowers.TryGetValue(mask.Key, out var flower2))
                {
                    flower2?.Flower_StopDragLoop();
                }
            }
            _flowerMasks.Clear();
            _flowerMasks2.Clear();
        }

        public void ExchangeFlowerMask()
        {
            UtilityTool.DictEqual(_flowerMasks, _flowerMasks2);
            _flowerMasks2.Clear();
        }

        private void TryAddToFlowerMask2(Vector2Int gridXY, bool isFlowerMask)
        {
            var flower = _inMapFlowers[gridXY];
            if (flower == null) return;

            if (_flowerMasks2.TryGetValue(gridXY, out var entity) == false)
            {
                if (_flowerMasks.TryGetValue(gridXY, out entity) == false)
                {
                    //历史没有,创建新的
                    entity = flowerRoot.flowerPool.flowerMaskPool.GetFromPoolSync().Item1;
                }

                //存在就更新 
                _flowerMasks2.Add(gridXY, entity);
            }

            var childTrans = customGrids.GetChildByIndex(gridXY);

            if (isFlowerMask)
            {
                //显示阴影
                entity.SetFlowerSprite(childTrans, flower.LevelConfig.FlowerSprite);
                flower.Flower_StopDragLoop();
            }
            else
            {
                //可移除的特效
                entity.SetFlowerRemoveEffect(childTrans);
                //循环播放动画
                flower.Flower_PlayDragLoop();
            }
        }

        private void DragFlowerTempPlaceToMap(List<Vector2Int> tempPlace, FlowerShape flowerShape)
        {
            for (int i = 0; i < _lastTempPlaceXY.Count; i++)
            {
                var XY = _lastTempPlaceXY[i];
                _inMapFlowers[XY] = null;
            }
            _lastTempPlaceXY.Clear();

            if (tempPlace != null)
            {
                for (int i = 0; i < tempPlace.Count; i++)
                {
                    var gridXY = tempPlace[i];
                    _inMapFlowers[gridXY] = flowerShape.ActiveFlowers[i];

                    TryAddToFlowerMask2(gridXY, true);
                    _lastTempPlaceXY.Add(gridXY);
                }
            }
        }

        private void TryAddToRemoveTemp(Dictionary<int, List<Garden_FlowerSingle>> removeFlowerTempCache, Garden_FlowerSingle flower)
        {
            if (flower == null) return;

            if (removeFlowerTempCache.TryGetValue(flower.FlowerTypeConfig.Id, out var list) == false)
            {
                list = new List<Garden_FlowerSingle>();
                removeFlowerTempCache.Add(flower.FlowerTypeConfig.Id, list);
            }

            if (list.Contains(flower) == false)
            {
                list.Add(flower);
            }
        }

        private void FullListNumber(List<int> list, int maxIndex)
        {
            list.Clear();
            for (int i = 0; i < maxIndex; i++)
            {
                list.Add(i);
            }
        }

        private FlowerRemoveResult JudgeRemove()
        {
            _removeResult.rowList.Clear();
            _removeResult.colList.Clear();

            //判断横排 可以消除的格子
            for (int y = 0; y < customGrids.rowCount; y++)
            {
                var isSuccess = true;

                for (int x = 0; x < customGrids.colCount; x++)
                {
                    var gridXY = new Vector2Int(x, y);
                    if (_inMapFlowers.TryGetValue(gridXY, out var singleFlower) && singleFlower == null)
                    {
                        isSuccess = false;
                        //这一行不能消除
                        break;
                    }
                }

                if (isSuccess)
                {
                    _removeResult.rowList.Add(y);
                }
            }

            //判断竖排 可以消除的格子
            for (int x = 0; x < customGrids.colCount; x++)
            {
                var isSuccess = true;

                for (int y = 0; y < customGrids.rowCount; y++)
                {
                    var gridXY = new Vector2Int(x, y);
                    if (_inMapFlowers.TryGetValue(gridXY, out var singleFlower) && singleFlower == null)
                    {
                        isSuccess = false;
                        //这一行不能消除
                        break;
                    }
                }

                if (isSuccess)
                {
                    _removeResult.colList.Add(x);
                }
            }

            return _removeResult;
        }

        public bool FlowerCanPlaceInAny(FlowerShape flower)
        {
            //有可能被旋转了
            var isSuccess = FlowerCanPlaceInAny(flower.GridsXY);
            return isSuccess;
        }

        private void AddGridXY(List<Vector2Int> flowerGridXY, Vector2Int leftUpStart, ref List<Vector2Int> result)
        {
            result.Clear();

            for (var i = 0; i < flowerGridXY.Count; i++)
            {
               var gridXY = flowerGridXY[i] + leftUpStart;
                result.Add(gridXY);
            }
        }

        private bool FlowerCanPlaceInAny(List<Vector2Int> flowerGridXY)
        {
            var result = ListPool<Vector2Int>.Get();

            bool isSuccess = true;

            foreach (var flowerItem in _inMapFlowers)
            {
                var leftUpGridXY = flowerItem.Key;

                AddGridXY(flowerGridXY, leftUpGridXY, ref result);

                isSuccess = true;
                foreach (var gridXY in result)
                {
                    //1 是否越界
                    if (customGrids.GridIndexIsValid(gridXY) == false)
                    {
                        isSuccess = false;
                        break;
                    }

                    //格子是否已经有鲜花了
                    if (HasFlower(gridXY))
                    {
                        isSuccess = false;
                        break;
                    }
                }

                if (isSuccess)
                {
                    break;
                }
            }

            ListPool<Vector2Int>.Release(result);
            return isSuccess;
        }

        public bool ShapeCanPlaceInAny(GardenFlowerGridTypeConfig gridTypeConfig)
        {
            var listGridXY = ListPool<Vector2Int>.Get();
            customGrids.ListToGridXYNonAlloc(gridTypeConfig.GridData, gridTypeConfig.Row, gridTypeConfig.Col, ref listGridXY);
            var result = FlowerCanPlaceInAny(listGridXY);
            ListPool<Vector2Int>.Release(listGridXY);
            return result;
        }

        //游戏结算全部的花开放
        public async UniTask<float> OnGameSuccessBloomAllFlower()
        {
            //播放成功音效
            AudioMgr.Instance.PlayAudioOneShot(AudioName.FlowerSuccessAudio);

            var lastAllFlowers = new List<Garden_FlowerSingle>();

            foreach (var item in _inMapFlowers)
            {
                if (item.Value != null)
                {
                    lastAllFlowers.Add(item.Value);
                }
            }

            var interval = 0.1f;

            //随机开花
            while (lastAllFlowers.Count > 0)
            {
                var randomFlower = UtilityTool.RandomNumber(lastAllFlowers);
                DoFlowerAnimation(randomFlower);
                lastAllFlowers.Remove(randomFlower);
                await UniTask.WaitForSeconds(interval);
            }

            return 2.5f;
        }
    }
}
