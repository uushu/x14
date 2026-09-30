using System.Collections;
using System.Collections.Generic;
using GameMain;
using TMPro;
using UnityEngine;

public class GoldGetWindow : ABaseWindow
{
    public TextMeshProUGUI Num;
    public UICustomButton OkBtn;
    protected override void OnShow(object parameters)
    {
        OkBtn.AddListener(OnOkBtnClick);
        ConfigMgr.Instance.Tables.GlobalConfigCategory.GetBagItem.TryGetValue(GameTool.ItemGoldId, out var bagItem);
        Num.text = $"x{bagItem}";
    }

    private void OnOkBtnClick()
    {
        PlatformMgr.Instance.LookAd((flag) =>
        {
            ConfigMgr.Instance.Tables.GlobalConfigCategory.GetBagItem.TryGetValue(GameTool.ItemGoldId, out var bagItem);
            List<ItemData_BagProp> rewardDatas= new List<ItemData_BagProp>();
            rewardDatas.Add(new ItemData_BagProp(GameTool.ItemGoldId, bagItem));
            BagMgr.Instance.AddBagProp(rewardDatas);
            UIWindowMgr.Instance.OpenWindowAsync<CommGetView>(rewardDatas);
        },LookAdsType.看广告领取主界面金币);
    }
}
