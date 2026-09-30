using System.Collections.Generic;
using System.Linq;
using cfg;

namespace GameMain
{
    [System.Serializable]
    public class Save_FlowerTreasureChest : ASaveData
    {
        public List<CollectFlowerTreasureChest> CollectFlowerTreasureChestDatas = new();
        public override void SetDefaultData(GlobalConfigCategory category)
        {
          
        }

        public CollectFlowerTreasureChest GetCurFlowerTreasureChestId(
            EMiniGameType GameType = EMiniGameType.GardenFlower)
        {
            if (CollectFlowerTreasureChestDatas == null)
            {
                CollectFlowerTreasureChestDatas = new();
            }

            var collectFlowerTreasureChest =
                CollectFlowerTreasureChestDatas.FirstOrDefault(x => x.gameType == (int)GameType);
            if (collectFlowerTreasureChest == null)
            {
                collectFlowerTreasureChest = new CollectFlowerTreasureChest()
                {
                    id = ConfigMgr.Instance.Tables.FlowerTreasureChestCategory.DataList[0].Id, gameType = (int)GameType,
                };
                CollectFlowerTreasureChestDatas.Add(collectFlowerTreasureChest);
            }

            return collectFlowerTreasureChest;
        }

        public void SetNextFlowerTreasureChest(EMiniGameType GameType = EMiniGameType.BlockFlower)
        {
            var collectFlowerTreasureChest =
                CollectFlowerTreasureChestDatas.FirstOrDefault(x => x.gameType == (int)GameType);
            if (collectFlowerTreasureChest != null)
            {
                collectFlowerTreasureChest.isClaim = true;
                var nextId = collectFlowerTreasureChest.id + 1;
                var nextConfig =
                    ConfigMgr.Instance.Tables.FlowerTreasureChestCategory.DataList.FirstOrDefault(x => x.Id == nextId);
                if (nextConfig != null)
                {
                    collectFlowerTreasureChest.id = nextId;
                    collectFlowerTreasureChest.CollectCount = 0;
                    collectFlowerTreasureChest.isClaim = false;
                }
                GameEventDefine.SendCollectFlowerTreasureChestChange();
            }
        }
        
    }

    [System.Serializable]
    public class CollectFlowerTreasureChest
    {
        public int id;
        public int gameType;
        public int CollectCount;
        public bool isClaim;
    }
}