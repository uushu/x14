using System.Collections;
using System.Collections.Generic;
using GameMain;
using UnityEngine;

public class CollectWindow : ABaseWindow
{
    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public UICustomButton goBtn;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public UICustomButton getBtn;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public GameObject isGetObj;

    private ItemData_BagProp temp;

    protected override void OnShow(object parameters)
    {
        base.OnShow(parameters);
        goBtn.AddListener(Go_OnClick);
        getBtn.AddListener(Get_OnClick);
        UpdateShow();
    }

    public void UpdateShow()
    {
        bool isGet = MainDataManager.Instance.IsGetCollectAward();
        if (isGet)
        {
            goBtn.gameObject.SetActive(false);
            getBtn.gameObject.SetActive(false);
            isGetObj.SetActive(true);
        }
        else if (PlatformMgr.IsMiniGameEntrance) //没有获得并小程序进入
        {
            goBtn.gameObject.SetActive(false);
            getBtn.gameObject.SetActive(true);
            isGetObj.SetActive(false);
        }
        else //没有获得并非小程序进入
        {
            goBtn.gameObject.SetActive(true);
            getBtn.gameObject.SetActive(false);
            isGetObj.SetActive(false);
        }

        List<int> data = ConfigMgr.Instance.Tables.GlobalConfigCategory.CollectAwardData;
        temp = new ItemData_BagProp(data[0], data[1]);
        //itemUI_BagProp.RefreshBagPropUI(temp);*/
    }

    private void Go_OnClick()
    {
        PlatformMgr.Instance.AddToMiniGameEntrance((result) =>
        {
            if (result)
            {
                PlatformMgr.IsMiniGameEntrance = true;
                UpdateShow();
            }
        });
    }

    private void Get_OnClick()
    {
        MainDataManager.Instance.SetGetCollectAward();
        List<ItemData_BagProp> list = new();
        list.Add(temp);
        BagMgr.Instance.AddBagProp(list);
        UIWindowMgr.Instance.OpenWindowAsync<CommGetView>(list);
    
        CloseSelfWindow();
    }
}