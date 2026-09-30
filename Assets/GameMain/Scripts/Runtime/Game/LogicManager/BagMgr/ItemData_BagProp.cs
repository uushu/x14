using System;
using System.Collections;
using System.Collections.Generic;
using cfg;

namespace GameMain
{
    public class ItemData_BagProp
    {
        public BagPropConfig Config { get; private set; }

        public int PropId => Config.Id;
        
        public float PropCount { get; private set; }

        public string PropCountString => UtilityTool.ShortNumberToString(PropCount);
        
        public ItemData_BagProp(int propId, float propCount)
        {
            Config = ConfigMgr.Instance.Tables.BagPropCategory.GetOrDefault(propId);
            if (Config == null)
            {
                Debugger.LogError($"不存在的道具:{propId}");
            }
            PropCount = propCount;
        }

        public bool BagPropEnough(int propCount)
        {
            return PropCount >= propCount;
        }

        public void AddProp(float addCount)
        {
            PropCount += addCount;
        }

        public void SetBagProp(float bagPropCount)
        {
            PropCount = bagPropCount;
        }

        public void RemoveProp(float removeCount)
        {
            PropCount -= removeCount;
        }

        public void EqualProp(float propCount)
        {
            PropCount = propCount;
        }
    }
}