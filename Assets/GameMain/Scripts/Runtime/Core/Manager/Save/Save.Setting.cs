using System;
using System.Collections;
using System.Collections.Generic;
using cfg;
using UnityEngine;

namespace GameMain
{
    [System.Serializable]
    public class Save_Setting : ASaveData
    {
        public bool soundIsOn;

        public bool musicIsOn;

        public bool shakeIsOn;

        //第一次进入游戏 直接进入战斗
        public bool firstEnterGameFlag;
        //进入游戏时间标签
        public string enterGameTimeTag;
        /// <summary>
        /// 广告用户
        /// </summary>
        public bool isAdsUser;
        /// <summary>
        /// 抖音广告进入用户数据
        /// </summary>
        [SerializeField]
        public DouYinAdsInfo adsInfo;
        
        public override void SetDefaultData(GlobalConfigCategory category)
        {
            soundIsOn = true;
            musicIsOn = true;
            shakeIsOn = true;
            firstEnterGameFlag = true;
        }

        /// <summary>
        /// 是否是首日用户
        /// </summary>
        public bool IsFirstDayUser()
        {
            string temp = CommonTool.GetTodayTag();
            return temp.Equals(enterGameTimeTag);
        }
    }
    
    /// <summary>
    /// 抖音启动广告信息
    /// </summary>
    [Serializable]
    public class DouYinAdsInfo
    {
        public long cid;
        public long ad_id;
        public DouYinLogExtra log_extra;
        public string web_url;
    }
    /// <summary>
    /// 额外信息
    /// </summary>
    [Serializable]
    public class DouYinLogExtra
    {
        public string content_type;
        public string ad_item_id;
        public string clickid;
        public string advertiser_id;
        public string bdid;
        public string req_id;
        public int price;
        public string ad_price;
        public long ad_id;
    }
}