using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using GameMain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementTopItem : EnhancedScrollerCellView
{

    public RectTransform StartRect;
    public RectTransform MiddleRect;
    public RectTransform EndRect;
    public TextMeshProUGUI startNum;
    public GameObject startPoint;
    
    public TextMeshProUGUI MiddletNum;
    public GameObject MiddletPoint;
    public Image MiddletRewardImage;
    public TextMeshProUGUI MiddletRewardNum;
    public UICustomButton MiddletRewardButton;
    public GameObject midCollected;
    public Image midCollecteRewardImage;
    
    public TextMeshProUGUI EndNum;
    public GameObject EndPoint;
    public Image EndRewardImage;
    public TextMeshProUGUI EndRewardNum;
    public UICustomButton EndRewardButton;
    
    public GameObject endCollected;
    public Image endCollecteRewardImage;

    public AchievementProgress CurData;
    public void SetData(AchievementProgress data,int dataIndex,int count,Action<AchievementTopItem> OnEndRewardButtonClick)
    {
        CurData = data;
        MiddletRewardButton.AddListener(() =>
        {
            OnEndRewardButtonClick?.Invoke(this);
        });

        EndRewardButton.AddListener(() =>
        {
            OnEndRewardButtonClick?.Invoke(this); 
        });
        
        StartRect.gameObject.SetActive(dataIndex == 0);
        EndRect.gameObject.SetActive(dataIndex == count - 1);
        MiddleRect.gameObject.SetActive(dataIndex != 0 && dataIndex != count - 1);
        if (dataIndex == 0)
        {
            startNum.text = 0.ToString();
            startPoint.SetActive(true);
        }
        if (dataIndex != 0 && dataIndex != count - 1)
        {
            var config = ConfigMgr.Instance.Tables.AchievementCategory.Get(data.achievementConfigId);
            MiddletPoint.gameObject.SetActive(data.progress>= config.Parameters);
            MiddletNum.text = config.Parameters.ToString();
            var bagCofig = ConfigMgr.Instance.Tables.BagPropCategory.Get(config.TaskAward.FirstOrDefault().Key);
            MiddletRewardNum.text = $"x{config.TaskAward.FirstOrDefault().Value}";
            ResourceMgr.Instance.SetImageSprite(MiddletRewardImage, bagCofig.Sprite);
            MiddletRewardButton.gameObject.SetActive(data.isClaim==false && data.progress>= config.Parameters);
            midCollected.gameObject.SetActive(data.isClaim);
            ResourceMgr.Instance.SetImageSprite(midCollecteRewardImage, bagCofig.Sprite);
        }
        if (dataIndex == count - 1)
        {
            var config = ConfigMgr.Instance.Tables.AchievementCategory.Get(data.achievementConfigId);
            EndNum.text =config.Parameters.ToString();
            EndPoint.gameObject.SetActive(data.progress>= config.Parameters);
            var bagCofig = ConfigMgr.Instance.Tables.BagPropCategory.Get(config.TaskAward.FirstOrDefault().Key);
            EndRewardNum.text = $"x{config.TaskAward.FirstOrDefault().Value}";
            ResourceMgr.Instance.SetImageSprite(EndRewardImage, bagCofig.Sprite);
            EndRewardButton.gameObject.SetActive(data.isClaim==false && data.progress>= config.Parameters);
            endCollected.gameObject.SetActive(data.isClaim);
            ResourceMgr.Instance.SetImageSprite(endCollecteRewardImage, bagCofig.Sprite);
        }
    }
}
