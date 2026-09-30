using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameMain;
using GameMain.AOT;
using TMPro;
using UnityEngine;

public class EndlessModeBtn : MonoBehaviour
{
    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public GameMain.UICustomButton mBtnGame_EndlessMode;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public TextMeshProUGUI timesText;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public GameObject AdGo;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public TextMeshProUGUI OpenTipText;
    
    private int RemainingTimes;

    void Start()
    {
        mBtnGame_EndlessMode.AddListener(OnEnterEndlessGameClick);
        var changlleCount = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().currentChanglleCount;
        RemainingTimes = ConfigMgr.Instance.Tables.GlobalConfigCategory.GardenEndlessDailyFreeCount - changlleCount;
        AdGo.gameObject.SetActive(LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().levelId>= ConfigMgr.Instance.Tables.GlobalConfigCategory.GardenEndlessOpenNeedPassLevel && RemainingTimes <= 0);
        timesText.gameObject.SetActive(LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().levelId>= ConfigMgr.Instance.Tables.GlobalConfigCategory.GardenEndlessOpenNeedPassLevel&& RemainingTimes > 0);
        timesText.text =
            $"今日还剩{RemainingTimes}/{ConfigMgr.Instance.Tables.GlobalConfigCategory.GardenEndlessDailyFreeCount}";
        OpenTipText.gameObject.SetActive(LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().levelId< ConfigMgr.Instance.Tables.GlobalConfigCategory.GardenEndlessOpenNeedPassLevel);
        OpenTipText.text =$"第{ConfigMgr.Instance.Tables.GlobalConfigCategory.GardenEndlessOpenNeedPassLevel-1000}关解锁";
    }
    
    private void EnterEndlessGame()
    {
        switch (Init.Instance.miniGameType)
        {
            case EMiniGameType.GardenFlower:
                UIWindowTool.ChangeToGardenEndlessGame().Forget();
                break;
            case EMiniGameType.BlockFlower:
                UIWindowTool.ChangeToBlockEndlessGame().Forget();
                break;
            case EMiniGameType.LineFlower:
                break;
            default:
                Debugger.LogError($"未处理的游戏类型:{Init.Instance.miniGameType}");
                break;
        }
    }

    private void OnEnterEndlessGameClick()
    {
        if (LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().levelId< ConfigMgr.Instance.Tables.GlobalConfigCategory.GardenEndlessOpenNeedPassLevel)
        {
            UIWindowMgr.Instance.ShowPromptBox($"闯关模式第{ConfigMgr.Instance.Tables.GlobalConfigCategory.GardenEndlessOpenNeedPassLevel-1000}关解锁");
            return;
        }
        
        if (RemainingTimes > 0)
        {
            LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().AddChanglleCount();
            EnterEndlessGame();
        }
        else
        {
                PlatformMgr.Instance.LookAd((flag) =>
                {
                    if (flag)
                    {
                        EnterEndlessGame();
                    }
                },LookAdsType.看广告进入无线模式);
        }
       
    }
}