using System;
using System.Collections;
using System.Collections.Generic;
using cfg;
using GameMain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionIllustratedItemItem : MonoBehaviour
{
    public Image ResImage;

    public TextMeshProUGUI NameText;

    public GameObject getNode;

    public TextMeshProUGUI ProgressText;

    public Image ProgressImage;
    
    public TiledMask GridPImage;
    
    
    public void SetData(FlowerAtlasConfig config)
    {
        
        NameText.text = config.FlowerAtlasName;
        ResourceMgr.Instance.SetImageSprite(ResImage, config.FlowerSprite);
        
        bool isGet = true;
        var AllcollectFragments = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_CollectFragments>();
        List<int> GetfragmentsId = new List<int>();
        foreach (var fragment in config.FragmentsData)
        {
            if (!AllcollectFragments.fragments.Contains(fragment))
            {
                isGet = false;
            }
            else
            {
                GetfragmentsId.Add(fragment);
            }
        }

        getNode.gameObject.SetActive(isGet);
        ProgressImage.gameObject.transform.parent.gameObject.SetActive(!isGet);
        if (!isGet)
        {   GridPImage.gameObject.SetActive(true);
            //GridPtf.gameObject.SetActive(false);
            var CollectTotalNum = config.GridCount;
            var CollectNum = 0;
            foreach (var FragmentId in GetfragmentsId)
            {
                var FragmentsConfig =
                    ConfigMgr.Instance.Tables.FlowerCollectFragmentsCategory.GetOrDefault(FragmentId);
                if (FragmentsConfig != null)
                {
                    for (int i = FragmentsConfig.ShowGridCount[0];
                         i <= FragmentsConfig.ShowGridCount[1];
                         i++)
                    {
                        CollectNum++;
                    }
                }
            }
            // 更新隐藏进度
            var currentProgress =CollectNum>0? (CollectNum * 1.0f / CollectTotalNum * 1.0f):0f;
            GridPImage.progress = 1 - currentProgress;
            ProgressImage.fillAmount = currentProgress;
            ProgressText.text = ProgressImage.fillAmount.ToString("P0");
        }
        else
        {
            GridPImage.gameObject.SetActive(false);
            //GridPtf.gameObject.SetActive(false);
        }
    }
}