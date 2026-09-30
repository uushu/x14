using cfg;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.FlowerBlock
{
    public class BlockRefresh : ABlockRefreshByWeight
    {
        private BlockLevelConfig _levelConfig;
        protected override Dictionary<int, int> _flowerIdWeight => _levelConfig.FlowerWeight;
        protected override Dictionary<int, int> _butterflyWeight => _levelConfig.CollectButterflyWeight;

        protected override List<List<int>> _fixRefreshList => _levelConfig.FixRefreshList;

        public override void Initialize(int levelId)
        {
            _levelConfig = ConfigMgr.Instance.Tables.BlockLevelCategory.GetOrDefault(levelId);

            base.Initialize(levelId);
        }

        protected override void GetWeightCahe(int waveIndex, ref SortedDictionary<int, int> result)
        {
            var waveConfig = ConfigMgr.Instance.Tables.BlockGridTypeRefreshCategory.DataList[waveIndex];
            var randomCount = UtilityTool.RandomKeyByWeight(waveConfig.RandomMaxCount);
            result.Clear();

            result.Add(1011, waveConfig.FlowerType1011);
            result.Add(1012, waveConfig.FlowerType1012);
            result.Add(1013, waveConfig.FlowerType1013);
            result.Add(1014, waveConfig.FlowerType1014);

            result.Add(1021, waveConfig.FlowerType1021);
            result.Add(1022, waveConfig.FlowerType1022);
            result.Add(1023, waveConfig.FlowerType1023);

            result.Add(1031, waveConfig.FlowerType1031);
            result.Add(1032, waveConfig.FlowerType1032);

            result.Add(1041, waveConfig.FlowerType1041);
        }
    }
}