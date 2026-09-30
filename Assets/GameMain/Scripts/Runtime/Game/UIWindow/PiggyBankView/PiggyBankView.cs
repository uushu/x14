using System.Collections;
using System.Collections.Generic;
using GameMain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PiggyBankView : ABaseWindow
{
    public UICustomButton CliamBtn;

    public TextMeshProUGUI CliamTargetNum;

    public TextMeshProUGUI TipText;
    
    public List<TextMeshProUGUI> CliamTargetListText;

    public Image ProgressImage;
    private long CurCliamNum = 0;

    protected override void OnShow(object parameters)
    {
        var PiggyBankTargetList = ConfigMgr.Instance.Tables.GlobalConfigCategory.PiggyBankTarget;
       
        TipText.text = $"通关获得金币,收集{PiggyBankTargetList[0]}枚金币可打开小猪存钱罐";
        for (int i = 0; i < CliamTargetListText.Count; i++)
        {
            CliamTargetListText[i].gameObject.SetActive(i < PiggyBankTargetList.Count);
            if (i < PiggyBankTargetList.Count)
            {
                CliamTargetListText[i].text = PiggyBankTargetList[i].ToString();
            }
        }
        UpdateUI(PiggyBankTargetList);
        CliamBtn.AddListener(OnClickCliam);
    }

    private void UpdateUI(List<int> PiggyBankTargetList )
    {
        var bank = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_PiggyBank>(true);
        CurCliamNum = 0;
        for (int i = PiggyBankTargetList.Count - 1; i>=0 && i < PiggyBankTargetList.Count; i--)
        {
            if (bank.CurPiggyBankMoney >= PiggyBankTargetList[i])
            {
                CurCliamNum = PiggyBankTargetList[i];
                break;
            }
        }

        ProgressImage.fillAmount = bank.CurPiggyBankMoney / (float)PiggyBankTargetList[PiggyBankTargetList.Count - 1];
        CliamTargetNum.text = CurCliamNum.ToString();
    }

    private void OnClickCliam()
    {
        if (CurCliamNum > 0)
        {
            PlatformMgr.Instance.LookAd((flag) =>
            {
                if (flag)
                {
                    var GoldNum = CurCliamNum;
                    //领取奖励
                    var bank = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_PiggyBank>(true);
                    BagMgr.Instance.AddBagProp(GameTool.ItemGoldId, CurCliamNum);
                    bank.RestPiggyBankMoney();
                    List<ItemData_BagProp> rewardDatas= new List<ItemData_BagProp>();
                    rewardDatas.Add(new ItemData_BagProp(GameTool.ItemGoldId,CurCliamNum) );
                    UIWindowMgr.Instance.OpenWindowAsync<CommGetView>(rewardDatas);
                    UpdateUI(ConfigMgr.Instance.Tables.GlobalConfigCategory.PiggyBankTarget);
                }
            },LookAdsType.看广告领取存钱罐金币);
        }
        else
        {
            UIWindowMgr.Instance.ShowPromptBox("当前可领取数量为0");
        }
    }
}