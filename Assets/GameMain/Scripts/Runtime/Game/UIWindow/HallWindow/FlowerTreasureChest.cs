using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameMain;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FlowerTreasureChest : MonoBehaviour
{
    public UICustomButton btn;

    public Image ProgressImg;

    public TextMeshProUGUI ProgressText;

    public Transform OkFlag;

    public Transform ShowRewardNode;

    public TextMeshProUGUI targetValue;

    public DoShakeRotation Shake;
    
    public Image targetImage;

    public List<Image> RewardImages;
    public Transform RewardTf;
    private EventGroup _eventGroup = new();
    private void Awake()
    {
        btn.AddListener(OnClickBtn);
    }

    private void OnClickBtn()
    {
        UIWindowMgr.Instance.OpenWindowAsync<OpenFlowerTreasureChestView>();
        /*var datas = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerTreasureChest>()
            .GetCurFlowerTreasureChestId();
        if (datas != null)
        {
            var config = ConfigMgr.Instance.Tables.FlowerTreasureChestCategory.Get(datas.id);
            if (datas.CollectCount >= config.TargetNum)
            {
                UIWindowMgr.Instance.OpenWindowAsync<OpenFlowerTreasureChestView>();
            }
            else
            {
                ShowRewardNode.gameObject.SetActive(!ShowRewardNode.gameObject.activeSelf);
            }
        }*/
    }

    void Start()
    {
        ShowRewardNode.gameObject.SetActive(false);
        _eventGroup.AddListener<GameEventDefine.CollectFlowerTreasureChestChange>(ProcessMessage);
        UpdateUI();
    }

    private void ProcessMessage(IEventMessage obj)
    {
        UpdateUI();
    }

    private void OnDestroy()
    {
        _eventGroup.RemoveAllListener();
    }

    private void UpdateUI()
    {
        var datas = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerTreasureChest>(true)
            .GetCurFlowerTreasureChestId();
        if (datas != null)
        {
            var config = ConfigMgr.Instance.Tables.FlowerTreasureChestCategory.Get(datas.id);
            targetValue.text = config.TargetNum.ToString();
            ProgressText.text = $"{datas.CollectCount}/{config.TargetNum}";
            ProgressImg.fillAmount = (float)datas.CollectCount / config.TargetNum;
            OkFlag.gameObject.SetActive(datas.CollectCount >= config.TargetNum);
            if (datas.CollectCount >= config.TargetNum)
            {
                Shake.PlayAnim();
            }
            else
            {
                Shake.StopAnim();
            }
            /*var posintConfig =
                ConfigMgr.Instance.Tables.FlowerLevelCategory.Get(config.FlowerType_Ref.Id,
                    config.FlowerType_Ref.MaxLevel);
            ResourceMgr.Instance.SetImageSprite(targetImage, posintConfig.FlowerSprite);

            var allKey = config.AwardItem.Keys.ToList();
            for (int i = 0; i < RewardImages.Count; i++)
            {
                RewardImages[i].gameObject.SetActive(i < config.AwardItem.Count);
                if (i < allKey.Count)
                {
                    var Config = ConfigMgr.Instance.Tables.BagPropCategory.GetOrDefault(allKey[i]);
                    ResourceMgr.Instance.SetImageSprite(RewardImages[i], Config.Sprite);
                }
                if (config.AwardPower>0 && i==allKey.Count)
                {
                    RewardImages[i].gameObject.SetActive(true);
                    ResourceMgr.Instance.SetImageSprite(RewardImages[i], "nuohua_zjm_tili");
                }
            }*/
        }
    }

    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                var clickObj = EventSystem.current.currentSelectedGameObject;
                if (clickObj == btn.gameObject)
                {
                    OnClickBtn();
                }
                else if (ShowRewardNode.gameObject.activeSelf)
                {
                    ShowRewardNode.gameObject.SetActive(!ShowRewardNode.gameObject.activeSelf);
                }
            }
        }*/
    }
}