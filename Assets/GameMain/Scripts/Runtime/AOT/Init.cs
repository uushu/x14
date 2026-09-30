using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using YooAsset;

namespace GameMain.AOT
{
    public class Init : MonoBehaviour
    {
        //多平台部署
        public enum EPlatform
        {
            [LabelText("默认")] Default,
            [LabelText("微信")] WeiXin,
            [LabelText("抖音")] DouYin,
        }

        public static Init Instance { get; private set; }

        public const string DefaultPackage = "DefaultPackage";

        [LabelText("资源加载方式")]
        public EPlayMode playMode = EPlayMode.EditorSimulateMode;

        [LabelText("资源平台路径")]
        public EPlatform platform;

        [LabelText("开启GM")]
        public bool openGmWindow;

        [LabelText("开启新手引导")]
        public bool openGuide;

        [LabelText("小游戏类型")]
        public EMiniGameType miniGameType;

        [LabelText("版本号(修改AS)")] [GUIColor("red")]
        public string appVersion;

        public UILoadingWindow loadingWindow;

        [HideInInspector] public bool playerIsClickStart;
        private readonly EventGroup _eventGroup = new EventGroup();

        //设置单例
        private void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        //资源加载模式
        private IEnumerator Start()
        {
#if !UNITY_EDITOR
            playMode = EPlayMode.WebPlayMode;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
            playMode = EPlayMode.OfflinePlayMode;
#endif
            Debug.Log($"YooAsset 运行模式:{playMode}");
           
            // 初始化资源系统
            YooAssets.Initialize();
            
            //添加显示进度的监听
            _eventGroup.AddListener<UserEventDefine.UserProgressUpdateValue>(UIShowChange);
            //现实加载界面
            loadingWindow.gameObject.SetActive(true);
            //设置点击回调
            loadingWindow.SetStartAction(() => { playerIsClickStart = true; });
#if !UNITY_EDITOR
            if (platform == EPlatform.Default)
            {
                playerIsClickStart = true;
            }
#endif
            AutoClickStartBtn();
            yield return new WaitUntil(() => playerIsClickStart);

            // 开始资源更新
            PatchOperation operation = new PatchOperation(DefaultPackage, playMode);
            YooAssets.StartOperation(operation);

            yield return operation;

        }
        
        //销毁事件监听
        private void OnDestroy()
        {
            _eventGroup.RemoveAllListener();
        }

        /// <summary>
        /// 自动帮玩家点击开始游戏
        /// </summary>
        public void AutoClickStartBtn()
        {
            loadingWindow.OnClickStartBtn();
        }

        /// <summary>
        /// 进度事件
        /// </summary>
        private void UIShowChange(IEventMessage message)
        {
            if (message is UserEventDefine.UserProgressUpdateValue value)
            {
                loadingWindow?.ShowBarValue(value.Progress);
            }
        }
    }
}