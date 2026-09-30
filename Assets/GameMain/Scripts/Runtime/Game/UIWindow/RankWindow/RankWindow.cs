using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EnhancedUI.EnhancedScroller;
using GameMain;
using UnityEngine;

public class RankWindow : ABaseWindow, IEnhancedScrollerDelegate
{
    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public EnhancedScroller ScrollView;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public EnhancedScrollerCellView CellView;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public UICustomButton RanKBtn;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public UICustomButton EndlessBtn;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public UICustomButton AchievementBtn;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public RankItem selfRankItem;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public Transform FriendsOn;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public Transform FriendsOff;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public Transform EndlessOn;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public Transform EndlessOff;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public Transform Selecton;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public Transform levelNode;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public Transform pointNode;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public Transform AchievementpointNode;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public Transform AchievementOn;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public Transform AchievementOff;

    private int curSelect;

    private Dictionary<string, List<RankRoleInfo>> rankDataDic = new();
    private Dictionary<string, RankRoleInfo> selfDataDic = new();

    protected override void OnShow(object parameters)
    {
        rankDataDic.Clear();
        selfDataDic.Clear();
        var select = (int)parameters;
        RanKBtn.AddListener(OnRanKBtnClick);
        EndlessBtn.AddListener(OnEndlessToggleChange);
        AchievementBtn.AddListener(OnAchievementBtnClick);
        if (select == 1)
        {
            OnRanKBtnClick();
        }
        else if (select == 2)
        {
            OnEndlessToggleChange();
        }
        else
        {
            OnAchievementBtnClick();
        }
    }

    private void OnAchievementBtnClick()
    {
        if (curSelect != 3)
        {
            curSelect = 3;
            SetSelect(AchievementBtn.gameObject);
        }
    }


    private void OnEndlessToggleChange()
    {
        if (curSelect != 2)
        {
            curSelect = 2;
            SetSelect(EndlessBtn.gameObject);
        }
    }

    private void OnRanKBtnClick()
    {
        if (curSelect != 1)
        {
            curSelect = 1;
            SetSelect(RanKBtn.gameObject);
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
        FriendsOn.gameObject.SetActive(curSelect == 1);
        FriendsOff.gameObject.SetActive(curSelect != 1);
        EndlessOn.gameObject.SetActive(curSelect == 2);
        EndlessOff.gameObject.SetActive(curSelect != 2);
        levelNode.gameObject.SetActive(curSelect == 1);
        pointNode.gameObject.SetActive(curSelect == 2);
        AchievementpointNode.gameObject.SetActive(curSelect == 3);
        AchievementOn.gameObject.SetActive(curSelect == 3);
        AchievementOff.gameObject.SetActive(curSelect != 3);

        Selecton.transform.DOLocalMoveX(go.transform.localPosition.x, 0.2f);
        var rankname = GetRankname();
        if (rankDataDic.TryGetValue(rankname, out var _rankData))
        {
            ScrollView.Delegate = this;
            ScrollView.ReloadData();
            selfRankItem.SetData(selfDataDic[rankname], curSelect);
            return;
        }

        GetRankData(rankname, (_rankData, _selfData) =>
        {
            rankDataDic[rankname] = _rankData;
            selfDataDic[rankname] = _selfData;
            _selfData.isSelf = true;
            selfRankItem.SetData(selfDataDic[rankname], curSelect);
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

public class RankRoleInfo
{
    public string uuid;
    public string rank;
    public string name;
    public int titleId;
    public string iconUrl;
    public int level;

    public bool isSelf;
}