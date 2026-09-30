using System.Collections;
using System.Collections.Generic;
using cfg;
using GameMain;
using UnityEngine;

public class MainDataManager : ASingleBase<MainDataManager>
{
    
    public override void Initialize()
    {
        base.Initialize();
    }
    /// <summary>
    /// 是否获得收藏奖励
    /// </summary>
    public bool IsGetCollectAward()
    {
        return LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Main>().HaveGetCollectAward;
    }
    
    /// <summary>
    /// 设置已经获得收藏奖励
    /// </summary>
    public void SetGetCollectAward()
    {
        LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Main>(true).HaveGetCollectAward = true;
    }
    
    /// <summary>
    /// 记录已领取邀请奖励 
    /// </summary>
    public void AddGetInvite(int tableId)
    {
        LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Main>(true).AddGetInvite(tableId);
    }
    /// <summary>
    /// 是否已经领取过邀请奖励
    /// </summary>
    public bool IsGetInvite(int tableId)
    {
        return  LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Main>(true).IsGetInvite(tableId);
    }
    
    /// <summary>
    /// 记录已邀请
    /// </summary>
    public void AddInviteCount(int count)
    {
        LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Main>(true).AddInviteCount(count);
    }
    /// <summary>
    /// 已经邀请人数
    /// </summary>
    public int GetInviteCount()
    {
        return  LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Main>(true).GetInviteCount();
    }
}
