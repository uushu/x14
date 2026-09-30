using System;
using Cysharp.Threading.Tasks;
using GameMain.AOT;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using GameMain.Water;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameMain
{
    public class GameEntrance : MonoBehaviour
    {
        public static GameEntrance Instance { get; private set; }

        [LabelText("打印日志")] 
        public Debugger.ELogLevel logLevel;

        [LabelText("管理器-资源更新之前")] 
        private List<IManager> _managersBefore = new();

        [LabelText("管理器-资源更新之后")] 
        private List<IManager> _managerAfter = new();

        [LabelText("资源管理器")]
        public ResourceMgr resourceMgr;

        [LabelText("配置表")] 
        public ConfigMgr configMgr;

        [LabelText("本地保存")] 
        public LocalSaveMgr localSaveMgr;

        [LabelText("音效")] 
        public AudioMgr audioMgr;

        [LabelText("定时器")]
        public TimerMgr timerMgr;

        [LabelText("UI窗口管理")] 
        public UIWindowMgr uiMgr;

        [LabelText("平台管理器")] 
        public PlatformMgr platformMgr;

        [LabelText("对象池")]
        public PoolsMgr poolMgr;

        [LabelText("逻辑管理器")]
        public GamePlayMgr gamePlayMgr;

        [LabelText("打点管理器")]
        public ReportManager reportManager;

        [LabelText("打点管理器")]
        public RecordMgr recordMgr;
        
        [LabelText("开启GM")]
        public bool openGmWindow;

        [LabelText("开启新手引导")] 
        public bool openGuide;

        private void Awake()
        {
            Instance = this;
            Debugger.logLevel = logLevel;
            DontDestroyOnLoad(gameObject);
        }

        private void InitializeBeforeManagers()
        {
            foreach (var manager in _managersBefore)
            {
                Debugger.Log($"Enter Before ResUpdate --- {manager}", Debugger.ELogColor.Orange);
                manager.Initialize();
            }
            
        }

        public void InitializeAfterManagers()
        {
            foreach (var manager in _managerAfter)
            {
                Debugger.Log($"Enter After ResUpdate --- {manager}", Debugger.ELogColor.Orange);
                manager.Initialize();
            }
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            foreach (var manager in _managerAfter)
            {
                manager.OnUpdate(deltaTime);
            }
        }

        private void OnDestroy()
        {
            foreach (var manager in _managerAfter)
            {
                manager.UnInitialize();
            }

            foreach (var manager in _managersBefore)
            {
                manager.UnInitialize();
            }
        }

        private void SortManagers(List<IManager> managers)
        {
            managers.Sort((a, b) =>
            {
                if (a.Priority < b.Priority)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            });
        }

        public void RegisterAfter(IManager manager)
        {
            if (_managerAfter.Contains(manager))
            {
                Debugger.LogError($"重复注册管理器:{manager.GetType().Name}");
                return;
            }

            Debugger.Log($"注册管理器:{manager.GetType()}");
            _managerAfter.Add(manager);
            manager.Initialize();
        }

        public void UnRegisterAfter(IManager manager)
        {
            if (_managerAfter.Contains(manager) == false) return;

            _managerAfter.Remove(manager);
            manager.UnInitialize();

            Debugger.Log($"移除管理器:{manager.GetType()}");
        }

        public async void Initialize()
        {
            //不等热更新资源加载完优先初始化的管理器
            _managersBefore = new List<IManager>()
            {
                platformMgr,
                recordMgr
            };
            SortManagers(_managersBefore);
            InitializeBeforeManagers();

            await ConfigMgr.Instance.LoadAllConfigs();

            _managerAfter = new List<IManager>()
            {
                resourceMgr,
                configMgr,
                localSaveMgr,
                timerMgr,
                audioMgr,
                uiMgr,
                poolMgr,
                gamePlayMgr,
                reportManager,
            };
            SortManagers(_managerAfter);
            InitializeAfterManagers();
            //游戏可交互
            PlatformMgr.Instance.TagGameIsRunning();
            //预加载资源
            var preloadList = new List<string>() {
               nameof(HallWindow),
            };

            for (int i = 0; i < preloadList.Count; i++)
            {
                UserEventDefine.UserProgressUpdateValue.SendEventMessage(0.8f + 0.1f / preloadList.Count * i);
                await ResourceMgr.Instance.LoadAssetAsync<Object>(preloadList[i]);
            }
            Save_Common temp = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Common>();
            UserEventDefine.UserProgressUpdateValue.SendEventMessage(0.9f);
            LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_CostPower>();
            Save_Sign saveSign = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Sign>(true);
            saveSign.RecordSignDay();

            await ResourceMgr.Instance.LoadSceneAsync(GameConstant.SceneHall);

            if (LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Setting>().firstEnterGameFlag)
            {
                var localSave = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Setting>(true);
                localSave.firstEnterGameFlag = false;
                localSave.enterGameTimeTag = CommonTool.GetTodayTag();
                
                await UIWindowTool.FirstEnterGame();
            }
            else
            {
                await UIWindowTool.ChangeToHallScene(false);
            }
            
            UserEventDefine.UserProgressUpdateValue.SendEventMessage(1f);

            ReportManager.Instance.EndLoad();
        }
        
        /// <summary>
        /// 是否显示 GM
        /// </summary>
        public static bool IsShowGm(string userId)
        {
            if (GameEntrance.Instance.openGmWindow) return true;
            if (string.IsNullOrEmpty(userId)) return false;
            
            var dataList = ConfigMgr.Instance.Tables.GmBlackListCategory.DataList;
            foreach (var data in dataList)
            {
                if (data.UserId.Equals(userId))
                { 
                    return true;
                }
            }
            return false;
        }

        #region 提交排行榜

        public static string RoleRankName = "RoleRankName2";
        public static string SupperLevelRankName = "SupperLevelRankName2";
        public static string EndlessBestRankName = "EndlessBestRankName2";
        public static string AchievementRankName = "AchievementRankName2";
        /// <summary>
        /// 提交积分排行榜
        /// </summary>
        public static void UpdateRoleProgressRank(int score)
        {
             HyzRankRoleInfo temp = new HyzRankRoleInfo()
             {
                 uuid = PlatformMgr.Instance.OpenId,
                 name = PlatformMgr.NickName,  
                 iconUrl = PlatformMgr.IconUrl,
                 level = score,
             };
             HyzRankManager.Instance.UpLoadRank(RoleRankName, temp);
        }
        
        /// <summary>
        /// 提交玩家等级排行榜
        /// </summary>
        public static void UpdateRoleLevelRank(int levelId)
        {
            HyzRankRoleInfo temp = new HyzRankRoleInfo()
            {
                uuid = PlatformMgr.Instance.OpenId,
                name = PlatformMgr.NickName,
                iconUrl = PlatformMgr.IconUrl,
                titleId = levelId,
                level = levelId,
            };
            HyzRankManager.Instance.UpLoadRank(SupperLevelRankName, temp);
        }
        /// <summary>
        /// 提交玩家无尽模式积分
        /// </summary>
        public static void UpdateRoleEndlessBestRank(int Points,int titleId)
        {
            HyzRankRoleInfo temp = new HyzRankRoleInfo()
            {
                uuid = PlatformMgr.Instance.OpenId,
                name = PlatformMgr.NickName,
                iconUrl = PlatformMgr.IconUrl,
                titleId = titleId,
                level = Points,
            };
            HyzRankManager.Instance.UpLoadRank(EndlessBestRankName, temp);
        }
        
        /// <summary>
        /// 提交玩家成就点
        /// </summary>
        public static void UpdateRoleAchievementRank(float Points)
        {
            HyzRankRoleInfo temp = new HyzRankRoleInfo()
            {
                uuid = PlatformMgr.Instance.OpenId,
                name = PlatformMgr.NickName,
                iconUrl = PlatformMgr.IconUrl,
                titleId =(int) Points,
                level = (int) Points,
            };
            HyzRankManager.Instance.UpLoadRank(AchievementRankName, temp);
        }

        #endregion

#if UNITY_EDITOR
        [FoldoutGroup("GM-添加-移除-道具")]
        [LabelText("添加,移除 道具数量")]
        public List<BagPropIdCount> bagPropIdCount;
        
        [FoldoutGroup("GM-添加-移除-道具")]
        [Button("添加道具")]
        private void AddProps()
        {
            foreach (var item in bagPropIdCount)
            {
                BagMgr.Instance.AddBagProp(item.bagPropId, item.bagPropCount);
            }
        }
        
        [FoldoutGroup("GM-添加-移除-道具")]
        [Button("移除道具")]
        private void RemoveProps()
        {
            foreach (var item in bagPropIdCount)
            {
                BagMgr.Instance.RemoveBagProp(item.bagPropId, item.bagPropCount);
            }
        }
#endif
    }
}