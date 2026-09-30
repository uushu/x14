using cfg;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace GameMain.Garden
{
    public class Garden_Endless_Refresh : IGardenRefreshRule
    {
        private SortedDictionary<int, int> _gridTypeCacheDict = new();
        public SortedDictionary<int, int> GridTypeCacheDict => _gridTypeCacheDict;

        public List<List<int>> InitMapGridType => LevelConfig.InitMapGridType;

        //第一次刷新可能是配置固定的刷新格子
        private int _refreshIndex;

        public GardenEndlessLevelConfig LevelConfig { get; private set; }

        private Dictionary<int, int> _unlockFlowerWeight;

        public void Initialize(int levelId)
        {
            _refreshIndex = 0;
            LevelConfig = ConfigMgr.Instance.Tables.GardenEndlessLevelCategory.GetOrDefault(levelId);

            _unlockFlowerWeight = new();
            foreach (var flowerKv in LevelConfig.FlowerWeight)
            {
                if (BagMgr.Instance.GetBagPropCount(flowerKv.Key) > 0)
                {
                    _unlockFlowerWeight.Add(flowerKv.Key, flowerKv.Value);
                }
            }
        }

        private void RefreshFlowerTypeWeight()
        {
            _gridTypeCacheDict.Clear();
            var gridTypes = ConfigMgr.Instance.Tables.GardenEndlessGridTypeRefreshCategory.DataList;

            _refreshIndex = Mathf.Min(_refreshIndex, gridTypes.Count - 1);
            var endlessTypeConfig = gridTypes[_refreshIndex];

            _gridTypeCacheDict.Add(101, endlessTypeConfig.FlowerType101);
            _gridTypeCacheDict.Add(102, endlessTypeConfig.FlowerType102);
            _gridTypeCacheDict.Add(103, endlessTypeConfig.FlowerType103);
            _gridTypeCacheDict.Add(104, endlessTypeConfig.FlowerType104);
            _gridTypeCacheDict.Add(105, endlessTypeConfig.FlowerType105);
            _gridTypeCacheDict.Add(106, endlessTypeConfig.FlowerType106);
            _gridTypeCacheDict.Add(107, endlessTypeConfig.FlowerType107);
            _gridTypeCacheDict.Add(108, endlessTypeConfig.FlowerType108);
            _gridTypeCacheDict.Add(109, endlessTypeConfig.FlowerType109);
            _gridTypeCacheDict.Add(110, endlessTypeConfig.FlowerType110);

            _gridTypeCacheDict.Add(111, endlessTypeConfig.FlowerType111);
            _gridTypeCacheDict.Add(112, endlessTypeConfig.FlowerType112);
            _gridTypeCacheDict.Add(113, endlessTypeConfig.FlowerType113);
            _gridTypeCacheDict.Add(114, endlessTypeConfig.FlowerType114);
            _gridTypeCacheDict.Add(115, endlessTypeConfig.FlowerType115);
            _gridTypeCacheDict.Add(116, endlessTypeConfig.FlowerType116);
            _gridTypeCacheDict.Add(117, endlessTypeConfig.FlowerType117);
            _gridTypeCacheDict.Add(118, endlessTypeConfig.FlowerType118);
            _gridTypeCacheDict.Add(119, endlessTypeConfig.FlowerType119);
            _gridTypeCacheDict.Add(120, endlessTypeConfig.FlowerType120);

            _gridTypeCacheDict.Add(121, endlessTypeConfig.FlowerType121);
            _gridTypeCacheDict.Add(122, endlessTypeConfig.FlowerType122);
            _gridTypeCacheDict.Add(123, endlessTypeConfig.FlowerType123);
            _gridTypeCacheDict.Add(124, endlessTypeConfig.FlowerType124);
            _gridTypeCacheDict.Add(127, endlessTypeConfig.FlowerType127);
            _gridTypeCacheDict.Add(128, endlessTypeConfig.FlowerType128);
            _gridTypeCacheDict.Add(129, endlessTypeConfig.FlowerType129);
            _gridTypeCacheDict.Add(130, endlessTypeConfig.FlowerType130);

            _gridTypeCacheDict.Add(131, endlessTypeConfig.FlowerType131);
            _gridTypeCacheDict.Add(132, endlessTypeConfig.FlowerType132);
            _gridTypeCacheDict.Add(133, endlessTypeConfig.FlowerType133);
            _gridTypeCacheDict.Add(134, endlessTypeConfig.FlowerType134);
            _gridTypeCacheDict.Add(135, endlessTypeConfig.FlowerType135);
            _gridTypeCacheDict.Add(136, endlessTypeConfig.FlowerType136);
            _gridTypeCacheDict.Add(137, endlessTypeConfig.FlowerType137);
            _gridTypeCacheDict.Add(138, endlessTypeConfig.FlowerType138);
            _gridTypeCacheDict.Add(139, endlessTypeConfig.FlowerType139);

            _gridTypeCacheDict.Add(140, endlessTypeConfig.FlowerType140);
            _gridTypeCacheDict.Add(141, endlessTypeConfig.FlowerType141);
            _gridTypeCacheDict.Add(142, endlessTypeConfig.FlowerType142);
            _gridTypeCacheDict.Add(143, endlessTypeConfig.FlowerType143);
        }

        private void RefreshInternal(out int gridTypeId, out int flowerId, ref List<int> flowersId)
        {
            flowerId = RefreshFlowerId();
            gridTypeId = UtilityTool.RandomKeyByWeight(_gridTypeCacheDict);

            var validGridCount = GetFlowerGridShapeValidGridCount(gridTypeId);
            FillGridFlowers(validGridCount, flowerId, ref flowersId);
        }

        public void InsureLeastFlowerCanPlace(int leastCount, out List<GardenRefreshFlowerInfo> listInfos)
        {
            listInfos = GardenRefreshFlowerInfoList.Get(leastCount);
            if (leastCount <= 0)
            {
                Debugger.LogError($"leastCount={leastCount}");
                return;
            }

            var levelCacheWeight = GridTypeCacheDict;
            if (levelCacheWeight.Count <= 0)
            {
                Debugger.LogError("levelCacheWeight<=0");
                return;
            }

            var cloneAllShape = ListPool<GardenFlowerGridTypeConfig>.Get();

            cloneAllShape.Clear();
            foreach (var item in levelCacheWeight)
            {
                //这一关的格子的权重>0才能出现在这关卡
                if (item.Value > 0)
                {
                    var levelWeightShape = ConfigMgr.Instance.Tables.GardenFlowerGridTypeCategory.GetOrDefault(item.Key);
                    cloneAllShape.Add(levelWeightShape);
                }
            }

            int alreadyPushCount = 0;
            while (cloneAllShape.Count > 0)
            {
                var shapeConfig = UtilityTool.RandomNumber(cloneAllShape);
                var isCanPut = GardenGameRoot.Instance.flowerGridsMap.ShapeCanPlaceInAny(shapeConfig);

                if (isCanPut)
                {
                    listInfos[alreadyPushCount].GridTypeId = shapeConfig.Id;

                    var mainFlowerId = RefreshFlowerId();
                    listInfos[alreadyPushCount].MainFlowerId = mainFlowerId;

                    var validGridCount = GetFlowerGridShapeValidGridCount(shapeConfig);

                    //替换为蝴蝶
                    FillGridFlowers(validGridCount, mainFlowerId, ref listInfos[alreadyPushCount].FlowersId);

                    alreadyPushCount++;
                    if (alreadyPushCount >= leastCount)
                    {
                        break;
                    }
                }
                else
                {
                    cloneAllShape.Remove(shapeConfig);
                }
            }

            ListPool<GardenFlowerGridTypeConfig>.Release(cloneAllShape);
        }

        public void FillGridFlowers(int maxExchangeCount, int mainFlowerId, ref List<int> flowersId)
        {
            if (maxExchangeCount <= 0)
            {
                Debug.LogError($"刷新蝴蝶数量出错:{maxExchangeCount} <= 0 mainFlowerId={mainFlowerId}");
            }

            flowersId.Clear();

            int butterflyCount = 0;
            for (int i = 0; i < maxExchangeCount; i++)
            {
                var isOk = RefreshBuffterFly(out var butterflyId);

                if (isOk)
                {
                    flowersId.Add(butterflyId);
                    butterflyCount++;
                }
                else
                {
                    flowersId.Add(mainFlowerId);
                }
            }
        }

        public bool RefreshBuffterFly(out int butterFlyId)
        {
            butterFlyId = 0;
            return false;
        }

        public int RefreshFlowerId()
        {
            return UtilityTool.RandomKeyByWeight(_unlockFlowerWeight);
        }

        private int GetFlowerGridShapeValidGridCount(int gridTypeId)
        {
            var gridTypeConfig = ConfigMgr.Instance.Tables.GardenFlowerGridTypeCategory.GetOrDefault(gridTypeId);
            return GetFlowerGridShapeValidGridCount(gridTypeConfig);
        }

        private int GetFlowerGridShapeValidGridCount(GardenFlowerGridTypeConfig gridTypeConfig)
        {
            return gridTypeConfig.ValidGridCount;
        }

        public void RefreshFlowerInfo(int maxCount, int leastCount, out List<GardenRefreshFlowerInfo> result)
        {
            RefreshFlowerTypeWeight();

            if (LevelConfig.InitWaitGridType.Count > _refreshIndex)
            {
                var fixGridTypeIds = LevelConfig.InitWaitGridType[_refreshIndex];

                //走策划配置的固定刷新的格子
                var minCount = Mathf.Min(maxCount, fixGridTypeIds.Count);
                result = GardenRefreshFlowerInfoList.Get(maxCount);

                for (int i = 0; i < minCount; i++)
                {
                    result[i].GridTypeId = fixGridTypeIds[i];
                }

                //剩下的走随机
                for (int i = minCount; i < maxCount; i++)
                {
                    var gridTypeId = UtilityTool.RandomKeyByWeight(_gridTypeCacheDict);
                    result[i].GridTypeId = gridTypeId;
                }

                for (int i = 0; i < maxCount; i++)
                {
                    var flowerId = RefreshFlowerId();
                    result[i].MainFlowerId = flowerId;

                    var validGridCount = GetFlowerGridShapeValidGridCount(result[i].GridTypeId);
                    //填充鲜花格子
                    FillGridFlowers(validGridCount, result[i].MainFlowerId, ref result[i].FlowersId);
                }
            }
            else
            {
                result = GardenRefreshFlowerInfoList.Get(maxCount);

                //走随机
                for (int i = 0; i < maxCount - leastCount; i++)
                {
                    RefreshInternal(out var gridTypeId, out var flowerId, ref result[i].FlowersId);
                    result[i].GridTypeId = gridTypeId;
                    result[i].MainFlowerId = flowerId;
                }

                //至少N个可以放入地图
                InsureLeastFlowerCanPlace(leastCount, out var list2);

                for (int i = 0; i < list2.Count; i++)
                {
                    var resultIndex = i + (maxCount - leastCount);

                    result[resultIndex].GridTypeId = list2[i].GridTypeId;
                    result[resultIndex].MainFlowerId = list2[i].MainFlowerId;
                    result[resultIndex].FlowersId.AddRange(list2[i].FlowersId);
                }

                GardenRefreshFlowerInfoList.Release(list2);
            }

            _refreshIndex++;
        }

        public void ReliveRefreshFlowerInfo(int refreshCount, out List<GardenRefreshFlowerInfo> result)
        {
            //复活固定刷新格子类型
            var flowerGridTypeList = new List<int>() { 101, 103, 105, 111, 117 };

            result = GardenRefreshFlowerInfoList.Get(refreshCount);

            for (int i = 0; i < refreshCount; i++)
            {
                var gridTypeId = UtilityTool.RandomNumber(flowerGridTypeList);
                var flowerId = RefreshFlowerId();

                var validGridCount = GetFlowerGridShapeValidGridCount(gridTypeId);
                FillGridFlowers(validGridCount, flowerId, ref result[i].FlowersId);

                result[i].GridTypeId = gridTypeId;
                result[i].MainFlowerId = flowerId;
            }
        }
    }
}