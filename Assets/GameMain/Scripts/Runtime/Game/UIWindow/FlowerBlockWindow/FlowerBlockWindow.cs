using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;

namespace GameMain.FlowerBlock
{
    public class FlowerBlockWindow : AMiniGameBase
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnSetting;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.FlowerBlock.BlockCollectFlowerRule mCollectFlowerRule;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.Button_UseBagProp mBtnCrush;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.Button_UseBagProp mBtnRefresh;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.Button_UseBagProp mBtnRemove;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.FlowerBlock.FlowerBlockLongTimeNotOperation mFlowerBlockLongTimeNotOperation;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.FlowerBlock.FlowerBlockCombo mFlowerBlockCombo;

        public override int MaxReliveCount => ConfigMgr.Instance.Tables.GlobalConfigCategory.BlockMaxReliveCount;

#if UNITY_EDITOR
        [LabelText("花了个花测试关卡Id")]
        public BlockLevelId testLevelId;
#endif

        protected override void OnShow(object parameters)
        {
            base.OnShow(parameters);

            AudioMgr.Instance.PlayBgm(AudioName.BgmFlowerBlock);
            mBtnSetting.AddListener(OnSettingClick);

            mBtnCrush.SetUseBagPropCallback(false, UseBagPropCondition, OnUseCrushBagPropClick, LookAdsType.花了个花击碎道具);
            mBtnRefresh.SetUseBagPropCallback(false, UseBagPropCondition, OnUseRefreshBagPropClick, LookAdsType.花了个花刷新道具);
            mBtnRemove.SetUseBagPropCallback(false, UseBagPropCondition, OnUseRemoveBagPropClick, LookAdsType.花了个花消除道具);

            BlockGameRoot.Instance.Initialize();
            mFlowerBlockLongTimeNotOperation.Initialize();
            mFlowerBlockCombo.Initialize();
            StartGame();
        }

        private bool UseBagPropCondition()
        {
            return BlockGameRoot.Instance.IsForbidenInput == false;
        }

        public void AutoNextLevel()
        {
            //打开结算的窗口
            OpenSuccessWindow(0);
        }

        private void OnUseRemoveBagPropClick()
        {
            BlockGameRoot.Instance.ResetRefreshWaveIndex();
            BlockGameRoot.Instance.blockMap.OnUseRemoveBagProp();
        }

        private void OnUseRefreshBagPropClick()
        {
            BlockGameRoot.Instance.ResetRefreshWaveIndex();
            BlockGameRoot.Instance.blockMap.OnUseRefreshBagProp();
        }

        private void OnUseCrushBagPropClick()
        {
            BlockGameRoot.Instance.ResetRefreshWaveIndex();
            BlockGameRoot.Instance.blockMap.OnUseCurshBagProp();
        }

        private async void OnSettingClick()
        {
            if (BlockGameRoot.Instance.IsForbidenInput)
            {
                Debugger.LogError($"{GetType().Name} 正在播放消除动画");
                return;
            }

            BlockGameRoot.Instance.PauseGame();
            var settingWindow = await UIWindowMgr.Instance.OpenWindowAsync<FlowerBlockSettingWindow>();
            //var settingWindow = await UIWindowMgr.Instance.OpenWindowAsync<FlowerBlockSettingWindow2>();
            settingWindow.OnBackHallClickCallBack = () =>
            {
                UIWindowTool.ChangeToHallScene(false).Forget();
            };
            settingWindow.OnCloseSettingWindowClick = () =>
            {
                BlockGameRoot.Instance.ResumeGame();
            };
            settingWindow.OnRestartClickCallBack = () =>
            {
                ManualStarGame();
            };
        }

        public void ManualStarGame()
        {
            StartGame();
        }

        protected override void OnHide()
        {
            base.OnHide();
            mFlowerBlockCombo.UnInitialize();
            mFlowerBlockLongTimeNotOperation.UnInitialize();
            BlockGameRoot.Instance.UnInitialize();
        }

        protected override void StartGame()
        {
            base.StartGame();

            var blockLevelId = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerBlock>().levelId;

#if UNITY_EDITOR

            if (testLevelId > 0)
            {
                blockLevelId = testLevelId;
                LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerBlock>().levelId = testLevelId;
            }
#endif

            var levelConfig = ConfigMgr.Instance.Tables.BlockLevelCategory.Get(blockLevelId);

            mFlowerBlockCombo.ResetTimer();
            mCollectFlowerRule.UnInitialize();
            mCollectFlowerRule.Initialize(levelConfig.Id, levelConfig.CollectFlower);
            mCollectFlowerRule.OnGameSuccessCallBack = OnGameSuccess;
            mCollectFlowerRule.OnGameFailedCallBack = OnFlowerBlockTopFull;
            RefreshLevelText(levelConfig.LevelName);

            var refreshRule = new BlockRefresh();
            BlockGameRoot.Instance.StartGame(blockLevelId, refreshRule);
        }

        private async void OpenReliveWindow(float delayTime)
        {
            BlockGameRoot.Instance.ChangeStatus(EStatus.Relive);
            UIWindowTool.ShowPromptBox("顶部已满");

            await UniTask.WaitForSeconds(delayTime);

            if (IsCanRelive)
            {
                var reliveWindow = await UIWindowMgr.Instance.OpenWindowAsync<FlowerBlockReliveWindow>(LastReliveCount);
                reliveWindow.OnRestartCallBack = () =>
                {
                    ManualStarGame();
                };
                reliveWindow.OnCloseReliveCallBack = () =>
                {
                    //打开失败窗口
                    OpenFailedWindow();
                };
                reliveWindow.OnReliveCallBack = () =>
                {
                    alreadyReliveCount++;

                    BlockGameRoot.Instance.ResetRefreshWaveIndex();

                    //移除顶部3行格子
                    BlockGameRoot.Instance.blockMap.OnUseReliveBagProp();
                    BlockGameRoot.Instance.ChangeStatus(EStatus.Gaming);

                };
            }
            else
            {
                //打开失败窗口
                OpenFailedWindow();
            }
        }

        private void OnGameSuccess()
        {
            OpenSuccessWindow(2f);
        }

        private async void OpenSuccessWindow(float delayTime)
        {
            UIWindowTool.ShowPromptBox("通关成功");
            var levelSave = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerBlock>();
            int lastLevelId = levelSave.levelId;
            levelSave.OnGameSuccess();
            TrySetNextLevel();

            await UniTask.WaitForSeconds(delayTime);

            UIWindowMgr.Instance.OpenWindow<FlowerBlockSuccessWindow>(lastLevelId);
        }

        private void OpenFailedWindow()
        {
            BlockGameRoot.Instance.ChangeStatus(EStatus.Failed);

            var levelSave = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerBlock>();
            levelSave.OnGameFailed();

            UIWindowMgr.Instance.OpenWindow<FlowerBlockFailedWindow>(false);
        }

        private void OnFlowerBlockTopFull()
        {
            OpenReliveWindow(1f);
        }

        public override void TrySetNextLevel()
        {
            base.TrySetNextLevel();

            var blockLevelId = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerBlock>().levelId;
            var nextLeveId = blockLevelId + 1;

            if (ConfigMgr.Instance.Tables.BlockLevelCategory.GetOrDefault(nextLeveId) != null)
            {
                LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerBlock>().SetNextLevelId(nextLeveId);
            }
        }
    }
}