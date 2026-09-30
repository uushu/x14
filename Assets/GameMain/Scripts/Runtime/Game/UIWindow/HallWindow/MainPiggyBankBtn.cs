using System.Collections;
using System.Collections.Generic;
using GameMain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainPiggyBankBtn : MonoBehaviour
{
    public UICustomButton Btn;
    
    public Image ProgressImg;

    public TextMeshProUGUI ProgressText;

    public Transform OkFlag;
    
    public DoShakeRotation Shake;
    
    private EventGroup _eventGroup = new();
    void Start()
    {
        Btn.AddListener(OnClick);
        _eventGroup.AddListener<GameEventDefine.PiggyBankGoldChange>(ProcessMessage);
        UpdateUI();
        
    } 
    private void OnDestroy()
    {
        _eventGroup.RemoveAllListener();
    }
    private void ProcessMessage(IEventMessage obj)
    {
        UpdateUI();
    }
    private void OnClick()
    {
        UIWindowMgr.Instance.OpenWindowAsync<PiggyBankView>();
    }

    private void UpdateUI()
    {
        var PiggyBankTargetList = ConfigMgr.Instance.Tables.GlobalConfigCategory.PiggyBankTarget;
        var bank = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_PiggyBank>(true);
        var CurCliamNum = 0;
        var CurMax = 0;
        for (int i = PiggyBankTargetList.Count - 1; i>=0 && i < PiggyBankTargetList.Count; i--)
        {
            if (bank.CurPiggyBankMoney >= PiggyBankTargetList[i])
            {
                CurCliamNum = PiggyBankTargetList[i];
                CurMax= PiggyBankTargetList[i];
                break;
            }
            else
            {
                CurMax= PiggyBankTargetList[i];
            }
        }
        ProgressImg.fillAmount = bank.CurPiggyBankMoney / (float)CurMax;
        ProgressText.text= $"{ bank.CurPiggyBankMoney}/{CurMax}";
        OkFlag.gameObject.SetActive(CurCliamNum>0);
        if (CurCliamNum>0)
        {
            Shake.PlayAnim();
        }
        else
        {
            Shake.StopAnim();
        }
       
    }
}
