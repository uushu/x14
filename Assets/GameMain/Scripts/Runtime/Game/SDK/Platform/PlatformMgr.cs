using GameMain.AOT;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameMain.Garden;
using UnityEngine;

namespace GameMain
{
    public enum EPlatform
    {
        [LabelText("默认")] Default,
        [LabelText("微信")] WeiXin,
        [LabelText("抖音")] DouYin,
    }

    public class PlatformMgr : AMonoManager<PlatformMgr>, IPlatform 
    {
        [LabelText("平台")] [OnValueChanged("OnPlatformChange")]
        public EPlatform ePlatform;

        [LabelText("广告-微信")] [ShowIf("IsWx")] public string rewardAdIdWx;

        [LabelText("App-微信")] [ShowIf("IsWx")] public string appIdWx;

        [LabelText("游戏标识-微信")] [ShowIf("IsWx")]
        public string gameTagWx;

        [LabelText("广告-抖音")] [ShowIf("IsDy")] public string rewardAdIdDy;

        [LabelText("App-抖音")] [ShowIf("IsDy")] public string appIdDy;

        [LabelText("game标识-抖音")] [ShowIf("IsDy")]
        public string gameTagDy;
        
        public string GetPlatformAdId()
        {
#if UNITY_DOUYIN_GAME
            return rewardAdIdDy;
#elif UNITY_WECHAT_GAME
            return rewardAdIdWx;
#else
            return null;
#endif
        }
        public string GetPlatformAppId()
        {
#if UNITY_DOUYIN_GAME
            return appIdDy;
#elif UNITY_WECHAT_GAME
            return appIdWx;
#else
            return null;
#endif
        }

        private IPlatform _platform;
        
        public IPlatform Platform => _platform;
        /// <summary>
        /// 用户唯一ID
        /// </summary>
        public string OpenId
        {
            get => _platform?.OpenId;
            set => _platform.OpenId = value;
        }

        [LabelText("跳过看广告")] 
        public bool skipLookAd;

        [LabelText("显示猫咪编号")]
        public bool isShowCatNum;
        
        [LabelText("显示游戏路径")]
        public bool isShowGamePath;
        
        /// <summary>
        /// 小程序进入
        /// </summary>
        public static bool IsMiniGameEntrance;

        protected override void OnEnter()
        {
            base.OnEnter();
            _platform = NewPlatform();
            _platform.Initialize();
            Debugger.LogWarning($"初始化平台成功:{_platform.GetType()}");
            switch (ePlatform)
            {
                case EPlatform.WeiXin:
#if !UNITY_EDITOR && PP_WX
                    SystemInfo systemInfo = WX.GetSystemInfoSync();
                    Debug.Log($"系统：{systemInfo?.platform}");
                    if (systemInfo != null && systemInfo.platform == "ios")
                    {
                        Debug.Log("苹果设置成24帧");
                        Application.targetFrameRate = 24;
                        WX.SetPreferredFramesPerSecond(24);
                    }
#endif
                    break;
                case EPlatform.DouYin:
                    Application.targetFrameRate = 24;
                    break;
                case EPlatform.Default:
                    Application.targetFrameRate = 45;
                    break;
            }
        }

        protected override void OnExit()
        {
            base.OnExit();
            Debugger.LogWarning($"反初始化平台成功:{_platform.GetType()}");
            _platform?.UnInitialize();
            _platform = null;
        }

        private IPlatform NewPlatform()
        {
#if UNITY_EDITOR
            return new PlatformDefault();
#elif UNITY_DOUYIN_GAME
            return new PlatformDy();
#elif UNITY_WECHAT_GAME
            return new PlatformWx();
#else
            return new PlatformDefault();
#endif
        }
        
        public void GameManagerInited()
        {
            _platform.GameManagerInited();
        }

