using System.Collections;
using System.Collections.Generic;
using cfg;

namespace GameMain
{
    [System.Serializable]
    public class Save_Common : ASaveData
    {
        public bool isFirstEnter;
        
        //第一次登录游戏时间
        public long userFirstEnterGameTimeStamp;

        public long latestOnlineTimeStamp;
        
        public bool isShowSubscribeWindow;
        
        public bool isGetSubscribeAward;
        /// <summary>
        /// 抖音 微信的appid
        /// </summary>
        public string AppId;
        public string OpenId;

        public override void SetDefaultData(GlobalConfigCategory category)
        {
            AppId= PlatformMgr.Instance.GetPlatformAppId();
            OpenId=PlatformMgr.Instance.OpenId;
        }

        public override void OnAfterDeserialize()
        {
            if (string.IsNullOrEmpty(AppId))
            {
                AppId= PlatformMgr.Instance.GetPlatformAppId();
            }
            if (string.IsNullOrEmpty(OpenId))
            {
                OpenId= PlatformMgr.Instance.OpenId;
            }
        }

        public void SetNoFirstEnter()
        {
            isFirstEnter = false;
        }
    }
}