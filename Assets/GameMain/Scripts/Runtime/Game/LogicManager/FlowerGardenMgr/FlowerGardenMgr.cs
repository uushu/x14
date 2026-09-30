using System.Collections.Generic;
using System.Linq;
using cfg;
using UnityEngine;

namespace GameMain
{
    public class FlowerGardenMgr: ASingleBase<FlowerGardenMgr>
    {
        private List<GardenLevelConfig> gardenLevelExtraAwardConfigs = new List<GardenLevelConfig>();
        public override void Initialize()
        {
            foreach (var item in  ConfigMgr.Instance.Tables.GardenLevelCategory.DataList)
            {
                if (item.ExtraAwardProps.Count>0)
                {
                    gardenLevelExtraAwardConfigs.Add(item);
                }
            }
        }

        public (float,int,int) GetExtraAwardProgress(int levelId)
        {
            var MinConfig = gardenLevelExtraAwardConfigs.LastOrDefault(x => x.Id < levelId);
            var MinId = MinConfig != null ? MinConfig.Id: 1000;
            
            var MaxConfig = gardenLevelExtraAwardConfigs.FirstOrDefault(x => x.Id >= levelId);
            var MaxId = MaxConfig != null ? MaxConfig.Id : ConfigMgr.Instance.Tables.GardenLevelCategory.Get(ConfigMgr.Instance.Tables.GardenLevelCategory.DataList.Count-1).Id;
            //Debug.Log("MinId:"+MinId+" MaxId:"+MaxId+" levelId:"+levelId+" progress："+(levelId-MinId)*1.0f/(MaxId-MinId) );
            
            return ( (levelId-MinId)*1.0f/(MaxId-MinId) ,MaxConfig.Id-levelId,MaxId);
        }


        public override void UnInitialize()
        {
            gardenLevelExtraAwardConfigs.Clear();
        }
    }
}