using cfg;
using System.Collections;
using System.Collections.Generic;

namespace GameMain.FlowerBlock
{
    public class RefreshBlockItemData
    {
        public int waveIndex;

        public int startGridX;

        public int blockTypeId;

        public int mainFlowerId;

        public List<int> flowersId = new();

        public BlockGridTypeConfig Config => ConfigMgr.Instance.Tables.BlockGridTypeCategory.GetOrDefault(blockTypeId);

        public override string ToString()
        {
            return $"waveIndex={waveIndex} startX={startGridX} blockTypeId={blockTypeId}";
        }
    }

    public interface IBlockRefreshRule
    {
        public void Initialize(int levelId);

        void ResetWaveIndex();

        //下标0开始
        public List<RefreshBlockItemData> RefreshBlockItems();
    }
}