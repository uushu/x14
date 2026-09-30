using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameMain
{
    public class RecordMgr : AMonoManager<RecordMgr>
    {
        [ValueDropdown("GetSdkList")] [LabelText("sdk类型")]
        public string sdkFullName;

        private IRecordEvent _eventSdk;

        public DefaultRecord AsDefaultSdk => _eventSdk as DefaultRecord;

        public GravityEngineRecord AsGravityRecord => _eventSdk as GravityEngineRecord;

        [ShowIf("IsGravityEngine")] [LabelText("微信token")]
        public string gravityTokenWx;

        [ShowIf("IsGravityEngine")] [LabelText("抖音token")]
        public string gravityTokenDy;

        public override int Priority => 997;

        private StringBuilder _sb = new();

        protected override void OnEnter()
        {
            _eventSdk = NewRecordImpl();
            _eventSdk?.Initialize();
        }

        public string GetGravityEngineToken()
        {
#if UNITY_DOUYIN_GAME
            return gravityTokenDy;

#elif UNITY_WECHAT_GAME
            return gravityTokenWx;
#else
            return null;
#endif
        }

        protected override void OnExit()
        {
            base.OnExit();

            _eventSdk?.UnInitialize();
            _eventSdk = null;
        }

        private IRecordEvent NewRecordImpl()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return new DefaultRecord();
            }

            if (sdkFullName.Equals(typeof(DefaultRecord).FullName))
            {
                return new DefaultRecord();
            }
            else if (sdkFullName.Equals(typeof(GravityEngineRecord).FullName))
            {
                return new GravityEngineRecord();
            }
            else
            {
                Debugger.LogError($"未处理的事件上报 sdkFullName = {sdkFullName}");
                return null;
            }
        }

        /// <summary>
        /// 广告打点
        /// </summary>
        public void ReportRewardLookAd(LookAdsType lookAd,bool result, Dictionary<string, object> properties)
        {
            if (result)
            {
                Debugger.LogWarning($"引力打点 玩家看广告= {result}");
                _sb.Clear();
                _sb.Append($"事件名:广告-{lookAd}, 事件属性: ");
                foreach (var item in properties)
                {
                    _sb.Append($"{item.Key}={item.Value},");
                }

                Debugger.LogWarning($"{RecordEvent.DebugPrefix} {_sb.ToString()}");

                string adUnitId = PlatformMgr.Instance.GetPlatformAdId();
                _eventSdk.ReportAdShowEvent("reward", adUnitId, properties);
            }
        }

        #region 自定义事件

        /// <summary>
        /// 关卡开始
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <param name="levelId">关卡Id</param>
        public void ReportLevelStart(string gameType, int levelId)
        {
            DataCenter saveData = LocalSaveMgr.Instance.DataCenter;
            ReportEvent(RecordEvent.event_meta_level_start, new Dictionary<string, object>()
            {
                { RecordEvent.event_name_isFirstDayUser,saveData.GetLocalSave<Save_Setting>().IsFirstDayUser()},
                { RecordEvent.event_name_level_id, levelId },
                { RecordEvent.event_name_game_type, gameType},
                { RecordEvent.event_name_date_time, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
            });
        }
        /// <summary>
        /// 关卡成功
        /// </summary>
        /// <param name="levelId"></param>
        /// <param name="gameType"></param>
        /// <param name="gameTime"></param>
        public void ReportLevelSuccess(string gameType,int levelId, float gameTime)
        {
            DataCenter saveData = LocalSaveMgr.Instance.DataCenter;
            ReportEvent(RecordEvent.event_meta_level_success, new Dictionary<string, object>()
            {
                { RecordEvent.event_name_isFirstDayUser,saveData.GetLocalSave<Save_Setting>().IsFirstDayUser()},
                { RecordEvent.event_name_level_id, levelId },
                { RecordEvent.event_name_game_type, gameType},
                { RecordEvent.event_name_time, gameTime}
            });
        }
        /// <summary>
        /// 关卡失败
        /// </summary>
        /// <param name="levelId"></param>
        /// <param name="gameType"></param>
        /// <param name="progress"></param>
        /// <param name="gameTime"></param>
        public void ReportLevelFailed(string gameType,int levelId,float gameTime,float score = 0)
        {
            DataCenter saveData = LocalSaveMgr.Instance.DataCenter;
            ReportEvent(RecordEvent.event_meta_level_failed, new Dictionary<string, object>()
            {
                { RecordEvent.event_name_isFirstDayUser,saveData.GetLocalSave<Save_Setting>().IsFirstDayUser()},
                { RecordEvent.event_name_level_id, levelId },
                { RecordEvent.event_name_game_type,gameType },
                { RecordEvent.event_name_time, gameTime },
                { RecordEvent.event_name_score, score }
            });
        }
        /// <summary>
        /// 关卡复活
        /// </summary>
        /// <param name="levelId"></param>
        /// <param name="gameType"></param>
        /// <param name="progress"></param>
        /// <param name="gameTime"></param>
        public void ReportLevelRelive(string gameType,int levelId, float gameTime,float score = 0)
        {
            DataCenter saveData = LocalSaveMgr.Instance.DataCenter;
            ReportEvent(RecordEvent.event_meta_level_relive, new Dictionary<string, object>()
            {
                { RecordEvent.event_name_isFirstDayUser,saveData.GetLocalSave<Save_Setting>().IsFirstDayUser()},
                { RecordEvent.event_name_level_id, levelId },
                { RecordEvent.event_name_game_type, gameType },
                { RecordEvent.event_name_time, gameTime },
                { RecordEvent.event_name_score, score }
            });
        }
        
        /// <summary>
        /// 关卡重玩
        /// </summary>
        /// <param name="levelId"></param>
        /// <param name="gameType"></param>
        /// <param name="progress"></param>
        /// <param name="gameTime"></param>
        public void ReportLevelRestart(string gameType,int levelId, float gameTime)
        {
            DataCenter saveData = LocalSaveMgr.Instance.DataCenter;
            ReportEvent(RecordEvent.event_meta_level_restart, new Dictionary<string, object>()
            {
                { RecordEvent.event_name_isFirstDayUser,saveData.GetLocalSave<Save_Setting>().IsFirstDayUser()},
                { RecordEvent.event_name_level_id, levelId },
                { RecordEvent.event_name_game_type, gameType },
                { RecordEvent.event_name_time, gameTime }
            });
        }
     
        /// <summary>
        /// 购买道具
        /// </summary>
        public void ReportBuyItem(string itemTag)
        {
            ReportEvent(RecordEvent.event_meta_buy_Prop, new Dictionary<string, object>()
            {
                { RecordEvent.event_name_propTag, itemTag},
            });
        }
        #endregion

        #region 未使用
        
        /// <summary>
        /// 使用道具
        /// </summary>
        /// <param name="levelId"></param>
        /// <param name="gameType"></param>
        /// <param name="propId"></param>
        /// <param name="progress"></param>
        /// <param name="gameTime"></param>
        public void ReportUseProp(int levelId, int gameType, int propId, float progress, float gameTime)
        {
            ReportEvent(RecordEvent.event_meta_use_prop, new Dictionary<string, object>()
            {
                { RecordEvent.event_name_level_id, levelId },
                { RecordEvent.event_name_game_type, (int)gameType },
                { RecordEvent.event_name_propId, propId },
                { RecordEvent.event_name_progress, progress },
                { RecordEvent.event_name_time, gameTime }
            });
        }
        
        public void ReportBackHall(int levelId, int gameType)
        {
            ReportEvent(RecordEvent.event_meta_back_hall, new Dictionary<string, object>()
            {
                { RecordEvent.event_name_level_id, levelId },
                { RecordEvent.event_name_game_type, (int)gameType },
            });
        }
        
        public void ReportStartLoadScene(string sceneName)
        {
            var timeStamp = TimeTool.GetNowTimeStamp();
            ReportEvent(RecordEvent.event_meta_scene_load_start, new Dictionary<string, object>()
            {
                { RecordEvent.event_name_scene_name, sceneName },
                { RecordEvent.event_name_date_time, DateTime.Now },
                { RecordEvent.event_name_timestamp, timeStamp },
            });
        }

        public void ReportStartLoadComplete(string sceneName)
        {
            var timeStamp = TimeTool.GetNowTimeStamp();
            ReportEvent(RecordEvent.event_meta_scene_load_complete, new Dictionary<string, object>()
            {
                { RecordEvent.event_name_scene_name, sceneName },
                { RecordEvent.event_name_date_time, DateTime.Now },
                { RecordEvent.event_name_timestamp, timeStamp },
            });
        }

        public void ReportLoadSceneUseTime(string sceneName, float time)
        {
            ReportEvent(RecordEvent.event_meta_scene_load_use_time, new Dictionary<string, object>()
            {
                { RecordEvent.event_name_scene_name, sceneName },
                { RecordEvent.event_name_time, time },
            });
        }

        public void ReportShareGame()
        {
            ReportEvent(RecordEvent.event_meta_share_game, new Dictionary<string, object>()
            {
            });
        }

        #endregion

        /// <summary>
        /// 通用事件打点总入口
        /// </summary>
        public void ReportEvent(string eventName, Dictionary<string, object> properties)
        {
            _sb.Clear();
            _sb.Append($"事件名:{eventName}, 事件属性: ");
            foreach (var item in properties)
            {
                _sb.Append($"{item.Key}={item.Value},");
            }

            Debugger.LogWarning($"{RecordEvent.DebugPrefix} {_sb.ToString()}");
            _eventSdk.ReportEvent(eventName, properties);
        }

#if UNITY_EDITOR

        private IEnumerable GetSdkList()
        {
            return new ValueDropdownList<string>()
            {
                { "默认", typeof(DefaultRecord).FullName },
                { "引力引擎", typeof(GravityEngineRecord).FullName },
            };
        }

        protected bool IsGravityEngine => sdkFullName.Equals(typeof(GravityEngineRecord).FullName);
#endif
    }
}