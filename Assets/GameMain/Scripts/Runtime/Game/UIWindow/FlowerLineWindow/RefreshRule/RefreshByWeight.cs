using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.FlowerLine
{
    public class RefreshByWeight : ILineRefreshRule
    {
        private LineLevelConfig _lineLevelConfig;

        public RefreshByWeight() { }

        public void Initialize(int levelId)
        {
            _lineLevelConfig = ConfigMgr.Instance.Tables.LineLevelCategory.GetOrDefault(levelId);
        }

        public void Refresh(out int flowerId, out int flowerLevel)
        {
            flowerId = UtilityTool.RandomKeyByWeight(_lineLevelConfig.FlowerWeight);
            flowerLevel = UtilityTool.RandomKeyByWeight(_lineLevelConfig.FlowerLevelWeight);
        }
    }
}