using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameMain;
using UnityEngine;
using UnityEngine.UI;

public class GardenEndlessTitleUpWindow : ABaseWindow
{
    public GameObject NodeTf;

    public CanvasGroup  CanvasGroup;
    private GameObject instance;

    public Image MaskImage;
    protected async override void OnShow(object parameters)
    {
        AudioMgr.Instance.PlayAudioOneShot(AudioName.duanweishengji);
        int ConfigId = (int)parameters;
        var confg = ConfigMgr.Instance.Tables.GardenEndlessTitleCategory.GetOrDefault(ConfigId);
        if (confg != null)
        {
            GameObject go = await ResourceMgr.Instance.LoadAssetAsync<GameObject>(confg.SpinePrefab);
            instance = Instantiate(go, NodeTf.transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localScale = Vector3.one;
        }
        await UniTask.WaitForSeconds(2f);
        MaskImage.DoAlpha(0, 0.5f);
        CanvasGroup.DoAlpha(0, 0.5f).SetOnFinish(() =>
        {
            Destroy(instance);
            instance = null;
            CloseSelfWindow();
        });
     
    }
}