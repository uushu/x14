using System.Collections;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using cfg;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain.Garden
{
    public class GardenSuccessWindow : ABaseWindow
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnNextLevel;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnMainPage;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public Image mResImage;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public Image ProgressValue;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public TextMeshProUGUI ProgressValueText;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public List<Transform> gridTfList;


        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public TextMeshProUGUI addValeuText;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public TextMeshProUGUI GoldValeuText;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public Image GoldImage;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public CoinEffect coinEffect;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public Transform ClickMask;
        
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public ItemUI_BagProp Rewarditem;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public TextMeshProUGUI TipsLevel;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public TextMeshProUGUI TipsRewardName;
        
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameObject TipGrid;
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameObject Tip2Grid;
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public TextMeshProUGUI Tips2RewardName;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameObject DiRes;
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public Image AllRes;
        
        public GameObject EffectGo;

        bool CollectAll;
        int CurFlowerAtlas;

        protected async override void OnShow(object parameters)
        {
            ClickMask.gameObject.SetActive(true);
            mBtnNextLevel.gameObject.transform.localScale = Vector3.zero;
            mBtnMainPage.gameObject.transform.localScale = Vector3.zero;
            AudioMgr.Instance.PlayAudioOneShot(AudioName.VictoryAudio);
            AllRes.gameObject.SetActive(false);
            DiRes.gameObject.SetActive(true);
            base.OnShow(parameters);
            int levelId = (int)parameters;
            List<Transform> hideTf = new List<Transform>();
            var CurGradenLevelConfig = ConfigMgr.Instance.Tables.GardenLevelCategory.GetOrDefault(levelId);
            var AllcollectFragments = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_CollectFragments>();
            var CollectNum = 0;
            var CollectTotalNum = 0;
            var AddNum = 0;
            CollectAll = true;
            CurFlowerAtlas = 0;
            var FullFlowerResName = string.Empty;
            if (CurGradenLevelConfig != null)
            {
                var Curfragments =
                    ConfigMgr.Instance.Tables.FlowerCollectFragmentsCategory.GetOrDefault(CurGradenLevelConfig
                        .AwardCollectFragments);
                CurGradenLevelConfig.AwardProps.TryGetValue(1, out AddNum);
                addValeuText.text = $"+{AddNum}";
                if (Curfragments != null)
                {
                    CurFlowerAtlas = Curfragments.FlowerAtlas;
                    var flowerAtlas =
                        ConfigMgr.Instance.Tables.FlowerAtlasCategory.GetOrDefault(Curfragments.FlowerAtlas);
                    FullFlowerResName= flowerAtlas.FlowerSpriteNoMask;
                    CollectTotalNum = flowerAtlas.GridCount;
                    for (int i = 0; i < gridTfList.Count; i++)
                    {
                        gridTfList[i].gameObject
                            .SetActive(i >= Curfragments.ShowGridCount[0] && i < flowerAtlas.GridCount);
                        if (i >= Curfragments.ShowGridCount[0] && i <= Curfragments.ShowGridCount[1])
                        {
                            hideTf.Add(gridTfList[i]);
                        }
                    }

                    ResourceMgr.Instance.SetImageSprite(mResImage, flowerAtlas.FlowerSprite);

                    if (flowerAtlas != null)
                    {
                        foreach (var FragmentId in flowerAtlas.FragmentsData)
                        {
                            if (FragmentId != Curfragments.Id && AllcollectFragments.fragments.Contains(FragmentId))
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

                            if (!AllcollectFragments.fragments.Contains(FragmentId))
                            {
                                CollectAll = false;
                            }
                        }
                    }
                }
            }

            resultsOld = FlowerGardenMgr.Instance.GetExtraAwardProgress(levelId - 1);
            var endnum = BagMgr.Instance.GetBagPropCount(GameTool.ItemGoldId);
            GoldValeuText.text = (endnum - AddNum).ToString();
            mBtnNextLevel.AddListener(OnNextLevelClick);
            mBtnMainPage.AddListener(OnMainPageClick);
            coinEffect.num = Random.Range(5, 10);
            addValeuText.gameObject.SetActive(false);

            resultsNew = FlowerGardenMgr.Instance.GetExtraAwardProgress(levelId);
            TipsLevel.text = resultsNew.Item2.ToString();

            ProgressValue.fillAmount = resultsOld.Item1 >= 0.99 ? 0 : resultsOld.Item1;
            ProgressValueText.text = resultsOld.Item1.ToString("P0");

            var config = ConfigMgr.Instance.Tables.GardenLevelCategory.Get(resultsNew.Item3);
            Rewarditem.gameObject.SetActive(config != null && config.ExtraAwardProps.Count > 0);
            if (config != null && config.ExtraAwardProps.Count > 0)
            {
                var id = config.ExtraAwardProps.First();
                var itemData = new ItemData_BagProp(id.Key, id.Value);
                Rewarditem.RefreshBagPropUI(itemData);
                TipsRewardName.text = itemData.Config.Name;
                Tips2RewardName.text = itemData.Config.Name;
            }
            TipGrid.gameObject.SetActive(resultsNew.Item2>0);
            Tip2Grid.gameObject.SetActive(resultsNew.Item2==0);
            
            //DOTween.To(value => { TipsLevel.text = Mathf.Floor(value).ToString(); }, startValue: 0,
            //    endValue: resultsNew.Item2, duration: 1f);
            await UniTask.WaitForSeconds(0.8f);
            DOTween.To(value => { GoldValeuText.text = Mathf.Floor(value).ToString(); }, startValue: endnum - AddNum,
                endValue: endnum, duration: 1.5f);
            coinEffect.CoinEffectShow(() =>
            {
                addValeuText.gameObject.SetActive(true);
                DOTween.Sequence()
                    .Join(addValeuText.transform.DOLocalMoveY(addValeuText.transform.localPosition.y + 30, 1.5f))
                    .Join(addValeuText.DOFade(0, 1.5f))
                    .onComplete = () => { addValeuText.gameObject.SetActive(false); };
            }, () =>
            {
                DoHitScaleAnimation();
                DoEffect();
            });
            ProgressEffect();
            AudioMgr.Instance.PlayAudioOneShot(AudioName.MaisaiketiaodongAudio);
            foreach (var tf in hideTf)
            {
                tf.gameObject.SetActive(false);
                await UniTask.WaitForSeconds(0.00001f);
            }

            ClickMask.gameObject.SetActive(false);
            mBtnNextLevel.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.Linear);
            mBtnMainPage.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.Linear);
            if (CollectAll && CurFlowerAtlas > 0)
            {
                UIWindowMgr.Instance.OpenWindowAsync<GetCollectedFlowersWindow>(CurFlowerAtlas);
              // if (panel!=null)
               {
                 //  panel.OnClose = (resName) =>
                   {
                       DiRes.gameObject.SetActive(false);
                       //AllRes.gameObject.transform.localScale = Vector3.zero;
                       ResourceMgr.Instance.SetImageSprite(AllRes, FullFlowerResName);
                       AllRes.gameObject.SetActive(true);
                      // AllRes.gameObject.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.Linear);
                   };
               }
            }

            if (CurGradenLevelConfig != null && CurGradenLevelConfig.ExtraAwardProps.Count > 0)
            {
                List<ItemData_BagProp> rewardDatas = new();
                foreach (var item in CurGradenLevelConfig.ExtraAwardProps)
                {
                    rewardDatas.Add(new ItemData_BagProp(item.Key, item.Value));
                }

                var findit= rewardDatas.FirstOrDefault(s => s.Config.ItemType == 4);
                if (findit!=null)
                {
                    UIWindowMgr.Instance.OpenWindowAsync<CommGetFlowerstView>(findit);
                }
                else
                {
                    UIWindowMgr.Instance.OpenWindowAsync<CommGetView>(rewardDatas);
                }
            
                
            }
        }

        private (float, int, int) resultsOld;
        private (float, int, int) resultsNew;

        private void ProgressEffect()
        {
            if (resultsOld.Item1 >= 0.99)
            {
                resultsOld.Item1 = 0;
            }

            DOTween.To(value => { ProgressValue.fillAmount = value; }, startValue: resultsOld.Item1,
                endValue: resultsNew.Item1, duration: 0.5f);
            DOTween.To(value => { ProgressValueText.text = value.ToString("P0"); }, startValue: resultsOld.Item1,
                endValue: resultsNew.Item1, duration: 0.5f);
        }

        public void DoHitScaleAnimation()
        {
            if (DOTween.IsTweening(GoldImage.transform))
            {
                return;
            }

            var sequence = DOTween.Sequence();
            var t1 = GoldImage.transform.DOScale(Vector3.one * 1.2f, 0.1f);
            var t2 = GoldImage.transform.DOScale(Vector3.one, 0.1f);
            sequence.Append(t1);
            sequence.Append(t2);
            sequence.SetTarget(GoldImage.transform);
        }

        // private GameObject effect;
        public async void DoEffect()
        {
            // if (effect==null)
            {
                //var go= await ResourceMgr.Instance.LoadAssetAsync<GameObject>(EffectGo);
                var effect = Instantiate(EffectGo, GoldImage.transform);
                effect.transform.localPosition = Vector3.zero;
                effect.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                // await UniTask.WaitForSeconds(0.5f);
                Destroy(effect, 0.3f);
                // effect = null;
            }
        }


        private void OnMainPageClick()
        {
            UIWindowTool.ChangeToHallScene(false).Forget();
        }

        private void OnNextLevelClick()
        {
            CloseSelfWindow(false);
            UIWindowMgr.Instance.GetWindow<GardenWindow>().ManualStarGame();
        }
    }
}