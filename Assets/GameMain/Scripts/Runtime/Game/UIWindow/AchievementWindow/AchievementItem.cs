using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using GameMain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementItem : EnhancedScrollerCellView
{
    public Image ResImage;
    public TextMeshProUGUI Num;
    public TextMeshProUGUI Desc;
    public UICustomButton ClaimBtn;
    public GameObject ClaimFlag;
    public Image ProgressValue;
    public TextMeshProUGUI ProgressText;

    public AchievementProgress CurData;

    private Action<AchievementItem> OnClickAction;
    public void SetData(AchievementProgress data,Action<AchievementItem> OnClick)
    {
        OnClickAction = OnClick;
        ClaimBtn.AddListener(OnClickClaim);
        CurData = data;
        if (data != null)
        {
            var config = ConfigMgr.Instance.Tables.AchievementCategory.Get(data.achievementConfigId);
            if (config != null)
            {
                var bagCofig = ConfigMgr.Instance.Tables.BagPropCategory.Get(config.TaskAward.FirstOrDefault().Key);
                Num.text = $"+{config.TaskAward.FirstOrDefault().Value}";
                ResourceMgr.Instance.SetImageSprite(ResImage, bagCofig.Sprite);
                Desc.text = string.Format(config.TaskType_Ref.TaskDesc, config.Parameters);
                ProgressValue.fillAmount = (float)data.progress / config.Parameters;
                var showNum= data.progress >= config.Parameters ? config.Parameters : data.progress;
                ProgressText.text = $"{showNum}/{config.Parameters}";
                ClaimFlag.gameObject.SetActive(data.isClaim);
                ClaimBtn.gameObject.SetActive(!data.isClaim && data.progress >= config.Parameters);
            }
        }
    }

    private void OnClickClaim()
    {
        if (CurData != null)
        {
            OnClickAction?.Invoke(this);
        }
    }
}