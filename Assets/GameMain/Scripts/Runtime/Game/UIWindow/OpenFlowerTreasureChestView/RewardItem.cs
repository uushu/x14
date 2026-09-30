using System.Collections;
using System.Collections.Generic;
using GameMain;
using UnityEngine;


public class RewardItem : ABaseComponent
{
    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public TMPro.TextMeshProUGUI mTxtPropCount;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public TMPro.TextMeshProUGUI Name;
    
    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public UnityEngine.UI.Image mImgPropIcon;

    private ItemData_BagProp propData;
    public ItemData_BagProp PropData => propData;

    public void SetData(ItemData_BagProp _propData)
    {
        propData = _propData;
        RectTransform rect = mImgPropIcon.GetComponent<RectTransform>();
        mTxtPropCount.text = $"x{_propData.PropCount}";
        Name.text= _propData.Config.Name;
        ResourceMgr.Instance.SetImageSprite(mImgPropIcon, _propData.Config.Sprite);
    }
}

public class RewardItemData
{
    public string ResourceName;
    public int Count;
}