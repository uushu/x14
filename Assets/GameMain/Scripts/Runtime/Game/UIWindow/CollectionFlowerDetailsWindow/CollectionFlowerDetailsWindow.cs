using System.Collections;
using System.Collections.Generic;
using cfg;
using GameMain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionFlowerDetailsWindow : ABaseWindow
{
    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public Image mResImage;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public TextMeshProUGUI mNameText;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public TextMeshProUGUI mLanguageFlowersText;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public TextMeshProUGUI mFlowersDescText;

    private FlowerCollectConfig CurConfig;

    protected override void OnShow(object parameters)
    {
        CurConfig = (FlowerCollectConfig)parameters;
        mNameText.text = CurConfig.FlowerType_Ref.FlowerName;
        ResourceMgr.Instance.SetImageSprite(mResImage, CurConfig.FlowerRes, false);
        mLanguageFlowersText.text = $"        {CurConfig.FlowerLanguage}";
        mFlowersDescText.text = $"    {CurConfig.FlowerIntroduction}";

        var data = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_CollectFragments>();
        if (data != null)
        {
            data.RemoveGetFlower(CurConfig.FlowerType_Ref.Id);
            GameEventDefine.SendCollectFlowerChange();
        }
    }
}