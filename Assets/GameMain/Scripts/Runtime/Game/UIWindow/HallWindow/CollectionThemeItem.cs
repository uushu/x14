using System;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using GameMain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionThemeItem : EnhancedScrollerCellView
{
    public Image bg;
    public Image bg2;
    public Image icon;
    public TextMeshProUGUI nameText;
    public Image inUse;
    
    public UICustomSlider slider;
    public TextMeshProUGUI unlockText;

    public GameObject adsUnlockNode;
    public TextMeshProUGUI adsNumText;
    public TextMeshProUGUI unlockText2;
    
    public UICustomButton useBtn;
    public UICustomButton adsBtn;

    private CollectionThemePrefab handle;
    private CollectionThemeItemSaveData data;
    public void SetData(CollectionThemePrefab _handle,CollectionThemeItemSaveData _data)
    {
        handle = _handle;
        data = _data;
        UpdateShow();
        useBtn.AddListener(UseBtnOnClick);
        adsBtn.AddListener(AdsBtnOnClick);
    }

    void UseBtnOnClick()
    {
        LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Main>(true).UseTheme(data.tableId);
        handle?.UpdateShow(true);
        UpdateShow();
        UIWindowTool.ShowPromptBox("使用成功");
    }

    void AdsBtnOnClick()
    {
        if (data.tableData.UnlockType == 3)//看广告获取
        {
            PlatformMgr.Instance.LookAd((result) =>
            {
                if (result)
                {
                    LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Main>(true).LookAdForTheme(data.tableId,1);
                    handle?.UpdateShow(true);
                    UpdateShow();
                }
            }, LookAdsType.看广告解锁主题);
        }
    }
    
    public void UpdateShow()
    {
        nameText.text = data.tableData.TitleName;
        inUse.gameObject.SetActive(data.State == CollectionThemeState.InUse);
        (float press, float value, float maxValue) progress = data.GetUnlockPress();
        slider.SetProgressPercent(progress.value, 0, progress.maxValue);
        unlockText.text = string.Format(data.tableData.UnlockTip,progress.maxValue);
        unlockText2.text = string.Format(data.tableData.UnlockTip,progress.maxValue);
        adsNumText.text = $"{data.adsNum}/{data.tableData.UnlockNum}";
        ResourceMgr.Instance.SetImageSprite(bg, data.tableData.BgSpriteName);
        ResourceMgr.Instance.SetImageSprite(bg2, data.tableData.Bg2SpriteName);
        ResourceMgr.Instance.SetImageSprite(icon, data.tableData.IconSpriteName);
        
        slider.gameObject.SetActive(true);
        switch (data.State)
        {
            case CollectionThemeState.Lock:
                //广告解锁
                if (data.tableData.UnlockType == 3)
                {
                    slider.gameObject.SetActive(false);
                    adsUnlockNode.SetActive(true);
                }
                else
                {
                    adsUnlockNode.SetActive(false);
                }
                break;
            case CollectionThemeState.Unlock:
                slider.gameObject.SetActive(false);
                adsUnlockNode.SetActive(false);
                break;
            case CollectionThemeState.InUse:
                slider.gameObject.SetActive(false);
                adsUnlockNode.SetActive(false);
                break;
        }

        //广告遮罩按钮
        if (!slider.gameObject.activeSelf && !adsUnlockNode.activeSelf)
        {
            adsBtn.gameObject.SetActive(false);
        }
        else
        {
            adsBtn.gameObject.SetActive(true);
        }
    }
}
