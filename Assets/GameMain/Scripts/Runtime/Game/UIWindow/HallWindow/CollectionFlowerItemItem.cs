using System;
using System.Collections;
using System.Collections.Generic;
using cfg;
using GameMain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionFlowerItemItem : MonoBehaviour
{
    public TextMeshProUGUI txtName;
    public Image imgIcon;
    public Transform GetTf;
    public Image QualityIcon;
    public TextMeshProUGUI UnLockText;
    public TextMeshProUGUI UnLockText2;
    public UICustomButton LookAdBtn;
    public Transform LookAdTip;
    public TextMeshProUGUI Points;
    private EventGroup _eventGroup = new();
    public UICustomButton LookNewBtn;
    public GameObject NewGo;
    private void Awake()
    {
        LookAdBtn.AddListener(OnClickLookAdBtn);
        LookNewBtn.AddListener(OnClickLookNewBtn);
    }

    private void OnClickLookNewBtn()
    {
        UIWindowMgr.Instance.OpenWindowAsync<CollectionFlowerDetailsWindow>(CurConfig);
    
    }

    private void ProcessMessage(IEventMessage obj)
    {
        if (obj is GameEventDefine.CollectFlowerAdCountChange)
        {
            UpdateUI(CurConfig);
        }else if (obj is GameEventDefine.CollectFlowerChange)
        {
            setNew();
        }
    }

    private void OnEnable()
    {
        _eventGroup.AddListener<GameEventDefine.CollectFlowerAdCountChange>(ProcessMessage);
        _eventGroup.AddListener<GameEventDefine.CollectFlowerChange>(ProcessMessage);
    }

    private void OnDisable()
    {
        _eventGroup.RemoveAllListener();
    }

    private void OnClickLookAdBtn()
    {
        PlatformMgr.Instance.LookAd((flag) =>
        {
            if (flag)
            {
                if (CurConfig != null)
                {
                    CollectFlowerMgr.Instance.CollectFloweraddAdCount(CurConfig.Id, 1);
                }
            }
        }, LookAdsType.收藏界面看广告解锁鲜花);
    }

    private FlowerCollectConfig CurConfig;

    public void SetData(FlowerCollectConfig config)
    {
        CurConfig = config;
        if (config != null)
        {
            txtName.text = config.FlowerType_Ref.FlowerName;
            ResourceMgr.Instance.SetImageSprite(imgIcon, config.FlowerRes,false);
            //imgIcon.SetNativeSize();
            QualityIcon.gameObject.SetActive(!string.IsNullOrEmpty(config.Quality));
            var posintConfig= ConfigMgr.Instance.Tables.GardenFlowerLevelCategory.Get(config.FlowerType_Ref.Id,config.FlowerType_Ref.MaxLevel);
            Points.text=posintConfig!=null? $"+{posintConfig.GardenAddScore}":"+0";
            if (!string.IsNullOrEmpty(config.Quality))
            {
                ResourceMgr.Instance.SetImageSprite(QualityIcon, config.Quality,true);
            }

            UpdateUI(config);
        }
    }

    private void setNew()
    {
        var data = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_CollectFragments>();
        NewGo.gameObject.SetActive(data!=null&& data.NewGet.Contains(CurConfig.FlowerType_Ref.Id));
        var findIt = CollectFlowerMgr.Instance.GetCollectFlower(CurConfig.Id);
        LookNewBtn.gameObject.SetActive(findIt!=null&& findIt.UnLock);
    }

    private void UpdateUI(FlowerCollectConfig config)
    {
        var findIt = CollectFlowerMgr.Instance.GetCollectFlower(config.Id);
        if (findIt != null)
        {
            LookAdBtn.gameObject.SetActive(findIt.UnLock == false && config.UnlockType == EUnLockType.AdUnLock);
            LookAdTip.gameObject.SetActive(findIt.UnLock == false && config.UnlockType == EUnLockType.AdUnLock);
            UnLockText.gameObject.SetActive(findIt.UnLock == false && config.UnlockType != EUnLockType.AdUnLock);
            setNew();
            GetTf.gameObject.SetActive(findIt.UnLock);
            if (findIt.UnLock)
            {
                imgIcon.SetAlpha(1);
            }
            else
            {
                imgIcon.SetAlpha(0.4f);
            }
            switch (config.UnlockType)
            {
                case EUnLockType.AdUnLock:
                    UnLockText.text = $"{findIt.adCount}/{config.UnlockValue}解锁";
                    UnLockText2.text = $"{findIt.adCount}/{config.UnlockValue}解锁";
                    break;
                case EUnLockType.ActivityUnLock:
                    UnLockText.text = "活动解锁";
                    break;
                case EUnLockType.LevelUnLock:
                    UnLockText.text = $"{config.UnlockValue-1000}关解锁";
                    break;
                case EUnLockType.VictoriesUnLock:
                    var level = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().currentSuccessCount;
                    UnLockText.text = $"{config.UnlockValue}连胜解锁{level}/{config.UnlockValue}";
                    break;
                case EUnLockType.FlowerTreasureChestUnLock:
                    UnLockText.text = $"花花宝箱解锁";
                    break;
            }
        }
    }
}