        /// <summary>
        /// 播放广告
        /// </summary>
        public void LookAd(Action<bool> result, LookAdsType lookAdType)
        {
            if (skipLookAd)
            {
                GameEventDefine.SendAddTaskProgress(TaskT6.TaskTypeId, 1);
                result?.Invoke(true);
            }
            else
            {
                _platform.LookAd((lookAdResult) =>
                {
                    result?.Invoke(lookAdResult);

                    if (lookAdResult)
                    {
                        GameEventDefine.SendAddTaskProgress(TaskT6.TaskTypeId, 1);
                    }
                    else
                    {
                        UIWindowTool.ShowPromptBox("未看完广告，无法获取奖励");
                    }

                    DataCenter saveData = LocalSaveMgr.Instance.DataCenter;
                    string gameTag = $"{Init.Instance.miniGameType}_未知模式";
                    //普通模式
                    bool isGardenWindow = UIWindowMgr.Instance.WindowIsOpen(nameof(GardenWindow));
                    if (isGardenWindow)
                    {
                        gameTag = $"{Init.Instance.miniGameType}_普通模式";
                    }
                    //无尽模式
                    bool isGardenEndlessWindow = UIWindowMgr.Instance.WindowIsOpen(nameof(GardenEndlessWindow));
                    if (isGardenEndlessWindow)
                    {
                        gameTag = $"{Init.Instance.miniGameType}_无尽模式";
                    }
                    
                    //数据
                    Dictionary<string, object> properties = new Dictionary<string, object>()
                    {
                        { RecordEvent.event_name_isFirstDayUser,saveData.GetLocalSave<Save_Setting>().IsFirstDayUser()},
                        { RecordEvent.event_name_level_id,saveData.GetLocalSave<Save_FlowerGarden>().levelId},
                        { RecordEvent.event_name_game_type,gameTag},
                        { RecordEvent.event_name_date_time, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
                    };
                    RecordMgr.Instance.ReportRewardLookAd(lookAdType,true,properties);
                    ReportManager.Instance.WatchAd(lookAdResult, lookAdType.ToString(),null,null);
                });
            }
        }

        void IPlatform.LookAd(Action<bool> result)
        {
        }

        /// <summary>
        /// 震动 heavy、medium、light
        /// </summary>
        public void Shake(string type = "medium")
        {
            //heavy、medium、light
            if (LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Setting>().shakeIsOn == false) return;
            _platform.Shake(type);
        }
        /// <summary>
        /// 分享
        /// </summary>
        public void ShowShare(string str = "",Action<bool> result = null)
        {
            _platform?.ShowShare(str,(success) =>
            {
                if (success)
                {
                    GameEventDefine.SendAddTaskProgress(TaskT2.TaskTypeId, 1);
                    GameEventDefine.SendGameShare(1);
                }
                result?.Invoke(success);
            });
        }
        /// <summary>
        /// 定向邀请
        /// </summary>
        public void DirectedShare(int posType,Action<bool> result)
        {
            _platform.DirectedShare(posType,(resultB)=>
            {
                result?.Invoke(resultB);
                if (resultB)
                {
                    GameEventDefine.SendAddTaskProgress(TaskT2.TaskTypeId, 1);
                    GameEventDefine.SendGameShare(1);
                }
            });
        }

        public void AddToMiniGameEntrance(Action<bool> result)
        {
            _platform.AddToMiniGameEntrance(result);
        }

        /// <summary>
        /// 创建桌面快捷方式
        /// </summary>
        public void CreateDesktopEnterAppIcon()
        {
            _platform.CreateDesktopEnterAppIcon();
        }

        /// <summary>
        /// 检测是否有桌面快捷方式
        /// </summary>
        public void CheckHaveDesktopAppIcon(Action<bool> callBack)
        {
            _platform?.CheckHaveDesktopAppIcon(callBack);
        }

        /// <summary>
        /// 获得屏幕大小
        /// </summary>
        public void GetCanvasSize(Action<bool, Rect> callBack)
        {
            _platform.GetCanvasSize(callBack);
        }

        /// <summary>
        /// 获得用户信息
        /// </summary>
        /// <param name="forceLogin"></param>
        /// <param name="result"></param>
        /// <param name="noAuthorityIsCreateBtn"></param>
        /// <param name="showBtn"></param>
        /// <param name="btnPos"></param>
        /// <param name="isLimitBtnCallback"></param>
        public void GetUserInfo(bool forceLogin, Action<bool, string, string> result, bool noAuthorityIsCreateBtn,
            Action showBtn, Rect btnPos, Action<bool> isLimitBtnCallback)
        {
            _platform.GetUserInfo(forceLogin, result, noAuthorityIsCreateBtn, showBtn, btnPos, isLimitBtnCallback);
        }

        public void HideGetUserBtn()
        {
            _platform.HideGetUserBtn();
        }

        /// <summary>
        /// 获得刘海高
        /// </summary>
        public void GetBangsHeight(Action<bool, float> result)
        {
            _platform.GetBangsHeight(result);
        }

        /// <summary>
        /// 添加生命周期回调
        /// </summary>
        public void AddLifeCycleEvent(System.Action hideCallback, System.Action showCallback)
        {
            _platform.AddLifeCycleEvent(hideCallback, showCallback);
        }

        /// <summary>
        /// 获得游戏圈
        /// </summary>
        public void GetGameCircle(RectTransform rectTrans, Vector2 pos, Vector2 canvasRect, Action<object> result)
        {
        }

        /// <summary>
        /// 显示游戏圈
        /// </summary>
        public void ShowGameCircleBtn()
        {
        }

        /// <summary>
        /// 隐藏游戏圈 
        /// </summary>
        public void HideGameCircleBtn()
        {
        }

        /// <summary>
        /// 设置私密分享信息 
        /// </summary>
        public async Task<string> SetPrivacyShareMessage(Action<bool> callBack)
        {
            return "";
        }

        /// <summary>
        /// 标记游戏开始运行
        /// </summary>
        public void TagGameIsRunning()
        {
            _platform.TagGameIsRunning();
        }
        /// <summary>
        /// 复制到剪切板
        /// </summary>
        public void GetCopyToClipboard(string str, Action<bool> result)
        {
            _platform.GetCopyToClipboard(str, result);
        }

        #region 登录相关

        private int loginCount;
        public static bool IsLoginSuccess;
        private static string userId;
        private static string defaultName = "未知用户";
        private static string defaultIconUrl = "";
        private static string nickName;
        private static string iconUrl;
        private static string channelStrID;
        private static string version;

        /// <summary>
        /// 用户显示短ID
        /// </summary>
        public static string UserId => userId;
        /// <summary>
        /// 设置显示短ID
        /// </summary>
        public void SetUserData(string _userId)
        {
            userId = _userId;
        }
        public static string DefaultName => defaultName + userId?.Substring(0, 4);
        public static string DefaultIconUrl => defaultIconUrl;

        /// <summary>
        /// 用户昵称
        /// </summary>
        public static string NickName
        {
            get => nickName;
            set => nickName = value;
        }

        /// <summary>
        /// 用户头像地址
        /// </summary>
        public static string IconUrl
        {
            get => iconUrl;
            set => iconUrl = value;
        }
        /// <summary>
        /// 渠道stringID
        /// </summary>
        public static string ChannelStrID => channelStrID;
        /// <summary>
        /// 版本
        /// </summary>
        public static string Version => version;
        
        private readonly EventGroup eventGroup = new EventGroup();
        public void Login(string platformTag, Action<bool, Action> resultAction)
        {
            channelStrID = platformTag;
            version = Application.version;
#if UNITY_EDITOR
            userId = SystemInfo.deviceUniqueIdentifier;
#endif
            IsLoginSuccess = true;
            eventGroup.AddListener<PlatformEventDefine.GetOpenIdSuccess>(OnGetOpenIdSuccess);
            resultAction?.Invoke(true, null);
            return;

#if !UNITY_EDITOR
            if (ePlatform == EPlatform.Default)
            {
                userId = SystemInfo.deviceUniqueIdentifier;
                IsLoginSuccess = true;
                resultAction?.Invoke(true,null);
                ReportManager.Instance.StartLoad();
                //已有玩家信息，开始加载
                Init.Instance.AutoClickStartBtn();
                NickName = DefaultName;
                IconUrl = defaultIconUrl;
                return;
            }
#endif
        }
        
        /// <summary>
        /// 获得唯一ID成功
        /// </summary>
        private void OnGetOpenIdSuccess(IEventMessage data)
        {
            if (data is PlatformEventDefine.GetOpenIdSuccess)
            {
                userId = OpenId.GetHashCode().ToString();
                ReportManager.Instance.StartLoad();
                //已有玩家信息，开始加载
                Init.Instance.AutoClickStartBtn();
                //获取用户信息
                GetUserInfo(true, (result, _nickName, _iconUrl) =>
                {
                    Debug.Log($"获取用户信息:{result}昵称nick:{_nickName},头像url:{_iconUrl}");
                    if (result)
                    {
                        NickName = _nickName;
                        IconUrl = _iconUrl;
                    }
                    else
                    {
                        NickName = DefaultName;
                        IconUrl = defaultIconUrl;
                    }
                }, false, null, Rect.zero, null);
            }
        }
        /// <summary>
        /// 打点
        /// </summary>
        public void Report(string eventName, Dictionary<string,string> data)
        {
           _platform.Report(eventName,data);
        }
        
        #endregion

#if UNITY_EDITOR
        private bool IsWx => ePlatform == EPlatform.WeiXin;

        private bool IsDy => ePlatform == EPlatform.DouYin;

        private void OnPlatformChange()
        {
            //获取当前是哪个平台
            UnityEditor.BuildTargetGroup buildTargetGroup =
                UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup;

            //获得当前平台已有的宏定义
            var symbols = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            //另外加一个SDK宏
            symbols = symbols.Replace("UNITY_WECHAT_GAME", "");
            symbols = symbols.Replace("UNITY_DOUYIN_GAME", "");
            symbols = symbols.Replace("UNITASK_DOTWEEN_SUPPORT", "");
            symbols += ";UNITASK_DOTWEEN_SUPPORT";

            switch (ePlatform)
            {
                case EPlatform.Default:
                    break;

                case EPlatform.WeiXin:

                    //另外加一个SDK宏
                    symbols += ";UNITY_WECHAT_GAME";
                    symbols += ";PF_WX";
                    break;

                case EPlatform.DouYin:

                    symbols += ";UNITY_DOUYIN_GAME";
                    symbols += ";PF_TT";
                    break;
            }

            //重新设置宏
            UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbols);
        }
#endif
        
    }
}