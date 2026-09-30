using cfg;
using Cysharp.Threading.Tasks;
using System.Linq;
using GameMain.AOT;

namespace GameMain.Garden
{
    public class GardenEndlessWindow : AMiniGameBase
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnSetting;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnRule;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnRank;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public TMPro.TextMeshProUGUI mTxtMyBestScore;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.Image mImgTitle;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public TMPro.TextMeshProUGUI mTxtTitleScore;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.Garden.GardenEndlessRule mGardenEndlessRule;

        private GardenEndlessTitleConfig _currentTitleConfig;

        public override int MaxReliveCount => ConfigMgr.Instance.Tables.GlobalConfigCategory.GardenEndlessMaxReliveCount;

        private GardenEndlessLevelConfig randomConfig;

        public float gameTime;

        protected override void OnShow(object parameters)
        {
            base.OnShow(parameters);

            mBtnSetting.AddListener(OnSettingClick);
            mBtnRule.AddListener(OnRuleClick);
            mBtnRank.AddListener(OnRankClick);
            mGardenEndlessRule.OnScoreChangeCallBack = OnScoreChange;

            GardenGameRoot.Instance.Initialize();
            StartGame();
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (GardenGameRoot.Instance.currentStatus == EStatus.Gaming)
            {
                gameTime += deltaTime;
            }
        }

        private void OnScoreChange(int currentScore)
        {
            if (_currentTitleConfig == null)
            {
                //一级开始
                _currentTitleConfig = ConfigMgr.Instance.Tables.GardenEndlessTitleCategory.GetOrDefault(2);
                RefreshMyNextTargetTitle(_currentTitleConfig);
            }

            if (currentScore > _currentTitleConfig.NeedScore)
            {
                //弹出等级提高
                UIWindowMgr.Instance.OpenWindow<GardenEndlessTitleUpWindow>(_currentTitleConfig.Id);

                var nextTitleConfig = ConfigMgr.Instance.Tables.GardenEndlessTitleCategory.GetOrDefault(_currentTitleConfig.Id + 1);
                if (nextTitleConfig != null)
                {
                    _currentTitleConfig = nextTitleConfig;
                    RefreshMyNextTargetTitle(_currentTitleConfig);
                }
            }
        }

        protected override void OnHide()
        {
            base.OnHide();

            GardenGameRoot.Instance.UnInitialize();
        }

        protected override void StartGame()
        {
            base.StartGame();
            gameTime = 0;
            AudioMgr.Instance.PlayAudioOneShot(AudioName.GaedenGameGoalAudio);
            AudioMgr.Instance.PlayBgm(AudioName.BgmGarden);

            randomConfig = RandomEndlessLevel();
            mGardenEndlessRule.UnInitialize();
            mGardenEndlessRule.Initialize(randomConfig.Id, null);
            mGardenEndlessRule.ShowFailedUI = OnShowGameFailedUI;

            var refreshRule = new Garden_Endless_Refresh();
            GardenGameRoot.Instance.StartGame(randomConfig.Id, refreshRule);

            _currentTitleConfig = null;
            OnScoreChange(0);
            RecordMgr.Instance.ReportLevelStart($"{Init.Instance.miniGameType}_无尽模式",randomConfig.Id);
        }

        private void OnShowGameFailedUI()
        {
            //无尽模式失败
            OpenReliveWindow(1f);
            RecordMgr.Instance.ReportLevelFailed($"{Init.Instance.miniGameType}_无尽模式",randomConfig.Id,gameTime,mGardenEndlessRule.endlessScoreProgress.currentScore);
        }

