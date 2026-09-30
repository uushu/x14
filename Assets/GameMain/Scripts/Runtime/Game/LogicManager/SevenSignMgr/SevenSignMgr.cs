using System.Collections;
using System.Collections.Generic;
using GameMain.RedDot;

namespace GameMain
{
    public class SevenSignMgr : ASingleBase<SevenSignMgr>
    {
        public List<ItemData_SevenSign> SignDataList => _signDataList;
        private readonly List<ItemData_SevenSign> _signDataList = new List<ItemData_SevenSign>();

        public override void Initialize()
        {
            base.Initialize();
            InitSevenSignData();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitSevenSignData()
        {
            _signDataList.Clear();
            var configList = ConfigMgr.Instance.Tables.DailySignCategory.DataList;
            for (int i = 0; i < configList.Count; i++)
            {
                _signDataList.Add(new ItemData_SevenSign(configList[i]));
            }

            CheckRedPoint();
        }

        /// <summary>
        /// 检测红点
        /// </summary>
        private void CheckRedPoint()
        {
            for (int i = 0; i < _signDataList.Count; i++)
            {
                var signItemStatus = _signDataList[i].State;
                if (signItemStatus == SevenSignState.CanGet || signItemStatus == SevenSignState.CanAdGet)
                {
                    RedDotMgr.Instance.ChangeValue(string.Format(RedDotPath.DailySign, _signDataList[i].DayId), 1);
                }
                else
                {
                    RedDotMgr.Instance.ChangeValue(string.Format(RedDotPath.DailySign, _signDataList[i].DayId), 0);
                }
            }
        }

        /// <summary>
        /// 获得签到数据
        /// </summary>
        public ItemData_SevenSign GetSignData(int signId)
        {
            for (int i = 0; i < _signDataList.Count; i++)
            {
                if (_signDataList[i].DayId == signId)
                {
                    return _signDataList[i];
                }
            }

            Debugger.LogError($"不存在的签到天数:{signId}");
            return null;
        }

        /// <summary>
        /// 尝试签到
        /// </summary>
        public bool TryGetSignDay(int signId)
        {
            var signData = GetSignData(signId);
            if (signData == null) return false;

            if (signData.State == SevenSignState.IsGet)
            {
                Debugger.Log($"签到已经领取:{signId}");
                return false;
            }

            if (signData.State == SevenSignState.NoGet)
            {
                Debugger.Log($"签到未完成:{signId}");
                return false;
            }

            //领取奖励
            signData.SaveSignDay();
            CheckRedPoint();
            return true;
        }
        
        /// <summary>
        /// 获得第一个可签到数据
        /// </summary>
        public ItemData_SevenSign TryGetFirstCanSign()
        {
            for (int i = 0; i < _signDataList.Count; i++)
            {
                if (_signDataList[i].State == SevenSignState.CanGet || _signDataList[i].State == SevenSignState.CanAdGet)
                {
                    return _signDataList[i];
                }
            }

            UIWindowTool.ShowPromptBox("没有可以领取的登录奖励");
            Debugger.Log("没有可以领取的登录奖励");
            return null;
        }
    }
}