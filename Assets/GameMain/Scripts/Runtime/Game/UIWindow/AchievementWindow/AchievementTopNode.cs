using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using GameMain;
using TMPro;
using UnityEngine;

public class AchievementTopNode : MonoBehaviour,IEnhancedScrollerDelegate
{
    public TextMeshProUGUI CurPoint;
    
    public EnhancedScroller TopScroller;

    public EnhancedScrollerCellView TopCellView;

    
    private List<AchievementProgress> AlLData = new ();
    private EventGroup _eventGroup = new();
    private void Awake()
    {
        _eventGroup.AddListener<GameEventDefine.GetBagProp>(ProcessMessage);
        _eventGroup.AddListener<GameEventDefine.AchievementProgressChange>(ProcessMessage);
    }

    private void OnDestroy()
    {
        _eventGroup.RemoveAllListener();
    }

    private void ProcessMessage(IEventMessage obj)
    {
        if (obj is GameEventDefine.GetBagProp)
        {
            CurPoint.text = BagMgr.Instance.GetBagPropCount(GameTool.AchievementPintId).ToString();
        }
        else if (obj is GameEventDefine.AchievementProgressChange)
        {
            Init();
        }
    }


    public void Init()
    {
        AlLData = AchievementMgr.Instance.GetAchievementTopProgress();
        TopScroller.Delegate = this;
        var index = 0;
        AlLData.Insert(0,new AchievementProgress());
       var findit=  AlLData.FirstOrDefault(s =>
        {
            if (s.achievementConfigId>0)
            {
                var config= ConfigMgr.Instance.Tables.AchievementCategory.Get(s.achievementConfigId);
                if (s.isClaim==false&&config!=null)
                {
                    return s.progress >= config.Parameters;
                }
            }
            return false;
        });
        if (findit!=null)
        {
            index= AlLData.IndexOf(findit);
        }
        else
        {
            var findit2=  AlLData.FirstOrDefault(s =>
            {
                if (s.achievementConfigId > 0)
                {
                    var config=  ConfigMgr.Instance.Tables.AchievementCategory.Get(s.achievementConfigId);
                    if (s.isClaim==false&&config!=null)
                    {
                        return s.progress < config.Parameters;
                    }
                }
                return false;
            });
            index= AlLData.IndexOf(findit2);
        } 
        TopScroller.ReloadData();
        TopScroller.JumpToDataIndex(index,0f,0f,true,EnhancedScroller.TweenType.immediate,0f,null,EnhancedScroller.LoopJumpDirectionEnum.Closest,true);
        //TopScroller.RefreshActiveCellViews();
        
        CurPoint.text = BagMgr.Instance.GetBagPropCount(GameTool.AchievementPintId).ToString();
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
      return AlLData.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        if (dataIndex==0)
        {
            return TopCellView.transform.GetComponent<AchievementTopItem>().StartRect.rect.width;
        }

        if (dataIndex==AlLData.Count-1)
        {
            return TopCellView.transform.GetComponent<AchievementTopItem>().EndRect.rect.width;
        }
        return TopCellView.transform.GetComponent<AchievementTopItem>().MiddleRect.rect.width;

    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        AchievementTopItem cellView = scroller.GetCellView(TopCellView) as AchievementTopItem;
        cellView.SetData(AlLData[dataIndex],dataIndex,AlLData.Count,OnRewardButtonClick);
        return cellView;
    }
    private List<ItemData_BagProp> rewardDatas = new();
    private Dictionary<int,int> rewardDic = new();
    private void OnRewardButtonClick(AchievementTopItem obj)
    {
        rewardDatas.Clear();
        rewardDic.Clear();
        foreach (var item in AlLData)
        {
            if (item.isClaim == false&&item.achievementConfigId>0)
            {
                var config = ConfigMgr.Instance.Tables.AchievementCategory.Get(item.achievementConfigId);
                if (config != null && item.progress >= config.Parameters)
                {
                    AchievementMgr.Instance.SetAchievementCompleted(item);
                    foreach (var reward in config.TaskAward)
                    {
                        if (rewardDic.ContainsKey(reward.Key))
                        {
                            rewardDic[reward.Key] += reward.Value;
                        }
                        else
                        {
                            rewardDic.Add(reward.Key, reward.Value);
                        }
                    }
                }
            }
        }
        foreach (var reward in rewardDic)
        {
            rewardDatas.Add(new ItemData_BagProp(reward.Key,reward.Value) );
        }
        if (rewardDatas.Count > 0)
        {   BagMgr.Instance.AddBagProp(rewardDatas);
            UIWindowMgr.Instance.OpenWindowAsync<CommGetView>(rewardDatas);
            GameEventDefine.SendAchievementProgressChange();
            Init();
        }
    }
}
