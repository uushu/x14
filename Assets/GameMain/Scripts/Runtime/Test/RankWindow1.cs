using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using DG.Tweening;
using EnhancedUI.EnhancedScroller;
using GameMain;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class RankWindow1 : ABaseWindow,IEnhancedScrollerDelegate
{
    [FoldoutGroup("组件")] public EnhancedScroller ScrollView;
    [FoldoutGroup("组件")] public EnhancedScrollerCellView CellView;
    [FoldoutGroup("组件")] public UICustomButton RankBtn;
    [FoldoutGroup("组件")] public UICustomButton EndlessBtn;
    [FoldoutGroup("组件")] public UICustomButton AchieveBtn;

    [FoldoutGroup("组件")] public Transform RankBtnOn;
    [FoldoutGroup("组件")] public Transform RankBtnOff;
    
    [FoldoutGroup("组件")] public Transform EndlessBtnOn;
    [FoldoutGroup("组件")] public Transform EndlessBtnOff;
    
    [FoldoutGroup("组件")] public Transform AchieveBtnOn;
    [FoldoutGroup("组件")] public Transform AchieveBtnOff;

    [FoldoutGroup("组件")] public Transform Selecton;
    [FoldoutGroup("组件")] public Transform RankNode;
    [FoldoutGroup("组件")] public Transform LevelNode;
    [FoldoutGroup("组件")] public Transform PointNode;
    [FoldoutGroup("组件")] public Transform AchieveNode;

    
    [FoldoutGroup("组件")] public RankItem RankItemPrefab;
    

    private int curSelect;

    private Dictionary<string, List<RankRoleInfo>> rankDataDic = new();
    private Dictionary<string,RankRoleInfo> selfDataDic= new();

    protected override void OnShow(object parameters)
    {
        base.OnShow(parameters);
        rankDataDic.Clear();
        selfDataDic.Clear();
        var select=(int) parameters;
        RankBtn.AddListener(OnRankBtn);
        EndlessBtn.AddListener(OnEndlessBtn);
        AchieveBtn.AddListener(OnAchieveBtn);

        if (select == 1)
        {
            OnRankBtn();
        }
        else if (select == 2)
        {
            OnEndlessBtn();
        }
        else 
        {
            OnAchieveBtn();
        }
    }

    private void OnAchieveBtn()
    {
        if (curSelect != 3)
        {
            curSelect = 3;
            SetSelect(AchieveBtn.gameObject);
        }
    }

   

    private void OnEndlessBtn()
    {
        if (curSelect != 2)
        {
            curSelect = 2;
            SetSelect(EndlessBtn.gameObject);
        }
    }

    private void OnRankBtn()
    {
        if (curSelect != 1)
        {
            curSelect = 1;
            SetSelect(RankBtn.gameObject);
        }
    }

    public async void GetRankData(string RankName, Action<List<RankRoleInfo>, RankRoleInfo> overCallBack)
    {
        List<RankRoleInfo> allData = new List<RankRoleInfo>();
        RankRoleInfo tempSelfData = null;
        Dictionary<string, RankRoleInfo> rankRoleDic = new Dictionary<string, RankRoleInfo>();

        HyzRankManager.Instance.GetRank(RankName, (resultAllData) =>
        {
            for (int i = 0; i < resultAllData.Count; i++)
            {
                var rankInfo = resultAllData[i];
                RankRoleInfo data = new RankRoleInfo()
                {
                    uuid = rankInfo.uuid,
                    rank = rankInfo.rank,
                    name = rankInfo.name,
                    iconUrl = rankInfo.iconUrl,
                    titleId = rankInfo.titleId,
                    level = rankInfo.level,
                };
                allData.Add(data);
                rankRoleDic[rankInfo.uuid] = data;
                //Debug.LogError($"排名数据:{data[i].uid},{data[i].num},{data[i].rankData}");
            }

            if (rankRoleDic.ContainsKey(PlatformMgr.UserId))
            {
                tempSelfData = rankRoleDic[PlatformMgr.UserId];
                rankRoleDic[PlatformMgr.UserId].name = PlatformMgr.NickName;
                rankRoleDic[PlatformMgr.UserId].iconUrl = PlatformMgr.IconUrl;
            }

            int myLocalScore;
            int myTitleId;
            Save_FlowerGarden localSave = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>(true);
            if (localSave != null)
            {
                myLocalScore = curSelect == 1 ? localSave.levelId : localSave.endlessBestScore;
                myTitleId = curSelect == 2 ? localSave.myTitleId != 0 ? localSave.myTitleId : 1 : 1;
            }
            else
            {
                myLocalScore = 0;
                myTitleId = 1;
            }

            tempSelfData = new RankRoleInfo()
            {
                rank = tempSelfData == null ? "100+" : tempSelfData.rank,
                iconUrl = PlatformMgr.IconUrl,
                name = PlatformMgr.NickName,
                titleId = tempSelfData?.titleId ?? myTitleId,
                level = tempSelfData?.level ?? myLocalScore,
            };
            overCallBack?.Invoke(allData, tempSelfData);
        }, () => { UIWindowTool.ShowPromptBox("获取排行榜失败"); });
    }

    public void SetSelect(GameObject go)
    {
        RankBtnOn.gameObject.SetActive(curSelect == 1);
        RankBtnOff.gameObject.SetActive(curSelect != 1);
        EndlessBtnOn.gameObject.SetActive(curSelect == 2);
        EndlessBtnOff.gameObject.SetActive(curSelect != 2);
        AchieveBtnOn.gameObject.SetActive(curSelect == 3);
        AchieveBtnOff.gameObject.SetActive(curSelect != 3);

        LevelNode.gameObject.SetActive(curSelect == 1);
        PointNode.gameObject.SetActive(curSelect == 2);
        AchieveNode.gameObject.SetActive(curSelect == 3);
        
        Selecton.transform.DOLocalMoveX(go.transform.localPosition.x, 0.2f);
        var rankname = GetRankname();
        if (rankDataDic.TryGetValue(rankname, out var _rankData))
        {
            ScrollView.Delegate = this;
            ScrollView.ReloadData();
            RankItemPrefab.SetData(selfDataDic[rankname], curSelect);
            return;
        }

        GetRankData(rankname, (_rankData, _selfData) =>
        {
            rankDataDic[rankname] = _rankData;
            selfDataDic[rankname] = _selfData;
            _selfData.isSelf = true;
            RankItemPrefab.SetData(selfDataDic[rankname], curSelect);
            ScrollView.Delegate = this;
            ScrollView.ReloadData();
        });
    }

    private string GetRankname()
    {
        string rankname = string.Empty;
        switch (curSelect)
        {
            case 1:
                rankname = GameEntrance.SupperLevelRankName;
                break;
            case 2:
                rankname = GameEntrance.EndlessBestRankName;
                break;
            case 3:
                rankname = GameEntrance.AchievementRankName;
                break;
        }

        return rankname;
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        var rankname = GetRankname();
        rankDataDic.TryGetValue(rankname, out var _rankData);
        return _rankData != null ? _rankData.Count : 0;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return CellView.GetComponent<RectTransform>().rect.height;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        RankItem cellView = scroller.GetCellView(CellView) as RankItem;
        cellView.gameObject.SetActive(true);
        cellView.name = "Cell " + dataIndex.ToString();
        var rankname = GetRankname();
        rankDataDic.TryGetValue(rankname, out var _rankData);
        cellView.SetData(_rankData[dataIndex], curSelect);
        return cellView;
    }
}

