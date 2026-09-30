using GravityEngine;
using GravitySDK.PC.Constant;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public class GravityEngineRecord : IRecordEvent
    {
        private class InitializeCallbackImpl : IInitializeCallback
        {
            public void onFailed(string errorMsg)
            {
                Debug.LogError($"初始化引力引擎失败 errorMsg:{errorMsg}");
                RecordEventDefine.SendRecordEventResult(false);
            }

            public void onSuccess(Dictionary<string, object> responseJson)
            {
                Debug.LogWarning($"初始化引力引擎成功");
                RecordEventDefine.SendRecordEventResult(true);
                // 建议在此执行一次Flush
                GravityEngineAPI.Flush();
            }
        }

        public void Initialize()
        {
            TryInitialize_GravityEngine();
        }

        public void UnInitialize()
        {
            GravityEngineAPI.Flush();
        }

        private void TryInitialize_GravityEngine()
        {
            if (string.IsNullOrEmpty(PlatformMgr.Instance.OpenId))
            {
                UniEvent.AddListener<PlatformEventDefine.GetOpenIdSuccess>(OnGetUniqueIdSuccess);
            }
            else
            {
                InitGravityEngine();
            }
        }

        private void OnGetUniqueIdSuccess(IEventMessage message)
        {
            if (message is PlatformEventDefine.GetOpenIdSuccess openId)
            {
                UniEvent.RemoveListener<PlatformEventDefine.GetOpenIdSuccess>(OnGetUniqueIdSuccess);
                InitGravityEngine();
            }
        }

        private void InitGravityEngine()
        {
            var openId = GameEntrance.Instance.platformMgr.OpenId;

            if (string.IsNullOrEmpty(openId))
            {
                Debug.LogError("初始化引力引擎失败 openId=NULL");
                return;
            }

            Debugger.LogWarning($"初始化引力引擎 openId={openId}");

            //开启引力引擎
            var go = new UnityEngine.GameObject(nameof(GravityEngineAPI), typeof(GravityEngineAPI));
            go.transform.parent = RecordMgr.Instance.gameObject.transform;
            go.transform.localPosition = UnityEngine.Vector3.zero;
            var gravityToken = RecordMgr.Instance.GetGravityEngineToken();

            if (string.IsNullOrEmpty(gravityToken))
            {
                Debugger.LogError("引力引擎token=null");
                return;
            }

            Debugger.LogWarning($"初始化引力引擎 gravityToken={gravityToken}");

            //  GravityEngineAPI.StartGravityEngine(gravityToken, openId, GravityEngineAPI.SDKRunMode.NORMAL);
#if UNITY_EDITOR
            GravityEngineAPI.StartGravityEngine(gravityToken, openId, GravityEngineAPI.SDKRunMode.DEBUG);
#else
            GravityEngineAPI.StartGravityEngine(gravityToken, openId, GravityEngineAPI.SDKRunMode.NORMAL);
#endif


#if UNITY_WECHAT_GAME

            GravityEngineAPI.EnableAutoTrack(AUTO_TRACK_EVENTS.WECHAT_GAME_ALL);

#elif UNITY_DOUYIN_GAME
            GravityEngineAPI.EnableAutoTrack(AUTO_TRACK_EVENTS.BYTEDANCE_GAME_ALL);
#endif

            GravityEngineAPI.Initialize(openId, openId, 1, openId, false,
                new InitializeCallbackImpl());
        }

        public void ReportEvent(string eventName, Dictionary<string, object> properties)
        {
            Debugger.LogWarning($"引力打点 API=ReportEvent(,) eventName={eventName}");
            GravityEngineAPI.Track(eventName, properties);
        }

        public void ReportEvent(string eventName)
        {
            Debugger.LogWarning($"引力打点 API=ReportEvent() eventName={eventName}");
            GravityEngineAPI.Track(eventName);
        }

        private void ReportEvent(ERecordUserProp recordProp, Dictionary<string, object> properties)
        {
            switch (recordProp)
            {
                case ERecordUserProp.UserSet:

                    GravityEngineAPI.UserSet(properties);

                    break;
                case ERecordUserProp.UserSetOnce:

                    GravityEngineAPI.UserSetOnce(properties);

                    break;
                case ERecordUserProp.UserAdd:

                    GravityEngineAPI.UserAdd(properties);

                    break;
                case ERecordUserProp.UserUnset:

                    var list = new List<string>();

                    foreach (var item in properties)
                    {
                        list.Add(item.Value as string);
                    }

                    GravityEngineAPI.UserUnset(list);

                    break;
                case ERecordUserProp.UserDelete:

                    GravityEngineAPI.UserDelete();

                    break;
                case ERecordUserProp.UserAppend:

                    GravityEngineAPI.UserAppend(properties);

                    break;
                case ERecordUserProp.UserUniqueAppend:

                    GravityEngineAPI.UserUniqAppend(properties);

                    break;
                case ERecordUserProp.UserNumberMax:

                    GravityEngineAPI.UserNumberMax(properties);

                    break;
                case ERecordUserProp.UserNumberMin:

                    GravityEngineAPI.UserNumberMin(properties);

                    break;
            }
        }

        public void ReportEvent(ERecordUserProp recordProp, string key, object value)
        {
            Debugger.LogWarning($"引力打点 recordProp={recordProp} key={key} value={value}");
            ReportEvent(recordProp, new Dictionary<string, object>() { { key, value } });
        }

        public void ReportAdShowEvent(string adType, string adUnitId, Dictionary<string, object> properties)
        {
            Debugger.LogWarning($"引力打点 adType={adType} adUnitId={adUnitId}");
            GravityEngineAPI.TrackWechatAdShowEvent(adType, adUnitId, properties);
        }
    }
}