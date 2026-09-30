using System.Collections.Generic;
using cfg;

namespace GameMain
{
    [System.Serializable]
    public class Save_Shop : ASaveData
    {
        public List<ShopItemLookAdCount> ShopItemLookAdCountList = new List<ShopItemLookAdCount>();

        public override void SetDefaultData(GlobalConfigCategory category)
        {
        }

        public ShopItemLookAdCount getShopItemLookAdCount(int id)
        {
            if (ShopItemLookAdCountList == null)
            {
                ShopItemLookAdCountList = new();
            }
            var item = ShopItemLookAdCountList.Find(x => x.Id == id);
            if (item == null)
            {
                item = new ShopItemLookAdCount() { Id = id };
                ShopItemLookAdCountList.Add(item);
            }
            return item;
        }
    }

    [System.Serializable]
    public class ShopItemLookAdCount
    {
        public int Id;

        public int Count;
    }
}