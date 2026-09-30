using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cfg;
using EnhancedUI.EnhancedScroller;
using GameMain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : EnhancedScrollerCellView
{
    public Transform TitleNode;
    public Transform LookAdNode;
    public Transform BuyNode;


    public Image LookRewardImage;
    public TextMeshProUGUI LookRewardNum;
    public TextMeshProUGUI LookAdCount;
    public Image RewardImage;
    public TextMeshProUGUI RewardName;
    public UICustomButton BuyButton;
    public UICustomButton LookAdButton;

    public Image CostGoldImage;
    public TextMeshProUGUI CostGoldText;

    public List<ItemUI_BagProp> RewardTf;
    
    private void Awake()
    {
        LookAdButton.AddListener(OnClickLookAd);
        BuyButton.AddListener(OnClickBuy);
    }

    private void OnClickBuy()
    {
        if (shopConfig != null && shopConfig.ShopType == 2)
        {
            if (BagMgr.Instance.BagPropEnough(1, shopConfig.CoinPrice))
            {
                BagMgr.Instance.RemoveBagProp(1, shopConfig.CoinPrice);
                AddItem();
                RecordMgr.Instance.ReportBuyItem(shopConfig.Name);
            }
            else
            {
                UIWindowMgr.Instance.ShowPromptBox("金币不足");
            }
        }
    }

    private void OnClickLookAd()
    {
        if (shopConfig != null && shopConfig.ShopType == 1)
        {
            PlatformMgr.Instance.LookAd((flag) =>
            {
                if (flag)
                {
                    var looadCount = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Shop>(true)
                        .getShopItemLookAdCount(shopConfig.Id);
                    looadCount.Count++;
                    if (looadCount.Count >= shopConfig.CoinPrice)
                    {
                        AddItem();
                        looadCount.Count = 0;
                        LookAdCount.text = looadCount.Count + "/" + shopConfig.CoinPrice;
                    }
                    else
                    {
                        LookAdCount.text = looadCount.Count + "/" + shopConfig.CoinPrice;
                    }
                }
            }, LookAdsType.看广告领取商店金币);
        }
    }

    private void AddItem()
    {
        List<ItemData_BagProp> list = new List<ItemData_BagProp>();
        foreach (var item in shopConfig.ShopItemId)
        {
            list.Add(new ItemData_BagProp(item.Key, item.Value));
        }
        BagMgr.Instance.AddBagProp(list);
        UIWindowMgr.Instance.OpenWindow<CommGetView>(list);
    }

    private ShopConfig shopConfig;

    public void SetData(int data)
    {
        TitleNode.gameObject.SetActive(data == -1);
        if (data > 0)
        {
            shopConfig = ConfigMgr.Instance.Tables.ShopCategory.Get(data);
            RewardName.text = shopConfig.Name;
            ResourceMgr.Instance.SetImageSprite(LookRewardImage, shopConfig.Res, true);
            LookRewardNum.text = $"x{shopConfig.ShopItemId.GetValueOrDefault(1)}";
            ResourceMgr.Instance.SetImageSprite(RewardImage, shopConfig.Res, true);
            ResourceMgr.Instance.SetImageSprite(CostGoldImage, "nuohua_zjm_jinbi", false);
            LookAdNode.gameObject.SetActive(data != -1 && shopConfig.ShopType == 1);
            BuyNode.gameObject.SetActive(data != -1 && shopConfig.ShopType == 2);
            CostGoldText.text = shopConfig.CoinPrice.ToString();
            LookAdCount.gameObject.SetActive(shopConfig.ShopType == 1&& shopConfig.CoinPrice>1);
            if (shopConfig.ShopType == 1)
            {
                LookAdCount.text = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Shop>().getShopItemLookAdCount(shopConfig.Id).Count + "/" + shopConfig.CoinPrice;
            }

            if (shopConfig.ShopType==2)
            {
               var allkey= shopConfig.ShopItemId.Keys.ToList();
                for (int i = 0; i < RewardTf.Count; i++)
                {
                    RewardTf[i].gameObject.SetActive(i<shopConfig.ShopItemId.Count);
                    if (i<allkey.Count)
                    {
                        RewardTf[i].RefreshBagPropUI(new ItemData_BagProp(allkey[i],shopConfig.ShopItemId[allkey[i]]));
                    }
                }
            }
        }
        else
        {
            LookAdNode.gameObject.SetActive(false);
            BuyNode.gameObject.SetActive(false);
        }
    }
}