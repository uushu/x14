using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cfg;

namespace GameMain
{
    public class CollectFlowerMgr : ASingleBase<CollectFlowerMgr>
    {
        private EventGroup _eventGroup = new();

        public override void Initialize()
        {
            base.Initialize();
            _eventGroup.AddListener<GameEventDefine.FlowerGardenLevelPass>(ProcessMessage);
            _eventGroup.AddListener<GameEventDefine.CollectButterFly>(ProcessMessage);
            _eventGroup.AddListener<GameEventDefine.GetBagProp>(ProcessMessage);
        }

        public List<int> GetCollectFlowerOpenData()
        {
            List<int> OpenData = new();
            foreach (var config in ConfigMgr.Instance.Tables.BagPropCategory.DataList)
            {
                if (config.ItemType == 4 && BagMgr.Instance.GetBagPropCount(config.Id) > 0)
                {
                    OpenData.Add(config.FlowerId);
                }
            }

            return OpenData;
        }

        public void ProcessMessage(IEventMessage obj)
        {
            if (obj is GameEventDefine.FlowerGardenLevelPass)
            {
                var datas = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_CollectFragments>(true).flowers;
                foreach (var data in datas)
                {
                    if (data != null && data.UnLock == false)
                    {
                        switch (data.Config.UnlockType)
                        {
                            case EUnLockType.LevelUnLock:
                                var level = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().levelId;
                                data.UnLock = level >= data.Config.UnlockValue;
                                break;
                            case EUnLockType.VictoriesUnLock:
                                var currentSuccessCount = LocalSaveMgr.Instance.DataCenter
                                    .GetLocalSave<Save_FlowerGarden>().currentSuccessCount;
                                data.UnLock = currentSuccessCount >= data.Config.UnlockValue;
                                break;
                        }

                        if (data.UnLock)
                        {
                            foreach (var item in data.Config.AwardItem)
                            {
                                BagMgr.Instance.AddBagProp(item.Key, item.Value);
                            }
                        }
                    }
                }
            }
            else if (obj is GameEventDefine.CollectButterFly result)
            {
                //花花宝箱
                var datas = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerTreasureChest>(true)
                    .GetCurFlowerTreasureChestId(result.GameType);
                if (datas != null)
                {
                    var config = ConfigMgr.Instance.Tables.FlowerTreasureChestCategory.Get(datas.id);
                    if (config != null)
                    {
                        result.ButterflyDict.TryGetValue(config.FlowerType_Ref.Id, out var count);
                        datas.CollectCount += count;
                    }

                    GameEventDefine.SendCollectFlowerTreasureChestChange();
                }
            }
            else if (obj is GameEventDefine.GetBagProp item)
            {
                var config = ConfigMgr.Instance.Tables.BagPropCategory.Get(item.BagPropId);
                if (config != null && config.ItemType == 4)
                {
                    var data = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_CollectFragments>(true);
                    if (data != null)
                    {
                        data.AddNewGetFlower(config.FlowerId);
                        GameEventDefine.SendCollectFlowerChange();
                    }
                }
            }
        }

        public void CollectFloweraddAdCount(int id, int count)
        {
            var datas = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_CollectFragments>(true).flowers;
            var data = datas.FirstOrDefault(x => x.id == id);
            if (data != null && data.Config.UnlockType == EUnLockType.AdUnLock && data.UnLock == false)
            {
                data.adCount += count;
                data.UnLock = data.adCount >= data.Config.UnlockValue;
            }

            if (data.UnLock)
            {
                foreach (var item in data.Config.AwardItem)
                {
                    BagMgr.Instance.AddBagProp(item.Key, item.Value);
                }
            }

            GameEventDefine.SendCollectFlowerAdCountChange();
        }

        public CollectFlower GetCollectFlower(int id)
        {
            var datas = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_CollectFragments>(true).flowers;
            var data = datas.FirstOrDefault(x => x.id == id);
            return data;
        }

        public override void UnInitialize()
        {
            _eventGroup.RemoveAllListener();
        }
    }
}