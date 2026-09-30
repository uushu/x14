using System;
using System.Collections.Generic;
using cfg;
using UnityEngine;

namespace GameMain
{
    [System.Serializable]
    public class Save_Sign : ASaveData
    {
        //首次登录时间
        public long firstSignDayTimeStamp;

        //上次登录时间
        public long lastSignDayTimeStamp;

        /// <summary>
        /// 登录天数
        /// </summary>
        public long signDay;

        //上一次签到时间戳
        public long lastSignTimeStamp;

        //每日签到
        public List<int> signDayList;

        public override void SetDefaultData(GlobalConfigCategory category)
        {
            signDayList = new List<int>();
            firstSignDayTimeStamp = TimeTool.GetNowTimeStamp();
        }

        /// <summary>
        /// 当前相对于初始登录天数
        /// </summary>
        public long CurrentSignDay()
        {
            DateTime firstData = TimeTool.ToDateTime(firstSignDayTimeStamp);
            return (long)(TimeTool.GetDay(DateTime.Now) - TimeTool.GetDay(firstData) + 1);
        }

        /// <summary>
        /// 记录登录天数
        /// </summary>
        public bool RecordSignDay()
        {
            if (TimeTool.IsSameDay(lastSignDayTimeStamp, TimeTool.GetNowTimeStamp()))
            {
                return false;
            }
            signDay++;
            lastSignDayTimeStamp = TimeTool.GetNowTimeStamp();
            if (signDay>1)
            {
                GlobalConfigCategory tableData = ConfigMgr.Instance.Tables.GlobalConfigCategory;
                Save_CostPower data = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_CostPower>();
                int addPower = tableData.MaxCostPower - data.CurrentPower;
                addPower = Mathf.Max(addPower, 0);
                CostPowerManager.Instance.AddCostPower(addPower);
            }
            LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>(true).currentChanglleCount = 0;
            return true;
        }

        public bool IsSign(int dayId)
        {
            return signDayList.Contains(dayId);
        }

        public void SignDay(int dayId, long signTimeStamp)
        {
            if (IsSign(dayId))
            {
                Debugger.LogError($"第{dayId}天已经被签到");
                return;
            }

            Debugger.Log($"领取每日签到:{dayId}");
            signDayList.Add(dayId);
            lastSignTimeStamp = signTimeStamp;
        }
    }
}