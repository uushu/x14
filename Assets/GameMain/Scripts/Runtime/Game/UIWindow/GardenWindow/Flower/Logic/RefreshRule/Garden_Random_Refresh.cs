using cfg;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.Garden
{
    public class Garden_Random_Refresh : IGardenRefreshRule
    {
        private List<GardenFlowerGridTypeConfig> _gridTypeList;
        private List<GardenFlowerTypeConfig> _flowerList;

        public SortedDictionary<int, int> GridTypeCacheDict => null;

        public List<List<int>> InitMapGridType => throw new System.NotImplementedException();

        public void Initialize(int levelId)
        {
            _gridTypeList = ConfigMgr.Instance.Tables.GardenFlowerGridTypeCategory.DataList;
            _flowerList = ConfigMgr.Instance.Tables.GardenFlowerTypeCategory.DataList;
        }

        public void InsureLeastFlowerCanPlace(int leastCount, out List<GardenRefreshFlowerInfo> listInfos)
        {
            throw new System.NotImplementedException();
        }

        public void Refresh(out int gridTypeId, out int flowerId)
        {
            var randomGrid = UtilityTool.RandomNumber(_gridTypeList);
            var randomFlower = UtilityTool.RandomNumber(_flowerList);

            gridTypeId = randomGrid.Id;
            flowerId = randomFlower.Id;
        }

        public void Refresh(int count, ref List<int> gridsType, ref List<int> flowerIds, List<int> butterflyIds)
        {
            gridsType.Clear();
            flowerIds.Clear();
            for (int i = 0; i < count; i++)
            {
                Refresh(out var gridTypeId, out var flowerId);
                gridsType.Add(gridTypeId);
                flowerIds.Add(flowerId);
            }
        }

        public void FillGridFlowers(int refreshButterflyCount, int mainFlowerId, ref List<int> buffterFlyIds)
        {
            buffterFlyIds.Clear();
        }

        public bool RefreshBuffterFly(out int butterFlyId)
        {
            throw new System.NotImplementedException();
        }

        public int RefreshFlowerId()
        {
            var randomFlower = UtilityTool.RandomNumber(_flowerList);
            return randomFlower.Id;
        }

        public void RefreshFlowerInfo(int count, int leastCount, out List<GardenRefreshFlowerInfo> flowerInfos)
        {
            flowerInfos = GardenRefreshFlowerInfoList.Get(count);

            for (int i = 0; i < flowerInfos.Count; i++)
            {
                Refresh(out var gridTypeId, out var flowerId);

                flowerInfos[i].GridTypeId = gridTypeId;
                flowerInfos[i].MainFlowerId = flowerId;
            }
        }

        public void ReliveRefreshFlowerInfo(int refreshCount, out List<GardenRefreshFlowerInfo> flowerInfos)
        {
            throw new System.NotImplementedException();
        }
    }
}