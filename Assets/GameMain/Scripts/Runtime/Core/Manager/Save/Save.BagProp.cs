using System.Collections;
using System.Collections.Generic;
using cfg;

namespace GameMain
{
    [System.Serializable]
    public class SaveBagPropItem
    {
        public int bagPropId;

        public float bagPropCount;

        public override string ToString()
        {
            return $"背包,道具Id:{bagPropId} 道具数量:{bagPropCount}";
        }
    }

    [System.Serializable]
    public class Save_BagProp : ASaveData
    {
        public List<SaveBagPropItem> savePropList;

        public override void SetDefaultData(GlobalConfigCategory category)
        {
            savePropList = new List<SaveBagPropItem>();

            foreach (var item in category.InitBagProps)
            {
                AddBagPropItem(item.Key, item.Value);
            }
        }

        private SaveBagPropItem SafeGetBagPropItem(int propId)
        {
            for (int i = 0; i < savePropList.Count; i++)
            {
                if (savePropList[i].bagPropId == propId)
                {
                    return savePropList[i];
                }
            }

            var newBagProp = new SaveBagPropItem() {
                bagPropId = propId,
                bagPropCount = 0
            };
            
            savePropList.Add(newBagProp);
            return newBagProp;
        }

        private void RemoveBagPropItem(int propId)
        {
            for (int i = 0; i < savePropList.Count; i++)
            {
                if (savePropList[i].bagPropId == propId)
                {
                    savePropList.RemoveAt(i);
                    break;
                }
            }
        }

        public void AddBagPropItem(int propId, float addCount)
        {
            var bagPropItem = SafeGetBagPropItem(propId);

            bagPropItem.bagPropCount += addCount;

            if (propId==GameTool.AchievementPintId)
            {
                GameEntrance.UpdateRoleAchievementRank(bagPropItem.bagPropCount);
            }
        }

        public void SetBagPropItem(int propId, float propCount)
        {
            var bagPropItem = SafeGetBagPropItem(propId);

            bagPropItem.bagPropCount = propCount;
        }

        public void RemoveBagPropItem(int propId, float removeCount)
        {
            var bagPropItem = SafeGetBagPropItem(propId);

            if (bagPropItem.bagPropCount < removeCount)
            {
                Debugger.LogError($"道具不足不能移除:{bagPropItem} 移除道具:{removeCount}");
            }

            bagPropItem.bagPropCount -= removeCount;
            if (bagPropItem.bagPropCount <= 0)
            {
                RemoveBagPropItem(bagPropItem.bagPropId);
            }
        }
    }
}