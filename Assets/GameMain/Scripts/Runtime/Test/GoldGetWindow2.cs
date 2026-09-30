using System.Collections;
using System.Collections.Generic;
using GameMain;
using UnityEngine;

public class GoldGetWindow2 : ABaseWindow
{
    public UICustomButton closeBtn;
    public UICustomButton imgMskBtn;
    
    
    
    protected override void OnShow(object parameters)
    {
        base.OnShow(parameters);
        closeBtn.AddListener(OnCloseBtnClick);
        imgMskBtn.AddListener(OnCloseBtnClick);
        
        //Debug.LogError("测试");
    }

    private void OnCloseBtnClick()
    {
        CloseSelfWindow();
    }
}
