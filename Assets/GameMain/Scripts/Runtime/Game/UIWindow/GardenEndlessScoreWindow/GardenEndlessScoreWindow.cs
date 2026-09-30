using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMain;
using TMPro;
using UnityEngine;

public class GardenEndlessScoreWindow : ABaseWindow
{
    public UICustomButton okRestBtn;
    public UICustomButton backBtn;
    public TextMeshProUGUI CurPoinsText;
    public TextMeshProUGUI MaxPoinsText;
    public UICustomButton ShareBtn;
    public GameObject GeneralGo;
    public GameObject BestGo;
    public TextMeshProUGUI BestPoinsText;
    
    public GameObject adGo;
    
    public Action ComeagainAction;
    int count;
    protected override void OnShow(object parameters)
    {
        AudioMgr.Instance.PlayAudioOneShot(AudioName.VictoryAudio);
        backBtn.AddListener(OnBackClick);
        okRestBtn.AddListener(OnOkRestClick);
        ShareBtn.AddListener(OnShareClick);
        CurPoinsText.text = 0.ToString();
        var isBest=LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().endlessIsNewRecord;
        GeneralGo.gameObject.SetActive(!isBest);
        BestGo.gameObject.SetActive(isBest);
        
        AudioMgr.Instance.PlayAudioOneShot(AudioName.MaisaiketiaodongAudio);
        DOTween.To(value => { CurPoinsText.text = Mathf.Floor(value).ToString(); }, startValue: 0,
            endValue: LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().leastBestScore, duration: 1f);
        BestPoinsText.text = 0.ToString();
        DOTween.To(value => { BestPoinsText.text = Mathf.Floor(value).ToString(); }, startValue: 0,
            endValue: LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().endlessBestScore, duration: 1f);
        
        MaxPoinsText.text = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().endlessBestScore
            .ToString();
        count = ConfigMgr.Instance.Tables.GlobalConfigCategory.GardenEndlessDailyFreeCount - LocalSaveMgr.Instance
            .DataCenter.GetLocalSave<Save_FlowerGarden>().currentChanglleCount;
        adGo.gameObject.SetActive(count<=0);
    }

    private void OnShareClick()
    {
        PlatformMgr.Instance.ShowShare("快来、快来、根本停不下来！！！",(flag) =>
        {
        });
    }

    private void OnOkRestClick()
    {
        if (count>0)
        {
            LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().AddChanglleCount();
            ComeagainAction?.Invoke();
        }
        else
        {
            PlatformMgr.Instance.LookAd((flag) =>
            {
                if (flag)
                {
                    ComeagainAction?.Invoke();
                }
            },LookAdsType.看广告进入无线模式);
        }
        CloseSelfWindow();
    }

    private void OnBackClick()
    {
        CloseSelfWindow();
        UIWindowTool.ChangeToHallScene(false);
    }
}