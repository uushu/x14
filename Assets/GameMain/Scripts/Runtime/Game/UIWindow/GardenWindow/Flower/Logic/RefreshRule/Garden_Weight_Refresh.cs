using cfg;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace GameMain.Garden
{
    public class Garden_Weight_Refresh : IGardenRefreshRule
    {
        private SortedDictionary<int, int> _gridTypeCacheDict = new();

        //主鲜花的权重
        private Dictionary<int, int> _mainFlowerWeight => _levelConfig.FlowerWeight;

        private Dictionary<int, int> _bufferFlyWeight => _levelConfig.ButterflyWeight;

        private Dictionary<int, int> _allWeight;

        public SortedDictionary<int, int> GridTypeCacheDict => _gridTypeCacheDict;

        public List<List<int>> InitMapGridType => _levelConfig.InitMapGridType;

        private GardenLevelConfig _levelConfig;

        //第一次刷新可能是配置固定的刷新格子
        private int _refreshIndex;

        private int _fromRefreshId => _levelConfig.FlowerGridWeight[0];

        private int _toRefreshId => _levelConfig.FlowerGridWeight[1];

        private int _currentRefreshId => Mathf.Clamp(_fromRefreshId + _refreshIndex, _fromRefreshId, _toRefreshId);

        public void Initialize(int levelId)
        {
            _levelConfig = ConfigMgr.Instance.Tables.GardenLevelCategory.Get(levelId);
            if (_levelConfig == null)
            {
                Debugger.LogError($"不存在的关卡表 GardenLevelCategory:{levelId}");
                return;
            }

            _refreshIndex = 0;
            _allWeight = UtilityTool.DictMerge(_bufferFlyWeight, _mainFlowerWeight);
        }

        private void RefreshFlowerTypeWeight()
        {
            _gridTypeCacheDict.Clear();

            var typeRefreshConfig = ConfigMgr.Instance.Tables.GardenFlowerGridTypeRefreshCategory.Get(_currentRefreshId);
            var gridTypes = ConfigMgr.Instance.Tables.GardenFlowerGridTypeCategory.DataMap;
            if (gridTypes[101].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(101, typeRefreshConfig.FlowerType101);
            if (gridTypes[102].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(102, typeRefreshConfig.FlowerType102);
            if (gridTypes[103].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(103, typeRefreshConfig.FlowerType103);
            if (gridTypes[104].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(104, typeRefreshConfig.FlowerType104);
            if (gridTypes[105].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(105, typeRefreshConfig.FlowerType105);
            if (gridTypes[106].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(106, typeRefreshConfig.FlowerType106);
            if (gridTypes[107].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(107, typeRefreshConfig.FlowerType107);
            if (gridTypes[108].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(108, typeRefreshConfig.FlowerType108);
            if (gridTypes[109].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(109, typeRefreshConfig.FlowerType109);
            if (gridTypes[110].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(110, typeRefreshConfig.FlowerType110);

            if (gridTypes[111].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(111, typeRefreshConfig.FlowerType111);
            if (gridTypes[112].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(112, typeRefreshConfig.FlowerType112);
            if (gridTypes[113].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(113, typeRefreshConfig.FlowerType113);
            if (gridTypes[114].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(114, typeRefreshConfig.FlowerType114);
            if (gridTypes[115].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(115, typeRefreshConfig.FlowerType115);
            if (gridTypes[116].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(116, typeRefreshConfig.FlowerType116);
            if (gridTypes[117].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(117, typeRefreshConfig.FlowerType117);
            if (gridTypes[118].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(118, typeRefreshConfig.FlowerType118);
            if (gridTypes[119].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(119, typeRefreshConfig.FlowerType119);
            if (gridTypes[120].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(120, typeRefreshConfig.FlowerType120);

            if (gridTypes[121].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(121, typeRefreshConfig.FlowerType121);
            if (gridTypes[122].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(122, typeRefreshConfig.FlowerType122);
            if (gridTypes[123].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(123, typeRefreshConfig.FlowerType123);
            if (gridTypes[124].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(124, typeRefreshConfig.FlowerType124);
            if (gridTypes[127].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(127, typeRefreshConfig.FlowerType127);
            if (gridTypes[128].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(128, typeRefreshConfig.FlowerType128);
            if (gridTypes[129].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(129, typeRefreshConfig.FlowerType129);
            if (gridTypes[130].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(130, typeRefreshConfig.FlowerType130);

            if (gridTypes[131].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(131, typeRefreshConfig.FlowerType131);
            if (gridTypes[132].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(132, typeRefreshConfig.FlowerType132);
            if (gridTypes[133].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(133, typeRefreshConfig.FlowerType133);
            if (gridTypes[134].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(134, typeRefreshConfig.FlowerType134);
            if (gridTypes[135].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(135, typeRefreshConfig.FlowerType135);
            if (gridTypes[136].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(136, typeRefreshConfig.FlowerType136);
            if (gridTypes[137].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(137, typeRefreshConfig.FlowerType137);
            if (gridTypes[138].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(138, typeRefreshConfig.FlowerType138);
            if (gridTypes[139].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(139, typeRefreshConfig.FlowerType139);

            if (gridTypes[140].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(140, typeRefreshConfig.FlowerType140);
            if (gridTypes[141].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(141, typeRefreshConfig.FlowerType141);
            if (gridTypes[142].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(142, typeRefreshConfig.FlowerType142);
            if (gridTypes[143].SpawnRefreshCount <= _refreshIndex) _gridTypeCacheDict.Add(143, typeRefreshConfig.FlowerType143);
        }

        private void FillGridsInternal(out int gridTypeId, out int flowerId, ref List<int> flowersId)
        {
            flowerId = UtilityTool.RandomKeyByWeight(_mainFlowerWeight);
            gridTypeId = UtilityTool.RandomKeyByWeight(_gridTypeCacheDict);

            var validGridCount = GetFlowerGridShapeValidGridCount(gridTypeId);
            FillGridFlowers(validGridCount, flowerId, ref flowersId);
        }

        public int RefreshFlowerId()
        {
            return UtilityTool.RandomKeyByWeight(_mainFlowerWeight);
        }

        public void RefreshFlowerInfo(int maxCount, int leastCount, out List<GardenRefreshFlowerInfo> result)
        {
            RefreshFlowerTypeWeight();

            if (_levelConfig.InitWaitGridType.Count > _refreshIndex)
            {
                var fixGridTypeIds = _levelConfig.InitWaitGridType[_refreshIndex];

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
                    var flowerId = UtilityTool.RandomKeyByWeight(_mainFlowerWeight);
                    result[i].MainFlowerId = flowerId;

                    var validGridCount = GetFlowerGridShapeValidGridCount(result[i].GridTypeId);
                    //替换为蝴蝶
                    FillGridFlowers(validGridCount, result[i].MainFlowerId, ref result[i].FlowersId);
                }
            }
            else
            {
                result = GardenRefreshFlowerInfoList.Get(maxCount);

                //走随机
                for (int i = 0; i < maxCount - leastCount; i++)
                {
                    FillGridsInternal(out var gridTypeId, out var flowerId, ref result[i].FlowersId);
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

        public int RefreshFlowerTypeId()
        {
            RefreshFlowerTypeWeight();
            var gridTypeId = UtilityTool.RandomKeyByWeight(_gridTypeCacheDict);
            return gridTypeId;
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

            Debug.Log($"最大刷新格子数:{maxExchangeCount} 替换蝴蝶数:{butterflyCount}");
        }

        public bool RefreshBuffterFly(out int outId)
        {
            var butterFlyId = UtilityTool.RandomKeyByWeight(_allWeight);

            if (_bufferFlyWeight.ContainsKey(butterFlyId))
            {
                outId = butterFlyId;
                return true;
            }

            outId = 0;
            return false;
        }

        private int GetFlowerGridShapeValidGridCount(GardenFlowerGridTypeConfig gridTypeConfig)
        {
            return gridTypeConfig.ValidGridCount;
        }

        private int GetFlowerGridShapeValidGridCount(int gridTypeId)
        {
            var gridTypeConfig = ConfigMgr.Instance.Tables.GardenFlowerGridTypeCategory.GetOrDefault(gridTypeId);
            return GetFlowerGridShapeValidGridCount(gridTypeConfig);
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