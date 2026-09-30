using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using GameMain;
using UnityEngine;

public class AchievementWindow : ABaseWindow, IEnhancedScrollerDelegate
{
    public EnhancedScroller Scroller;

    public EnhancedScrollerCellView CellView;

    private List<AchievementProgress> AlLData = new();

    public AchievementTopNode TopNode;

    protected override void OnShow(object parameters)
    {
        Init();
        TopNode.Init();
    }

    private void Init()
    {
        AlLData = AchievementMgr.Instance.GetAchievementProgress();
        AlLData = AlLData.OrderByDescending(s =>
        {
            var config = ConfigMgr.Instance.Tables.AchievementCategory.Get(s.achievementConfigId);
            if (s.isClaim)
                return 0;
            if (s.isClaim == false && s.progress >= config.Parameters)
            {
                return 1000;
            }

            return 100;
        }).ToList();
        Scroller.Delegate = this;
        Scroller.ReloadData();
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return AlLData.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return CellView.transform.GetComponent<RectTransform>().rect.height;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        AchievementItem cellView = scroller.GetCellView(CellView) as AchievementItem;
        cellView.SetData(AlLData[dataIndex], ClickAction);
        return cellView;
    }

    private List<ItemData_BagProp> rewardDatas = new();

    private Dictionary<int,int> rewardDic = new();
    private void ClickAction(AchievementItem obj)
    {
        rewardDatas.Clear();
        rewardDic.Clear();
        foreach (var item in AlLData)
        {
            if (item.isClaim == false)
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