using System.Collections;
using System.Collections.Generic;
using cfg;
using GameMain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvitationWindow : ABaseWindow
{
    public Image RewardImage;
    public TextMeshProUGUI RewardNumText;
    public TextMeshProUGUI RewardNameText;

    public TextMeshProUGUI InvitationTargetText;

    public Image InvitationProgress;

    public UICustomButton RefreshBtn;
    public UICustomButton InvitationBtn;
    public UICustomButton ClaimBtn;
  
    protected InvitationConfig data;
    List<ItemData_BagProp> taskAward = new List<ItemData_BagProp>();

    private int inviteCount;
    protected override void OnShow(object parameters)
    {
        RefreshBtn.AddListener(OnClickRefreshBtn);
        InvitationBtn.AddListener(OnClickInvitationBtn);
        ClaimBtn.AddListener(OnClickClaimBtn);
        HyzShareManager.Instance.GetMyShareInfo(HyzShareManager.SharePosType.InvitePos, (result) =>
        {
            UIWindowTool.CloseMaskWindow();
            inviteCount = result.num;
            UpdateShow();
            Debugger.Log($"获取分享信息成功:{inviteCount}");
        }, () =>
        {
            UIWindowTool.CloseMaskWindow();
            UpdateShow();
            Debugger.Log("获取分享信息失败");
        });
    }

    
    public void UpdateShow()
    
    {
        List<InvitationConfig> all = ConfigMgr.Instance.Tables.InvitationCategory.DataList;
        InvitationConfig firstNoGet = null;
        for (int i = 0; i < all.Count; i++)
        {
            if (!MainDataManager.Instance.IsGetInvite(all[i].Id))
            {
                firstNoGet = all[i];
                break;
            }
        }

        data = firstNoGet;
        bool isShowOver = false;
        if (data == null)
        {
            //已领取所有邀请奖励
            //ShowOver();
            //return;
            isShowOver = true;
            data = all[all.Count - 1];
        }


        taskAward.Clear();
        foreach (var item in data.TaskAward)
        {
            taskAward.Add(new(item.Key, item.Value));
        }

        //int inviteCount = MainUIDataManager.Instance.GetInviteCount();

        
        InvitationProgress.fillAmount= inviteCount / (float) data.Count;
        InvitationTargetText.text = $"{Mathf.Max(0, data.Count - inviteCount)}";
        if (taskAward.Count > 0)
        {
            RewardNameText.text = taskAward[0].Config.Name;
            RewardNumText.text = $"x{taskAward[0].PropCountString}";
            ResourceMgr.Instance.SetImageSprite(RewardImage, taskAward[0].Config.Sprite);
        }
     
        InvitationBtn.gameObject.SetActive(inviteCount < data.Count);
        ClaimBtn.gameObject.SetActive(inviteCount >= data.Count);
        if (isShowOver)
        {
            InvitationBtn.gameObject.SetActive(true);
            ClaimBtn.gameObject.SetActive(false); 
        }
    }
    
    
    private void OnClickClaimBtn()
    {
        MainDataManager.Instance.AddGetInvite(data.Id);
        UIWindowMgr.Instance.OpenWindowAsync<CommGetView>(taskAward);
        UpdateShow();
    }

    private void OnClickInvitationBtn()
    {
        PlatformMgr.Instance.DirectedShare(0, (result) =>
        {
            if (result)
            {
                ReportManager.Instance.MainShare(true);
                UIWindowTool.ShowPromptBox("邀请成功发送");
                //MainUIDataManager.Instance.AddInviteCount(1);
                UIWindowTool.ShowMaskWindow(0.5f);
                HyzShareManager.Instance.GetMyShareInfo(HyzShareManager.SharePosType.InvitePos, (result) =>
                {
                    UIWindowTool.CloseMaskWindow();
                    inviteCount = result.num;
                    UpdateShow();
                    Debugger.Log($"获取分享信息成功:{inviteCount}");
                }, () =>
                {
                    UIWindowTool.CloseMaskWindow();
                    UpdateShow();
                    Debugger.Log("获取分享信息失败");
                });
            }
        });
    }

    private void OnClickRefreshBtn()
    {
        UIWindowTool.ShowMaskWindow(0.5f);
        HyzShareManager.Instance.GetMyShareInfo(HyzShareManager.SharePosType.None, (result) =>
        {
            UIWindowTool.CloseMaskWindow();
            inviteCount = result.num;
            UpdateShow();
            Debugger.Log($"获取分享信息成功:{inviteCount}");
        }, () =>
        {
            UIWindowTool.CloseMaskWindow();
            UpdateShow();
            Debugger.Log("获取分享信息失败");
        });
    }
}
