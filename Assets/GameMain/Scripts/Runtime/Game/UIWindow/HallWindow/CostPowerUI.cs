using System;
using System.Collections;
using System.Collections.Generic;
using GameMain;
using TMPro;
using UnityEngine;

public class CostPowerUI : MonoBehaviour
{
    public UICustomButton btn;
    public TextMeshProUGUI powerCount;
    public TextMeshProUGUI powerTime;

    private float time;
    private float timeInterval = 0.02f;

    private void Awake()
    {
        btn.AddListener(OnClick);
    }

    public void OnEnable()
    {
        time = 0;
    }

    public void Update()
    {
        time += Time.unscaledDeltaTime;
        if (time >= timeInterval)
        {
            time = 0;
            Save_CostPower temp = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_CostPower>();
            bool isMaxPower = temp.IsMaxPower();
            if (!isMaxPower)
            {
                temp.CheckRecoveryCostPower();
            }
            
            DateTime nowTime = DateTime.Now;
            DateTime lastTime = CommonTool.TimestampConvertToDate(temp.LastGetPowerTime);
            TimeSpan ts = nowTime - lastTime;
           // powerCount.text = $"{temp.CurrentPower}/{ConfigMgr.Instance.Tables.GlobalConfigCategory.MaxCostPower}";
            powerCount.text = $"{temp.CurrentPower}";
            int remnantSeconds = (int)(temp.RecoveryInterval - ts.TotalSeconds);
            TimeSpan tsRemnant = TimeSpan.FromSeconds(remnantSeconds);
            if (temp.IsInfiniteCostPower)
            {
                powerCount.text = "无限";
                Save_CostPower data = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_CostPower>();
                DateTime lastInfiniteCostPowerStartTime = CommonTool.TimestampConvertToDate((long)data.LastInfiniteCostPowerStartTime);
                lastInfiniteCostPowerStartTime = lastInfiniteCostPowerStartTime.AddSeconds(data.InfiniteCostPowerTime);
                
                TimeSpan downTime = lastInfiniteCostPowerStartTime - DateTimeOffset.Now.LocalDateTime;
                powerTime.text = downTime.ToString(@"hh\:mm\:ss");//倒计时
            }
            else
            {
                powerTime.text = isMaxPower ? "已满" : tsRemnant.ToString(@"hh\:mm\:ss");
            }
        }
    }

    private void OnClick()
    {
        UIWindowMgr.Instance.OpenWindow<PowerWindow>(false);
    }
}