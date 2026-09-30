using System.Collections;
using System.Collections.Generic;
using GameMain;
using UnityEngine;

public class CommGetView : ABaseWindow
{
    public List<RewardItem> RewardTf;
    protected override void OnShow(object parameters)
    {
        AudioMgr.Instance.PlayAudioOneShot(AudioName.GetAwardAudio);
        List<ItemData_BagProp> rewardDatas =(List<ItemData_BagProp>) parameters;
        for (int i = 0; i < RewardTf.Count; i++)
        {
            RewardTf[i].gameObject.SetActive(i<rewardDatas.Count);
            if (i<rewardDatas.Count)
            {
                RewardTf[i].SetData(rewardDatas[i]);
            }
        }
    }
}