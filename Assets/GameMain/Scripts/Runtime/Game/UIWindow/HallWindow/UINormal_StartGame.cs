using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using GameMain.Water;
using GameMain.AOT;

namespace GameMain
{
    public class UINormal_StartGame : ABaseComponent
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnStartGame;

        private void Start()
        {
            mBtnStartGame.AddListener(OnStartGameClick);
        }

        public void OnStartGameClick()
        {
            Save_CostPower saveData = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_CostPower>();
            int tableDataCost = ConfigMgr.Instance.Tables.GlobalConfigCategory.NormalLevelCost;
            //无限体力
            if (saveData.IsInfiniteCostPower)
            {
                UIWindowTool.ShowMaskWindow(0);
                var hallwindow = UIWindowMgr.Instance.GetWindow<HallWindow>();
                hallwindow.OnHideBtn();
                CostPowerManager.Instance.ShowCostPowerAniTips(mBtnStartGame.transform.parent.transform,
                    mBtnStartGame.transform.localPosition, tableDataCost, () =>
                    {
                        UIWindowTool.CloseMaskWindow();
                        EnterGame();
                    });
                return;
            }
            
            //扣体力
            if (saveData.CurrentPower >= tableDataCost)
            {
                saveData.RemoveCostPower(tableDataCost);
                UIWindowTool.ShowMaskWindow(0);
                var hallwindow = UIWindowMgr.Instance.GetWindow<HallWindow>();
                hallwindow.OnHideBtn();
                CostPowerManager.Instance.ShowCostPowerAniTips(mBtnStartGame.transform.parent.transform,
                    mBtnStartGame.transform.localPosition, tableDataCost, () =>
                    {
                        UIWindowTool.CloseMaskWindow();

                        EnterGame();
                    });
            }
            else
            {
               // UIWindowTool.ShowPromptBox("体力不足");
                //体力弹窗
                UIWindowMgr.Instance.OpenWindow<PowerWindow>(true);
            }
        }

        private void EnterGame()
        {
            switch (Init.Instance.miniGameType)
            {
                case EMiniGameType.GardenFlower:

                    UIWindowTool.ChangeToGardenGame(false).Forget();

                    break;
                case EMiniGameType.BlockFlower:

                    UIWindowTool.ChangeToBlockGame(false).Forget();

                    break;
                case EMiniGameType.LineFlower:

                    UIWindowTool.ChangeToLineGame(false).Forget();
                    break;
                default:
                    break;
            }
        }
    }
}