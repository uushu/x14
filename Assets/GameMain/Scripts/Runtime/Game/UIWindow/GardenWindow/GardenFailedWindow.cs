using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace GameMain.Garden
{
    public class GardenFailedWindow : ABaseWindow
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnMainPage;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnRestart;
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnShare;

        protected override void OnShow(object parameters)
        {
            AudioMgr.Instance.PlayAudioOneShot(AudioName.GardenFailedAudio);
            base.OnShow(parameters);
            int levelId = (int)parameters;

            mBtnMainPage.AddListener(OnMainPageClick);
            mBtnRestart.AddListener(OnRestartClick);
            mBtnShare.AddListener(OnShareClick);
        }

        private void OnShareClick()
        {
            PlatformMgr.Instance.ShowShare("快来挑战！！！！", (flag) =>
            {
              
            });
        }

        private void OnRestartClick()
        {
            Save_CostPower saveData = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_CostPower>();
            int tableDataCost = ConfigMgr.Instance.Tables.GlobalConfigCategory.NormalLevelCost;
            if (saveData.CurrentPower >= tableDataCost)
            {
                saveData.RemoveCostPower(tableDataCost);
                UIWindowTool.ShowMaskWindow(0);
                CostPowerManager.Instance.ShowCostPowerAniTips(gameObject.transform,
                    mBtnRestart.transform.localPosition, tableDataCost, () =>
                    {
                        CloseSelfWindow();
                        UIWindowMgr.Instance.GetWindow<GardenWindow>().ManualStarGame();
                    });
            }
            else
            {
                //UIWindowTool.ShowPromptBox("体力不足");
                //体力弹窗
                UIWindowMgr.Instance.OpenWindow<PowerWindow>(false);
            }
        }

        private void OnMainPageClick()
        {
            UIWindowTool.ChangeToHallScene(false).Forget();
        }
    }
}