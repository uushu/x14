using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameMain;
using GameMain.Garden;
using Spine;
using TMPro;
using UnityEngine;

public class CommGetFlowerstView : ABaseWindow
{
    public GameObject NodeTf;
    public TextMeshProUGUI Name;
    public List<Transform> TfList;
    List<Garden_FlowerSingle> tfFlowerSingle=new ();
    protected async override void OnShow(object parameters)
    {
        AudioMgr.Instance.PlayAudioOneShot(AudioName.xianhuahuode);
        ItemData_BagProp flowerItem = (ItemData_BagProp)parameters;
        var config= ConfigMgr.Instance.Tables.GardenFlowerTypeCategory.Get(flowerItem.Config.FlowerId);
        var size = Vector3.one;
        if (config!=null)
        {
            Name.text = String.Empty;
            GameObject go = await ResourceMgr.Instance.LoadAssetAsync<GameObject>(config.GardenSpinePrefab);
            var instance = Instantiate(go, NodeTf.transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localScale = size;
        
            for (int i = 0; i < TfList.Count; i++)
            {
                var instance2 = Instantiate(go, TfList[i]);
                instance2.transform.localPosition = Vector3.zero;
                instance2.transform.localScale = size;
                Garden_FlowerSingle Garden_FlowerSingle2= instance2.GetComponent<Garden_FlowerSingle>();
                tfFlowerSingle.Add(Garden_FlowerSingle2);
            }
            var Garden_FlowerSingle= instance.GetComponent<Garden_FlowerSingle>();
            if (Garden_FlowerSingle!=null)
            {  Name.text = config.FlowerName;
                foreach (var item in tfFlowerSingle)
                {
                    TrackEntry  trackEntry2=  item.flowerSpine.AnimationState.SetAnimation(0, "0_born", false);
                }
                TrackEntry  trackEntry=  Garden_FlowerSingle.flowerSpine.AnimationState.SetAnimation(0, "0_born", false);
                await UniTask.WaitForSeconds(1f);
                trackEntry.TimeScale = 0;
            }
        }
        else
        {
            CloseSelfWindow();
        }
    }
}