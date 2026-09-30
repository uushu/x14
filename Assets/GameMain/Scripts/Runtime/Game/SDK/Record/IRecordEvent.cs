using System.Collections.Generic;

namespace GameMain
{
    public enum ERecordUserProp
    {
        UserSet,
        UserSetOnce,
        UserAdd,
        UserUnset,
        UserDelete,
        UserAppend,
        UserUniqueAppend,
        UserNumberMax,
        UserNumberMin,
    }
    
    public interface IRecordEvent
    {
        void Initialize();

        void ReportEvent(string eventName);

        void ReportEvent(string eventName, Dictionary<string, object> properties);

        void ReportEvent(ERecordUserProp recordProp, string key, object value);

        /// <summary>
        /// 上报小游戏、快游戏广告观看事件 AdShow
        /// </summary>
        /// <param name="adType"></param>               广告类型 取值为：reward、banner、native、interstitial、video_feed、video_begin，分别对应：激励视频广告、Banner广告、原生模板广告、插屏广告、视频广告、视频贴片广告
        /// <param name="adUnitId"></param>             广告位ID（一般以adunit开头，注意不要填错，会导致广告收入统计不准！）
        /// <param name="otherProperties"></param>      其他需要携带的自定义参数
        void ReportAdShowEvent(string adType, string adUnitId, Dictionary<string, object> otherProperties);

        void UnInitialize();
    }
}