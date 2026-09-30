using System;
using System.Collections.Generic;
using System.Linq;
using cfg;

namespace GameMain
{
    public class ItemData_SevenSign
    {
        public int DayId => Config.Id;
        public string DayString => $"第{Config.Id}天";
        public DailySignConfig Config { get; private set; }

        public SevenSignState State
        {
            get
            {
                Save_Sign saveSign = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Sign>(false);
                //已经签到
                var isSign = saveSign.IsSign(DayId);
                if (isSign) return SevenSignState.IsGet;

                //当前登录天数
                long currentSignDay = saveSign.signDay;
                //今天是否可以获得
                if (currentSignDay >= DayId)
                {
                   return SevenSignState.CanGet;
                }
                
                // //可以广告补签
                // if (currentSignDay > DayId)
                // {
                //     return SevenSignState.CanAdGet;
                // }
                return SevenSignState.NoGet;
            }
        }

        private List<ItemData_BagProp> awardList;

        public List<ItemData_BagProp> AwardListList
        {
            get
            {
                if (awardList == null)
                {
                    awardList = new List<ItemData_BagProp>();

                    for (int i = 0; i < Config.Award.Count; i++)
                    {
                        KeyValuePair<int, int> temp = Config.Award.ElementAt(i);
                        awardList.Add(new ItemData_BagProp(temp.Key, temp.Value));
                    }
                }

                return awardList;
            }
        }

        public ItemData_SevenSign(DailySignConfig config)
        {
            Config = config;
        }

        public void SaveSignDay()
        {
            LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Sign>().SignDay(Config.Id, TimeTool.GetNowTimeStamp());
        }
    }

    public enum SevenSignState
    {
        NoGet,
        CanAdGet,
        CanGet,
        IsGet,
    }
}