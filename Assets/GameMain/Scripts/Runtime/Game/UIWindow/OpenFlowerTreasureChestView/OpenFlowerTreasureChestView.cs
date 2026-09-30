using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cfg;
using GameMain;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenFlowerTreasureChestView : ABaseWindow
{
    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public GameMain.UICustomButton ClaimBtn;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public SkeletonGraphic Skeleton;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public GameObject RewardPrefab;
    

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public Transform StartPosTf;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public Image targetImage;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public TextMeshProUGUI targetText;

    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public Transform AnimMask;
    
    
    private List<RewardItem> rewardObj;
    private TrackEntry _entry;
    private CollectFlowerTreasureChest CurData;

    private FlowerTreasureChestConfig CurConfig;

    List<ItemData_BagProp> allReward = new();
    protected override void OnShow(object parameters)
    {
        rewardObj = new();
        ClaimBtn.AddListener(OnClickClaim);
        // ClaimBtn.gameObject.SetActive(false);
        AnimMask.gameObject.SetActive(false);
        CurData = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerTreasureChest>(true)
            .GetCurFlowerTreasureChestId();
        if (CurData != null)
        {
            CurConfig = ConfigMgr.Instance.Tables.FlowerTreasureChestCategory.Get(CurData.id);
            if (CurConfig != null)
            {
                var posintConfig =
                    ConfigMgr.Instance.Tables.GardenFlowerLevelCategory.Get(CurConfig.FlowerType_Ref.Id,
                        CurConfig.FlowerType_Ref.MaxLevel);
                ResourceMgr.Instance.SetImageSprite(targetImage, posintConfig.FlowerSprite);
                targetText.text = CurConfig.TargetNum.ToString();

                allReward.Clear();
                foreach (var item in    CurConfig.AwardItem)
                {
                    allReward.Add(new ItemData_BagProp(item.Key, item.Value));
                }
             
            }
        }
    }

    private async void AwardAnimatinComplete(TrackEntry trackEntry)
    {
        _entry.Complete -= AwardAnimatinComplete;
       
       // UIWindowMgr.Instance.ShowPromptBox("领取成功");
        UIWindowMgr.Instance.OpenWindowAsync<CommGetView>(allReward);
        //CloseSelfWindow();
    }

    private void OnDestroy()
    {
        if (rewardObj != null)
        {
            foreach (var item in rewardObj)
            {
                Destroy(item.gameObject);
            }
            rewardObj.Clear();
        }

        if (_entry!= null)
        {    _entry.Complete -= AwardAnimatinComplete;
            _entry.TimeScale=0;
        }
    }

    private async Task<T> LoadPrefab<T>(string Prefabname, Transform tf)
    {
        var go = await ResourceMgr.Instance.LoadAssetAsync<GameObject>(Prefabname);
        var instance = Instantiate(go, tf);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localScale = Vector3.one;
        var prefab = instance.GetComponent<T>();
        return prefab;
    }

    private void OnClickClaim()
    {
        var config = ConfigMgr.Instance.Tables.FlowerTreasureChestCategory.Get(CurData.id);
        if (CurData.isClaim == false && CurData.CollectCount >= config.TargetNum)
        {
            ClaimBtn.gameObject.SetActive(false);
            _entry = Skeleton.AnimationState.SetAnimation(0, "idle", false);
            _entry.Complete += AwardAnimatinComplete;
            LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerTreasureChest>(true)
                .SetNextFlowerTreasureChest((EMiniGameType)CurData.gameType);
            
            BagMgr.Instance.AddBagProp(allReward);

            foreach (var item in allReward)
            {
                if (item.Config.ItemType==4)
                {
                    var data = CollectFlowerMgr.Instance.GetCollectFlower(item.Config.FlowerId);
                    if (data != null)
                    {
                        data.UnLock = true;
                    }
                }
            }
        }
        else
        {
            UIWindowMgr.Instance.ShowPromptBox("快去收集蝴蝶吧");
        }
    }
}