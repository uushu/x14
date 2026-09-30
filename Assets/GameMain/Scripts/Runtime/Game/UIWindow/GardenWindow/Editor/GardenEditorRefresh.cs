using cfg;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.Garden.Editor
{
#if UNITY_EDITOR
    public class GardenEditorRefresh
    {
        private SortedDictionary<int, int> _gridTypeCacheDict = new();

        public SortedDictionary<int, int> GridTypeCacheDict => _gridTypeCacheDict;

        private GardenLevelConfig _levelConfig;
        public int LevelId => _levelConfig.Id;

        //第一次刷新可能是配置固定的刷新格子
        private int _refreshIndex;
        private int _fromRefreshId => _levelConfig.FlowerGridWeight[0];
        private int _toRefreshId => _levelConfig.FlowerGridWeight[1];
        private int _currentRefreshId => Mathf.Clamp(_fromRefreshId + _refreshIndex, _fromRefreshId, _toRefreshId);

        private static GardenFlowerGridTypeCategory _gardenFlowerGridTypeCategory;
        public static GardenFlowerGridTypeCategory GardenFlowerGridTypeCategory
        {
            get
            {
                if (_gardenFlowerGridTypeCategory == null)
                {
                    _gardenFlowerGridTypeCategory = AssetLoader.GetCategory_Editor<GardenFlowerGridTypeCategory>();
                }

                return _gardenFlowerGridTypeCategory;
            }
        }

        private GardenFlowerGridTypeRefreshCategory _typeRefreshCategory;
        public GardenFlowerGridTypeRefreshCategory TypeRefreshCategory
        {
            get
            {
                if (_typeRefreshCategory == null)
                {
                    _typeRefreshCategory = AssetLoader.GetCategory_Editor<GardenFlowerGridTypeRefreshCategory>();
                }

                return _typeRefreshCategory;
            }
        }

        private GardenFlowerTypeCategory _flowerTypeCategory;
        public GardenFlowerTypeCategory FlowerTypeCategory
        {
            get
            {
                if (_flowerTypeCategory == null)
                {
                    _flowerTypeCategory = AssetLoader.GetCategory_Editor<GardenFlowerTypeCategory>();
                }

                return _flowerTypeCategory;
            }
        }

        public GardenEditorRefresh(GardenLevelConfig levelConfig)
        {
            _levelConfig = levelConfig;
            _refreshIndex = 0;
        }

        public void RefreshFlowerTypeWeight()
        {
            var typeRefreshConfig = AssetLoader.GetCategory_Editor<GardenFlowerGridTypeRefreshCategory>().Get(_currentRefreshId);

            _gridTypeCacheDict.Clear();
            var gridTypes = GardenFlowerGridTypeCategory.DataMap;
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

        private void RefreshInternal(out int gridTypeId)
        {
            gridTypeId = UtilityTool.RandomKeyByWeight(_gridTypeCacheDict);
        }

        public List<GardenFlowerGridTypeConfig> RefreshFlowerType(int refreshCount)
        {
            var result = new List<GardenFlowerGridTypeConfig>();

            RefreshFlowerTypeWeight();

            for (int i = 0; i < refreshCount; i++)
            {
                RefreshInternal(out var gridTypeId);
                var gridTypeConfig = GardenFlowerGridTypeCategory.GetOrDefault(gridTypeId);
                result.Add(gridTypeConfig);
            }

            _refreshIndex++;
            return result;
        }

        public int RefreshFlowerId()
        {
            return UtilityTool.RandomKeyByWeight(_levelConfig.FlowerWeight);
        }
    }

#endif
}