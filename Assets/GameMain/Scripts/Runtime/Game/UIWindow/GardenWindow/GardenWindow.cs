using cfg;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using GameMain.AOT;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain.Garden
{
    public partial class GardenWindow : AMiniGameBase
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnSetting;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.Button_UseBagProp mBtnRefresh;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.Button_UseBagProp mBtnRemove;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.Button_UseBagProp mBtnRotate;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.GameObject mImgRotateTip;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.Garden.GardenScoreRule mProgressRule;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.Garden.GardenCollectButterflyRule mCollectFlowerRule;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.Garden.GardenLongTimeNotOperation mGardenLongTimeNotOperation;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.Transform mRemoveEffect;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public Spine.Unity.SkeletonGraphic mSkeletonGraphic_jiandao;

#if UNITY_EDITOR
        [LabelText("俄罗斯花园测试Id")]
        public GardenLevelId mTestLevelId;
#endif
        public GardenLevelConfig LevelConfig { get; private set; }
        public int MaxScore => LevelConfig.CollectScore;
        /// <summary>
        /// 游戏时间
        /// </summary>
        public float gameTime;

        public override int MaxReliveCount => ConfigMgr.Instance.Tables.GlobalConfigCategory.GardenMaxReliveCount;

        protected override void OnShow(object parameters)
        {
            base.OnShow(parameters);

            mRemoveEffect.gameObject.SetActive(false);
            mBtnSetting.AddListener(OnSettingClick);

            GardenGameRoot.Instance.Initialize();
            GardenGameRoot.Instance.flowerGridsMap.OnPlaceToMapBeforeCallBack += OnFlowerPlaceToMapBefore;
            mGardenLongTimeNotOperation.Initialize();

            StartGame();
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            gameTime += deltaTime;
        }

        private void OnFlowerPlaceToMapBefore(FlowerRemoveResult result)
        {
            mImgRotateTip.SetActive(false);
        }

        private bool UseBagPropCondition()
        {
            return GardenGameRoot.Instance.IsForbidenInput == false;
        }

        protected override void OnHide()
        {
            base.OnHide();

            GardenGameRoot.Instance.flowerGridsMap.OnPlaceToMapBeforeCallBack -= OnFlowerPlaceToMapBefore;
            mGardenLongTimeNotOperation.UnInitialize();
            GardenGameRoot.Instance.UnInitialize();
        }

        public void ManualStarGame()
        {
            StartGame();
            var levelSave = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>();
            RecordMgr.Instance.ReportLevelRestart($"{Init.Instance.miniGameType}_普通模式",levelSave.levelId,gameTime);
        }

        protected override void StartGame()
        {
            base.StartGame();

            AudioMgr.Instance.PlayBgm(AudioName.BgmGarden);
            var levelId = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().levelId;

#if UNITY_EDITOR

            if (mTestLevelId > 0)
            {
                levelId = mTestLevelId;
            }
#endif

            RefreshLevel(levelId);

            var refreshRule = new Garden_Weight_Refresh();
            GardenGameRoot.Instance.StartGame(levelId, refreshRule);

            TryAddNewGuide(levelId);
        }

        private void RefreshLevel(int levelId)
        {
            gameTime = 0;
            LevelConfig = ConfigMgr.Instance.Tables.GardenLevelCategory.GetOrDefault(levelId);
            RecordMgr.Instance.ReportLevelStart($"{Init.Instance.miniGameType}_普通模式",levelId);
            
            RefreshLevelText(LevelConfig.LevelName);
            mImgRotateTip.SetActive(false);

            //设置通关规则
            mProgressRule.gameObject.SetActive(false);
            mCollectFlowerRule.gameObject.SetActive(false);
            mProgressRule.UnInitialize();
            mCollectFlowerRule.UnInitialize();
            if (LevelConfig.CollectScore > 0)
            {
                mProgressRule.gameObject.SetActive(true);
                //积分通关
                mProgressRule.Initialize(LevelConfig.Id, LevelConfig.CollectScore);
                mProgressRule.ShowGamSuccessUI = OnShowGamSuccessUI;
                mProgressRule.ShowFailedUI = OnShowGameFailedUI;
            }
            else
            {
                mCollectFlowerRule.gameObject.SetActive(true);
                //收集鲜花通关
                mCollectFlowerRule.Initialize(LevelConfig.Id, LevelConfig.CollectFlower);
                mCollectFlowerRule.ShowGamSuccessUI = OnShowGamSuccessUI;
                mCollectFlowerRule.ShowFailedUI = OnShowGameFailedUI;
            }

            //局内免费使用道具
            mBtnRefresh.SetUseBagPropCallback(LevelConfig.FreeUseBagProps.Contains(mBtnRefresh.propId),
                () => UseBagPropCondition() && GardenGameRoot.Instance.waitingArea.IsGeneratorFlowers == false,
                OnRefreshBagProp, LookAdsType.俄罗斯花园刷新道具);

            mBtnRemove.SetUseBagPropCallback(LevelConfig.FreeUseBagProps.Contains(mBtnRemove.propId),
               () =>
               {
                   return UseBagPropCondition() && GardenGameRoot.Instance.flowerGridsMap.MapIsEmpty() == false;
               },
                OnRemoveBagProp, LookAdsType.俄罗斯花园消除道具);

            mBtnRotate.SetUseBagPropCallback(LevelConfig.FreeUseBagProps.Contains(mBtnRotate.propId),
                 UseBagPropCondition, OnRotateBagProp, LookAdsType.俄罗斯花园旋转道具);

            if (LevelConfig.ShowPassRuleTip > 0)
            {
                UIWindowMgr.Instance.OpenWindow<GardenLevelGoalWindow>(levelId);
            }
        }

        public void AutoNextLevel()
        {
            OpenSuccessWindow();
        }

        private async void OnShowGamSuccessUI()
        {
            var waitTime = await GardenGameRoot.Instance.flowerGridsMap.OnGameSuccessBloomAllFlower();
            await UniTask.WaitForSeconds(waitTime);
            OpenSuccessWindow();
        }

        private void OnShowGameFailedUI()
        {
            OpenReliveWindow(1f);
            var levelSave = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>();
            RecordMgr.Instance.ReportLevelFailed($"{Init.Instance.miniGameType}_普通模式",levelSave.levelId,gameTime);
        }

        private void OpenSuccessWindow()
        {
            var levelSave = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>();
            var oldLevel = levelSave.levelId;
            levelSave.OnGameSuccess();
            //游戏通关
            TrySetNextLevel();

            RecordMgr.Instance.ReportLevelSuccess($"{Init.Instance.miniGameType}_普通模式",oldLevel,gameTime);
            //打开通关的界面
            UIWindowMgr.Instance.OpenWindow<GardenSuccessWindow>(oldLevel);
        }

        public override void TrySetNextLevel()
        {
            base.TrySetNextLevel();

            var levelSave = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>();
            var collectFragment= LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_CollectFragments>();
            var CurlevelConfig = ConfigMgr.Instance.Tables.GardenLevelCategory.GetOrDefault(levelSave.levelId);
            if (CurlevelConfig!=null)
            {
                if (!collectFragment.fragments.Contains(CurlevelConfig.AwardCollectFragments))
                {
                    collectFragment.fragments.Add(CurlevelConfig.AwardCollectFragments);
                }
                foreach (var key in CurlevelConfig.AwardProps.Keys)
                {
                    BagMgr.Instance.AddBagProp(key, CurlevelConfig.AwardProps[key]);
                    if (key == GameTool.ItemGoldId)
                    {
                        var bank= LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_PiggyBank>(true);
                        bank.AddPiggyBankMoney(CurlevelConfig.AwardProps[key]);
                    }
                }
                foreach (var reward in CurlevelConfig.ExtraAwardProps)
                {
                    BagMgr.Instance.AddBagProp(reward.Key, reward.Value);
                }
            }
            GameEventDefine.SendFlowerGardenPass(levelSave.levelId);
            GameEntrance.UpdateRoleLevelRank(levelSave.levelId);
            var levelConfig = ConfigMgr.Instance.Tables.GardenLevelCategory.GetOrDefault(levelSave.levelId + 1);
            if (levelConfig != null)
            {
                levelSave.SetNextLevelId(levelSave.levelId + 1);
            }

#if UNITY_EDITOR

            //编辑器下自动下一关
            if (mTestLevelId > 0)
            {
                var nextTestId = mTestLevelId + 1;
                levelConfig = ConfigMgr.Instance.Tables.GardenLevelCategory.GetOrDefault(nextTestId);
                if (levelConfig != null)
                {
                    mTestLevelId = nextTestId;
                }
            }
#endif

            GameEventDefine.SendFlowerGardenLevelPassChange();
        }

        private void OnRotateBagProp()
        {
            mImgRotateTip.SetActive(true);
            AudioMgr.Instance.PlayAudioOneShot(AudioName.UseBagPropAudio);
            GardenGameRoot.Instance.waitingArea.ShowRotateButton();
        }

        private Vector3 CalcAvgPosition(List<Vector2Int> gridsXY)
        {
            var totalPosition = Vector3.zero;

            foreach (var item in gridsXY)
            {
                var gridChild = GardenGameRoot.Instance.flowerGridsMap.customGrids.GetChildByIndex(item);
                totalPosition += gridChild.position;
            }

            return totalPosition / gridsXY.Count;
        }

        private async void OnRemoveBagProp()
        {
            //随机移除 5X5的格子
            var gridsXY = GardenGameRoot.Instance.flowerGridsMap.GetRandomClearRowCols(5, 5);

            var avgPosition = CalcAvgPosition(gridsXY);
            mRemoveEffect.gameObject.SetActive(true);
            mRemoveEffect.transform.position = avgPosition;

            mSkeletonGraphic_jiandao.DOKill();
            mSkeletonGraphic_jiandao.SetAlpha(1);

            AudioMgr.Instance.PlayAudioOneShot(AudioName.GardenRemoveBagPropAudio);

            foreach (var gridXY in gridsXY)
            {
                var flower = GardenGameRoot.Instance.flowerGridsMap.GetFlowerBy(gridXY);
                if (flower != null)
                {
                    flower.transform.DOKill();
                    var t1 = flower.transform.DOScale(Vector3.zero, 0.7f).SetEase(Ease.InOutBack).SetDelay(0.3f).OnComplete(() =>
                    {
                        GardenGameRoot.Instance.flowerGridsMap.SetGridDataNull(gridXY, true);
                    });
                }

                var gridChild = GardenGameRoot.Instance.flowerGridsMap.customGrids.GetChildByIndex(gridXY);
                var image = gridChild.GetComponent<Image>();
                image.color = Color.red;

                image.DOColor(Color.white, 0.7f).SetDelay(0.3f).ToUniTask().Forget();
            }

            mSkeletonGraphic_jiandao.DOFade(0f, 0.7f).SetDelay(0.3f).ToUniTask().Forget();

            await UniTask.WaitForSeconds(1.2f);
            mRemoveEffect.gameObject.SetActive(false);

            //刷新等待区的格子是否可以拖拽
            GardenGameRoot.Instance.waitingArea.RefreshShapeEnable();
        }

        private void OnRefreshBagProp()
        {
            AudioMgr.Instance.PlayAudioOneShot(AudioName.UseBagPropAudio);
            GardenGameRoot.Instance.waitingArea.UseBagPropRefresh();
        }

        private async void OnSettingClick()
        {
            if (GardenGameRoot.Instance.IsForbidenInput)
            {
                Debugger.LogError($"{GetType().Name} 正在播放消除动画");
                return;
            }

            GardenGameRoot.Instance.PauseGame();
            var settingWindow = await UIWindowMgr.Instance.OpenWindowAsync<GardenSettingWindow>();
            settingWindow.OnBackHallClickCallBack = () =>
            {
                UIWindowTool.ChangeToHallScene(false).Forget();
            };
            settingWindow.OnCloseSettingWindowClick = () =>
            {
                GardenGameRoot.Instance.ResumeGame();
            };
            settingWindow.OnRestartClickCallBack = () =>
            {
                ManualStarGame();
            };
        }

        private async void OpenReliveWindow(float delayTime)
        {
            GardenGameRoot.Instance.ChangeStatus(EStatus.Relive);

            await UniTask.WaitForSeconds(delayTime);

            if (IsCanRelive)
            {
                var reliveWindow = await UIWindowMgr.Instance.OpenWindowAsync<GardenReliveWindow>(LastReliveCount);
                reliveWindow.OnRestartCallBack = () =>
                {
                    ManualStarGame();
                };
                reliveWindow.OnCloseReliveCallBack = () =>
                {
                    //打开失败窗口
                    GardenGameRoot.Instance.ChangeStatus(EStatus.Failed);

                    var levelSave = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>();
                    levelSave.OnGameFailed();
                    UIWindowMgr.Instance.OpenWindow<GardenFailedWindow>(levelSave.levelId);
                };
                reliveWindow.OnReliveCallBack = () =>
                {
                    //刷新等待区
                    alreadyReliveCount++;
                    var levelSave = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>();
                    RecordMgr.Instance.ReportLevelRelive($"{Init.Instance.miniGameType}_普通模式",levelSave.levelId,gameTime);

                    GardenGameRoot.Instance.flowerGridsMap.AllColsDropOut(() =>
                    {
                        if (GardenGameRoot.Instance.currentStatus != EStatus.Success)
                        {
                            GardenGameRoot.Instance.ChangeStatus(EStatus.Gaming);
                            GardenGameRoot.Instance.waitingArea.ReliveRefresh();
                        }
                    });
                };
            }
            else
            {
                //打开失败窗口
                GardenGameRoot.Instance.ChangeStatus(EStatus.Failed);

                var levelSave = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>();
                levelSave.OnGameFailed();
                UIWindowMgr.Instance.OpenWindow<GardenFailedWindow>(levelSave.levelId);
            }
        }
    }
}