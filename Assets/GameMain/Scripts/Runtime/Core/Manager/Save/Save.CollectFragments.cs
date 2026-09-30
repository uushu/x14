
using System.Collections.Generic;
using System.Linq;
using cfg;
using Newtonsoft.Json;
using UnityEngine;

namespace GameMain
{
    [System.Serializable]
    public class Save_CollectFragments : ASaveData
    {
        public List<int> fragments = new List<int>();

        public List<CollectFlower> flowers = new List<CollectFlower>();

        public List<int> NewGet = new List<int>();
        
        public override void SetDefaultData(GlobalConfigCategory category)
        {
            fragments= new List<int>();
            foreach (var flower in  ConfigMgr.Instance.Tables.FlowerCollectCategory.DataList)
            {
                var collectFlower = new CollectFlower() { id = flower.Id, UnLock = flower.UnlockType== EUnLockType.None, adCount = 0 };
                if (collectFlower.UnLock)
                {
                    foreach (var item in  flower.AwardItem)
                    {
                        BagMgr.Instance.AddBagProp(item.Key,item.Value);
                    }
                }
                flowers.Add(collectFlower);
            }
        }

        public void AddNewGetFlower(int id)
        {
            if (!NewGet.Contains(id))
            {
                NewGet.Add(id);
            }
           
        }
        public void RemoveGetFlower(int id)
        {
            NewGet.Remove(id);
        }

        public override void OnAfterDeserialize()
        {
            Debug.Log("Save_CollectFragments OnAfterDeserialize ");
            if (flowers==null)
            {
                flowers = new List<CollectFlower>();
                foreach (var flower in  ConfigMgr.Instance.Tables.FlowerCollectCategory.DataList)
                {
                    var collectFlower = new CollectFlower() { id = flower.Id, UnLock = flower.UnlockType== EUnLockType.None, adCount = 0 };
                    if (collectFlower.UnLock)
                    {
                        foreach (var item in  flower.AwardItem)
                        {
                            BagMgr.Instance.AddBagProp(item.Key,item.Value);
                        }
                    }
                    flowers.Add(collectFlower);
                }
            }
            else
            {
                foreach (var flower in  ConfigMgr.Instance.Tables.FlowerCollectCategory.DataList)
                {
                    var find = flowers.FirstOrDefault(s => s.id == flower.Id);
                    if (find==null)
                    {
                        var collectFlower = new CollectFlower() { id = flower.Id, UnLock = flower.UnlockType== EUnLockType.None, adCount = 0 };
                        if (collectFlower.UnLock)
                        {
                            foreach (var item in  flower.AwardItem)
                            {
                                BagMgr.Instance.AddBagProp(item.Key,item.Value);
                            }
                        }
                        flowers.Add(collectFlower);
                    }
                }
            }
        }
    }
    
    [System.Serializable]
    public class CollectFlower
    {
        public int id;

        public bool UnLock;
        
        public int adCount;
        [JsonIgnore]
        public FlowerCollectConfig Config=> ConfigMgr.Instance?.Tables?.FlowerCollectCategory?.GetOrDefault(id);
    }
}