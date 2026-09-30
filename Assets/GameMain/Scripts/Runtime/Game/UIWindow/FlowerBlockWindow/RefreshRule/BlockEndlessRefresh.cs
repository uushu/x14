using cfg;
using System.Collections;
using System.Collections.Generic;

namespace GameMain.FlowerBlock
{
    public class BlockEndlessRefresh : ABlockRefreshByWeight
    {
        protected override Dictionary<int, int> _flowerIdWeight => _levelConfig.FlowerWeight;

        protected override Dictionary<int, int> _butterflyWeight => new();

        protected override List<List<int>> _fixRefreshList => _levelConfig.FixRefreshList;

        private BlockEndlessLevelConfig _levelConfig;

        private Dictionary<int, int> _unlockFlowerWeight;

        public override void Initialize(int levelId)
        {
            _levelConfig = ConfigMgr.Instance.Tables.BlockEndlessLevelCategory.GetOrDefault(levelId);

            base.Initialize(levelId);

            _unlockFlowerWeight = new();
            foreach (var flowerKv in _levelConfig.FlowerWeight)
            {
                if (BagMgr.Instance.GetBagPropCount(flowerKv.Key) > 0)
                {
                    _unlockFlowerWeight.Add(flowerKv.Key, flowerKv.Value);
                }
            }
        }

        protected override int RefreshMainFlowerId()
        {
            var flowerId = UtilityTool.RandomKeyByWeight(_flowerIdWeight);
            return flowerId;
        }

        protected override void GetWeightCahe(int waveIndex, ref SortedDictionary<int, int> result)
        {
            var waveConfig = ConfigMgr.Instance.Tables.BlockEndlessGridTypeRefreshCategory.DataList[waveIndex];
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