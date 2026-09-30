using cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Serialization;
using System.Linq;

namespace GameMain
{
    [System.Serializable]
    public class Save_Main : ASaveData
    {
        /// <summary>
        /// 邀请领取列表
        /// </summary>
        public List<int> inviteGetList;
        /// <summary>
        /// 已经邀请人数
        /// </summary>
        public int inviteCount;

        //上次免费分享获取体力时间戳
        public long lastFreeGetPowerStamp;

        //是否获得过收藏奖励
        public bool HaveGetCollectAward;

        //免费分享获取体力
        public int freeShareGetPower = 1;

        public SerializableDictionary<int, CollectionThemeItemSaveData> allThemeItem;

        public override void SetDefaultData(GlobalConfigCategory category)
        {
            inviteGetList= new List<int>();
            allThemeItem = new SerializableDictionary<int, CollectionThemeItemSaveData>();
            foreach (ThemeSkinConfig item in ConfigMgr.Instance.Tables.ThemeSkinCategory.DataList)
            {
                allThemeItem.Dictionary.Add(item.Id, new CollectionThemeItemSaveData()
                {
                    tableId = item.Id, inUse = false
                });
            }

            //默认使用第一个
            KeyValuePair<int, CollectionThemeItemSaveData> temp = allThemeItem.Dictionary.FirstOrDefault();
            temp.Value.inUse = true;
        }

        public override void OnAfterDeserialize()
        {
            foreach (ThemeSkinConfig item in ConfigMgr.Instance.Tables.ThemeSkinCategory.DataList)
            {
                if (!allThemeItem.Dictionary.ContainsKey(item.Id))
                {
                    allThemeItem.Dictionary.Add(item.Id, new CollectionThemeItemSaveData()
                    {
                        tableId = item.Id, inUse = false
                    });
                }
            }
            if ( inviteGetList==null)
            {
                inviteGetList= new List<int>();
            }
        }

        /// <summary>
        /// 今天是否可以免费分享获取体力
        /// </summary>
        public bool ToDayIsCanFreeShareGetPower()
        {
            //上次免费使用是否是今天
            bool temp = TimeTool.IsSameDay(lastFreeGetPowerStamp, TimeTool.GetNowTimeStamp());
            //不是，跨天重置
            if (!temp)
            {
                lastFreeGetPowerStamp = TimeTool.GetNowTimeStamp();
                freeShareGetPower = 1;
            }

            return freeShareGetPower > 0;
        }

        /// <summary>
        /// 免费分享获取体力
        /// </summary>
        public void TagToDayFreeShareGetPower()
        {
            freeShareGetPower -= 1;
            lastFreeGetPowerStamp = TimeTool.GetNowTimeStamp();
        }

        /// <summary>
        /// 获取当前使用主题
        /// </summary>
        public ThemeSkinConfig GetUseTheme()
        {
            foreach (var item in allThemeItem.Dictionary)
            {
                if (item.Value.inUse)
                {
                    return item.Value.tableData;
                }
            }

            return null;
        }
        /// <summary>
        /// 使用主题
        /// </summary>
        public void UseTheme(int id)
        {
            foreach (var item in allThemeItem.Dictionary)
            {
                if (item.Value.tableId == id)
                {
                    item.Value.inUse = true;
                }
                else
                {
                    item.Value.inUse = false;
                }
            }
        }
        
        /// <summary>
        /// 看广告解锁主题
        /// </summary>
        public void LookAdForTheme(int id,int adsNum)
        {
            if (allThemeItem.Dictionary.TryGetValue(id, out var item))
            {
                item.adsNum += adsNum;
            }
        }
        /// <summary>
        /// 记录已领取邀请奖励 
        /// </summary>
        public void AddGetInvite(int tableId)
        {
            inviteGetList.Add(tableId);
        }
        /// <summary>
        /// 是否已经领取过邀请奖励
        /// </summary>
        public bool IsGetInvite(int tableId)
        {
            return inviteGetList.Contains(tableId);
        }
        
        /// <summary>
        /// 记录已领取邀请奖励 
        /// </summary>
        public void AddInviteCount(int count)
        {
            inviteCount += count;
        }
        
        /// <summary>
        /// 获取已领取邀请奖励 
        /// </summary>
        public int GetInviteCount()
        {
            return inviteCount;
        }
    }

    [Serializable]
    public class CollectionThemeItemSaveData
    {
        public int tableId;
        public bool inUse;
        public int adsNum;

        [JsonIgnore]
        public ThemeSkinConfig tableData => ConfigMgr.Instance.Tables.ThemeSkinCategory.GetOrDefault(tableId);

        [JsonIgnore]
        public CollectionThemeState State
        {
            get
            {
                if (inUse)
                {
                    return CollectionThemeState.InUse;
                }
                else if (IsUnlock())
                {
                    return CollectionThemeState.Unlock;
                }
                else
                {
                    return CollectionThemeState.Lock;
                }
            }
        }

        public bool IsUnlock()
        {
            switch (tableData.UnlockType)
            {
                case 1: //关卡解锁
                {
                    int tempId = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().levelId;
                    tempId = tempId - 1000;
                    return tempId >= tableData.UnlockNum;
                }
                case 2: //收集解锁
                    return BagMgr.Instance.BagPropEnough(tableData.GetItemId, tableData.UnlockNum);
                case 3: //广告解锁
                    return adsNum >= tableData.UnlockNum;
                default:
                    break;
            }

            return false;
        }

        /// <summary>
        /// 解锁进度
        /// </summary>
        public (float press, float value, float maxValue) GetUnlockPress()
        {
            switch (tableData.UnlockType)
            {
                case 1: //关卡解锁
                {
                    int tempId = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().levelId;
                    tempId = tempId - 1000;
                    return (tempId / (float) tableData.UnlockNum, tempId, tableData.UnlockNum);
                }
                case 2: //收集解锁
                {
                    float temp = BagMgr.Instance.GetBagPropCount(tableData.GetItemId);
                    return (temp / (float) tableData.UnlockNum, temp, tableData.UnlockNum);
                }
                case 3: //广告解锁
                    return (adsNum / (float) tableData.UnlockNum, adsNum, tableData.UnlockNum);
                default:
                    break;
            }

            return (0, 0, 0);
        }
    }

    /// <summary>
    /// 状态
    /// </summary>
    public enum CollectionThemeState
    {
        Lock,
        Unlock,
        InUse
    }
}