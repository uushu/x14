using System.Collections;
using System.Collections.Generic;
using cfg;
using GameMain;
using GameMain.Water;
using TMPro;
using UnityEngine;

public class PowerWindow : ABaseWindow
{
    public TextMeshProUGUI addPowerNum1;
    public TextMeshProUGUI addPowerNum2;
    public TextMeshProUGUI addPowerNum3;

    public TextMeshProUGUI addPower1Tips;
    public TextMeshProUGUI addPower2Tips;
    public TextMeshProUGUI addPower3Tips;

    public UICustomButton addPower1Btn;
    public UICustomButton addPower2Btn;
    public UICustomButton addPower3Btn;


    public UICustomButton BuyPowerBtn;
    public UICustomButton AdPowerBtn;
    public TextMeshProUGUI DayTimeTips;
    public TextMeshProUGUI BuyPowerCostGoldText;
    public GameObject grid;
    public GameObject graygrid;
    public TextMeshProUGUI GrayBuyPowerCostGoldText;
    
    public TextMeshProUGUI addPower3BtnNum;

    public GameObject maskNode1;
    private bool canFreeShareGet;

    private int RemainingTimes;

    private bool isEnterGame;
    protected override void OnShow(object parameters)
    {
        base.OnShow(parameters);
        isEnterGame = (bool)parameters;
        UpdateUI();
        UpdateShow();
        addPower1Btn.AddListener(AddPower1Btn_OnClick);
        addPower2Btn.AddListener(AddPower2Btn_OnClick);
        addPower3Btn.AddListener(AddPower3Btn_OnClick);
        BuyPowerBtn.AddListener(OnClickBuyPower);
        AdPowerBtn.AddListener(OnClickAdPower);
    }

    private void UpdateUI()
    {      
        GlobalConfigCategory tableData = ConfigMgr.Instance.Tables.GlobalConfigCategory;
        RemainingTimes = 0;
         BuyPowerCostGoldText.text = tableData.BuyPowerCostGold.ToString();
         GrayBuyPowerCostGoldText.text = tableData.BuyPowerCostGold.ToString();
        var DayAddCostPowerCount = CostPowerManager.Instance.GetToDayAddCostPowerCount();
        RemainingTimes = tableData.BuyPowerDayMaxNum - DayAddCostPowerCount;
        DayTimeTips.text = $"今日剩余{RemainingTimes}次";
        grid.gameObject.SetActive(RemainingTimes>0);
        graygrid.gameObject.SetActive(RemainingTimes <= 0);
        BuyPowerBtn.SetGray(RemainingTimes <= 0);
    }

    private void OnClickBuyPower()
    {
        if (CostPowerManager.Instance.PowerIsMax())
        {
            UIWindowMgr.Instance.ShowPromptBox("体力已满");
            return;
        }
        
        if (RemainingTimes <= 0)
        {
            UIWindowMgr.Instance.ShowPromptBox("剩余次数不足");
        }
        else
        {
            if (BagMgr.Instance.BagPropEnough(1,ConfigMgr.Instance.Tables.GlobalConfigCategory.BuyPowerCostGold))
            {
                BagMgr.Instance.RemoveBagProp(1, ConfigMgr.Instance.Tables.GlobalConfigCategory.BuyPowerCostGold);
                GlobalConfigCategory tableData = ConfigMgr.Instance.Tables.GlobalConfigCategory;
                Save_CostPower data = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_CostPower>();
                int addPower = tableData.MaxCostPower - data.CurrentPower;
                addPower = Mathf.Max(addPower, 0);
                AddPowerCount(addPower);
                CostPowerManager.Instance.TagAddCostPowerCount();
                UpdateUI();
                CloseSelfWindow();
                if (isEnterGame)
                {
                    EnterGame();
                }
            }
            else
            {
                UIWindowMgr.Instance.ShowPromptBox("金币不足,可通过商店领取!");
            }
        }

    }

    private void EnterGame()
    {
       var hallwindow= UIWindowMgr.Instance.GetWindow<HallWindow>();
       if (hallwindow != null)
       {
           hallwindow.mBtnGame_LevelStartGame.OnStartGameClick();
       }
    }

