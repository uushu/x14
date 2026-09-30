using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;

namespace GameMain.FlowerBlock
{
    public class FlowerBlockEndlessWindow : AMiniGameBase
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnSetting;

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

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.FlowerBlock.BlockEndlessRule mBlockEndlessRule;

        public override int MaxReliveCount => ConfigMgr.Instance.Tables.GlobalConfigCategory.BlockEndlessMaxReliveCount;

        protected override void OnShow(object parameters)
        {
            base.OnShow(parameters);

            AudioMgr.Instance.PlayBgm(AudioName.BgmFlowerBlock);
            mBtnSetting.AddListener(OnSettingClick);

            mBtnCrush.SetUseBagPropCallback(false, UseBagPropCondition, OnUseCrushBagPropClick, LookAdsType.花了个花击碎道具);
            mBtnRefresh.SetUseBagPropCallback(false, UseBagPropCondition, OnUseRefreshBagPropClick, LookAdsType.花了个花刷新道具);
            mBtnRemove.SetUseBagPropCallback(false, UseBagPropCondition, OnUseRemoveBagPropClick, LookAdsType.花了个花消除道具);
            AudioMgr.Instance.PlayBgm(AudioName.BgmGarden);

            BlockGameRoot.Instance.Initialize();
            mFlowerBlockLongTimeNotOperation.Initialize();
            mFlowerBlockCombo.Initialize();
            StartGame();
        }

        private bool UseBagPropCondition()
        {
            return BlockGameRoot.Instance.IsForbidenInput == false;
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

            mBlockEndlessRule.UnInitialize();
            mBlockEndlessRule.Initialize();
            mBlockEndlessRule.OnGameFailedCallBack = OnFlowerBlockTopFull;

            mFlowerBlockCombo.ResetTimer();

            //随机一个无尽Id
            var levelConfigs = ConfigMgr.Instance.Tables.BlockEndlessLevelCategory.DataList;
            var randomLevelConfig = UtilityTool.RandomNumber(levelConfigs);
            var refreshRule = new BlockEndlessRefresh();
            BlockGameRoot.Instance.StartGame(randomLevelConfig.Id, refreshRule);
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
                    var currentScore = mBlockEndlessRule.mGardenEndlessScoreProgress.currentScore;
                    LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerBlock>().SetEndlessMaxScore(currentScore, 0);

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

        private void OpenFailedWindow()
        {
            BlockGameRoot.Instance.ChangeStatus(EStatus.Failed);
            UIWindowMgr.Instance.OpenWindow<FlowerBlockFailedWindow>(true);

            var currentScore = mBlockEndlessRule.mGardenEndlessScoreProgress.currentScore;
            LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerBlock>().SetEndlessMaxScore(currentScore, 0);
        }

        private void OnFlowerBlockTopFull()
        {
            OpenReliveWindow(1f);
        }

        public override void TrySetNextLevel()
        {
            throw new NotImplementedException();
        }
    }
}