using System.Collections;
using System.Collections.Generic;
using cfg;

namespace GameMain
{
    public class BagMgr : ASingleBase<BagMgr>
    {
        public enum EBagPropType
        {
            //局内使用
            InGameUse = 1,

            //装饰道具
            Decoration,
        }

        private readonly Dictionary<int, ItemData_BagProp> _bagPropDict = new();

        public override void Initialize()
        {
            base.Initialize();

            InitPropDict();
        }

        private void InitPropDict()
        {
            _bagPropDict.Clear();

            var propList = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_BagProp>(false).savePropList;

            for (int i = 0; i < propList.Count; i++)
            {
                AddBagProp(propList[i].bagPropId, propList[i].bagPropCount, false);
            }
        }

        public ItemData_BagProp TryGetBagProp(int propId)
        {
            return _bagPropDict.GetValueOrDefault(propId);
        }

        public float GetBagPropCount(int propId)
        {
            var bagProp = TryGetBagProp(propId);

            return bagProp?.PropCount ?? 0;
        }

        public void AddBagProp(List<ItemData_BagProp> bagProps, bool saveToLocal = true)
        {
            for (int i = 0; i < bagProps.Count; i++)
            {
                AddBagProp(bagProps[i].PropId, bagProps[i].PropCount, saveToLocal);
            }
        }

        public void SetBagProp(int bagPropId, float addCount, bool saveToLocal)
        {
            ItemData_BagProp bagPropItem = TryGetBagProp(bagPropId);

            if (bagPropItem == null)
            {
                bagPropItem = new ItemData_BagProp(bagPropId, addCount);
                _bagPropDict.Add(bagPropId, bagPropItem);
            }
            else
            {
                bagPropItem.SetBagProp(addCount);
            }

            Debugger.Log($"道具数量 Id:{bagPropId} 添加数量:{addCount} 剩余数量:{bagPropItem.PropCount}");

            //更新
            if (saveToLocal)
            {
                LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_BagProp>().SetBagPropItem(bagPropId, addCount);
            }
        }

        public void AddBagProp(int bagPropId, float addCount, bool saveToLocal = true)
        {
            ItemData_BagProp bagPropItem = TryGetBagProp(bagPropId);

            if (bagPropItem == null)
            {
                bagPropItem = new ItemData_BagProp(bagPropId, addCount);
                _bagPropDict.Add(bagPropId, bagPropItem);
            }
            else
            {
                bagPropItem.AddProp(addCount);
            }
            Debugger.Log($"添加道具成功 Id:{bagPropId} 添加数量:{addCount} 剩余数量:{bagPropItem.PropCount}");

            //更新
            if (saveToLocal)
            {
                LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_BagProp>().AddBagPropItem(bagPropId, addCount);
            }

            GameEventDefine.SendGetBagProp(bagPropId, addCount);
        }

        /// <summary>
        /// 道具是否充足
        /// </summary>
        public bool BagPropEnough(List<ItemData_BagProp> bagProps, bool debug = true)
        {
            for (int i = 0; i < bagProps.Count; i++)
            {
                if (BagPropEnough(bagProps[i], debug) == false)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 道具是否充足
        /// </summary>
        public bool BagPropEnough(ItemData_BagProp bagProp, bool debug = true)
        {
            return BagPropEnough(bagProp.PropId, bagProp.PropCount, debug);
        }

        /// <summary>
        /// 道具是否充足
        /// </summary>
        public bool BagPropEnough(int bagPropId, float propCount, bool debug = true)
        {
            var bagPropItem = TryGetBagProp(bagPropId);

            if (bagPropItem == null)
            {
                if (debug)
                {
                    Debugger.Log($"不存在的道具 道具Id:{bagPropId} propCount:{propCount}");
                }

                return false;
            }

            if (bagPropItem.PropCount < propCount)
            {
                if (debug)
                {
                    Debugger.Log($"道具不足 道具Id:{bagPropId} 需要数量:{propCount} 拥有数量:{bagPropItem.PropCount}");
                }

                return false;
            }

            return true;
        }

        public void RemoveBagProp(int bagPropId, float removeCount)
        {
            var bagPropItem = TryGetBagProp(bagPropId);

            if (bagPropItem == null)
            {
                Debugger.LogError($"移除道具失败 不存在的道具 bagPropId:{bagPropId} removeCount:{removeCount}");
                return;
            }

            if (bagPropItem.PropCount < removeCount)
            {
                Debugger.LogError(
                    $"移除道具失败 道具不足 bagPropId:{bagPropId} removeCount:{removeCount} 拥有数量:{bagPropItem.PropCount}");
                return;
            }

            bagPropItem.RemoveProp(removeCount);
            Debugger.Log($"移除道具成功 Id:{bagPropId} 移除数量:{removeCount} 剩余数量:{bagPropItem.PropCount}");

            //更新
            LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_BagProp>().RemoveBagPropItem(bagPropId, removeCount);

            GameEventDefine.SendUseBagProp(bagPropId, removeCount);
        }

        public override void UnInitialize()
        {
            base.UnInitialize();

            _bagPropDict.Clear();
        }
    }
}