        private async void OpenReliveWindow(float delayTime)
        {
            GardenGameRoot.Instance.ChangeStatus(EStatus.Relive);

            await UniTask.WaitForSeconds(delayTime);

            if (IsCanRelive)
            {
                var reliveWindow = await UIWindowMgr.Instance.OpenWindowAsync<GardenReliveWindow>(LastReliveCount);
                reliveWindow.OnRestartCallBack = () => { StartGame(); };
                reliveWindow.OnCloseReliveCallBack = async () =>
                {
                    //打开失败窗口
                    GardenGameRoot.Instance.ChangeStatus(EStatus.Failed);

                    var currentScore = mGardenEndlessRule.endlessScoreProgress.currentScore;
                    int titleConfigId = GetMyEndlessTitle(currentScore).Id;
                    LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().SetEndlessMaxScore(currentScore, titleConfigId);
                    
                    var scoreView = await UIWindowMgr.Instance.OpenWindowAsync<GardenEndlessScoreWindow>();
                    scoreView.ComeagainAction = () =>
                    {
                        StartGame();
                    };
                };
                reliveWindow.OnReliveCallBack = () =>
                {
                    //刷新等待区
                    alreadyReliveCount++;
                    
                    RecordMgr.Instance.ReportLevelRelive($"{Init.Instance.miniGameType}_无尽模式",randomConfig.Id,gameTime,mGardenEndlessRule.endlessScoreProgress.currentScore);
                    GardenGameRoot.Instance.flowerGridsMap.AllColsDropOut(() =>
                    {
                        GardenGameRoot.Instance.ChangeStatus(EStatus.Gaming);
                        GardenGameRoot.Instance.waitingArea.ReliveRefresh();
                    });
                };
            }
            else
            {
                //打开失败窗口
                GardenGameRoot.Instance.ChangeStatus(EStatus.Failed);

                var currentScore = mGardenEndlessRule.endlessScoreProgress.currentScore;
                int titleConfigId = GetMyEndlessTitle(currentScore).Id;
                LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().SetEndlessMaxScore(currentScore, titleConfigId);

                var scoreView = await UIWindowMgr.Instance.OpenWindowAsync<GardenEndlessScoreWindow>();
                scoreView.ComeagainAction = () =>
                {
                    StartGame();
                };
            }
        }

        private GardenEndlessLevelConfig RandomEndlessLevel()
        {
            var allEndless = ConfigMgr.Instance.Tables.GardenEndlessLevelCategory.DataList;
            var randomConfig = UtilityTool.RandomNumber(allEndless);
            return randomConfig;
        }

        private GardenEndlessTitleConfig GetMyEndlessTitle(int myBestScore)
        {
            var list = ConfigMgr.Instance.Tables.GardenEndlessTitleCategory.DataList;

            for (int i = 0; i < list.Count - 2; i++)
            {
                var before = list[i].NeedScore;
                var after = list[i + 1].NeedScore;

                if (myBestScore >= before && myBestScore < after)
                {
                    return list[i];
                }
            }

            return list.LastOrDefault();
        }

        private void RefreshMyNextTargetTitle(GardenEndlessTitleConfig titleConfig)
        {
            var bestScore = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().endlessBestScore;
            mTxtMyBestScore.text = UtilityTool.ShortNumberToString(bestScore);

            ResourceMgr.Instance.SetImageSprite(mImgTitle, titleConfig.TitleSprite, true);
            mTxtTitleScore.text = UtilityTool.ShortNumberToString(titleConfig.NeedScore);
        }

        private void OnRankClick()
        {
            UIWindowMgr.Instance.OpenWindowAsync<RankWindow>(2);
            //UIWindowMgr.Instance.OpenWindowAsync<RankWindow1>(2);
        }

        private void OnRuleClick()
        {
            UIWindowMgr.Instance.OpenWindowAsync<GardenEndlessRuleWindow>();
        }

        private async void OnSettingClick()
        {
            GardenGameRoot.Instance.PauseGame();
            var settingWindow = await UIWindowMgr.Instance.OpenWindowAsync<GardenSettingWindow>();
            settingWindow.mBtnNextLevel.gameObject.SetActive(false);
            settingWindow.OnBackHallClickCallBack = () => { UIWindowTool.ChangeToHallScene(false).Forget(); };
            settingWindow.OnCloseSettingWindowClick = () => { GardenGameRoot.Instance.ResumeGame(); };
            settingWindow.OnRestartClickCallBack = () => { StartGame(); };
        }
    }
}