    private void OnClickAdPower()
    {
        if (CostPowerManager.Instance.PowerIsMax())
        {
            UIWindowMgr.Instance.ShowPromptBox("体力已满");
            return;
        }
        
        PlatformMgr.Instance.LookAd((flag) =>
        {
            GlobalConfigCategory tableData = ConfigMgr.Instance.Tables.GlobalConfigCategory;
            Save_CostPower data = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_CostPower>();
            int addPower = tableData.MaxCostPower - data.CurrentPower;
            addPower = Mathf.Max(addPower, 0);
            AddPowerCount(addPower);
            CloseSelfWindow();
            if (isEnterGame)
            {
                EnterGame();
            }
        }, LookAdsType.看广告回满体力);
    }

    private void AddPowerCount(int addPower = 1)
    {
        CostPowerManager.Instance.AddCostPower(addPower);
        UIWindowTool.ShowPromptBox($"恢复体力:{addPower}");
    }

    public void UpdateShow()
    {
        GlobalConfigCategory tableData = ConfigMgr.Instance.Tables.GlobalConfigCategory;
        addPower1Tips.text = $"免费获取\n<color=#75cd2d>{tableData.PowerWindow1AddPower}个体力</color>";
        addPower2Tips.text = $"免费获取\n<color=#75cd2d>{tableData.PowerWindow2AddPower}个体力</color>";
        addPowerNum1.text = $"x{tableData.PowerWindow1AddPower}";
        addPowerNum2.text = $"x{tableData.PowerWindow2AddPower}";
        addPowerNum3.text = $"{tableData.PowerWindow3AddPower / 60}分钟";

        Save_CostPower saveData = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_CostPower>();
        addPower3BtnNum.text = $"{saveData.LookInfiniteCostPowerAdCount}/{tableData.PowerWindow3AdsNum}";

        canFreeShareGet = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Main>().ToDayIsCanFreeShareGetPower();
        maskNode1.SetActive(!canFreeShareGet);
    }

    private void AddPower1Btn_OnClick()
    {
        GlobalConfigCategory tableData = ConfigMgr.Instance.Tables.GlobalConfigCategory;
        PlatformMgr.Instance.ShowShare("", (result) =>
        {
            if (result)
            {
                LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Main>().TagToDayFreeShareGetPower();
                CostPowerManager.Instance.AddCostPower(tableData.PowerWindow1AddPower);
                UIWindowTool.ShowPromptBox($"恢复体力:{tableData.PowerWindow1AddPower}");
                UpdateShow();
            }
        });
    }

    private void AddPower2Btn_OnClick()
    {
        GlobalConfigCategory tableData = ConfigMgr.Instance.Tables.GlobalConfigCategory;
        PlatformMgr.Instance.LookAd((result) =>
        {
            if (result)
            {
                CostPowerManager.Instance.AddCostPower(tableData.PowerWindow2AddPower);
                UIWindowTool.ShowPromptBox($"恢复体力:{tableData.PowerWindow2AddPower}");
                UpdateShow();
            }
        }, LookAdsType.看广告获取体力);
    }

    private void AddPower3Btn_OnClick()
    {
        GlobalConfigCategory tableData = ConfigMgr.Instance.Tables.GlobalConfigCategory;

        PlatformMgr.Instance.LookAd((result) =>
        {
            if (result)
            {
                Save_CostPower saveData = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_CostPower>();
                saveData.AddLookInfiniteCostPowerAdCount();
                if (saveData.LookInfiniteCostPowerAdCount >= tableData.PowerWindow3AdsNum)
                {
                    saveData.ResetLookInfiniteCostPowerAdCount();
                    saveData.SetInfiniteCostPowerTime(tableData.PowerWindow3AddPower);
                    UIWindowTool.ShowPromptBox($"无限体力:{tableData.PowerWindow3AddPower / 60}分钟");
                }

                UpdateShow();
            }
        }, LookAdsType.看广告获取无限体力);
    }
}