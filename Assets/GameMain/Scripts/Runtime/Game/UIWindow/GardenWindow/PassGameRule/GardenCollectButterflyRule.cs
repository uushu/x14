using Spine.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameMain.Garden
{
    public class GardenCollectButterflyRule : ABaseGardenSuccessRule
    {
        public override EPassGameRule PassGameRule => EPassGameRule.CollectButterFly;

        public GameObject completeRoot;

        public SkeletonGraphic completeSkeleton;

        public List<ItemUI_CollectButterfly> butterflys;

        public override void Initialize(int levelId, object parameter)
        {
            base.Initialize(levelId, parameter);
            Dictionary<int, int> collect = parameter as Dictionary<int, int>;
            var keys = collect.Keys.ToList();

            for (int i = 0; i < butterflys.Count; i++)
            {
                if (i < keys.Count)
                {
                    int flowerId = keys[i];
                    var flowerLevelConfig = ConfigMgr.Instance.Tables.GardenFlowerLevelCategory.Get(flowerId, 1);
                    butterflys[i].Initialize(flowerId, collect[keys[i]], flowerLevelConfig.FlowerSprite);
                    butterflys[i].gameObject.SetActive(true);
                }
                else
                { 
                    butterflys[i].gameObject.SetActive(false);
                }
            }

            completeRoot.SetActive(false);
        }

        protected override void OnPlaceToMapBefore(FlowerRemoveResult result)
        {
            //判断本次的蝴蝶是否收集完成
            foreach (var item in result.removeFlowers)
            {
                var butterflyItemUI = GetButterflyBy(item.Key);
                if (butterflyItemUI != null)
                {
                    butterflyItemUI.AddLogic_CollectCount(item.Value.Count);
                }
            }

            base.OnPlaceToMapBefore(result);
        }

        protected override void OnPlaceToMapAfter()
        {
            base.OnPlaceToMapAfter();

            if (IsGameSuccess())
            {
                AudioMgr.Instance.PlayAudioOneShot(AudioName.CompleteGameAudio);

                completeRoot.SetActive(true);
                completeSkeleton.AnimationState.SetAnimation(0, "idle", false);
                StartCoroutine(DelayHide(completeRoot, 2f));
            }
        }

        protected override void ShowGameSuccessUI()
        {
            base.ShowGameSuccessUI();

            var collectFlower = ConfigMgr.Instance.Tables.GardenLevelCategory.GetOrDefault(LevelId).CollectFlower;
            GameEventDefine.SendCollectButterfly(EMiniGameType.GardenFlower, LevelId, collectFlower);
        }

        public void SubLastCollectCount(int butterflyId)
        {
            var flower = GetButterflyBy(butterflyId);

            if (flower != null)
            {
                flower.SubAnimation_LastCollectCount(1);
            }

            //防止多次触发
            if (ShowGamSuccessUI != null)
            {
                if (CollectButterflyComplete())
                {
                    ShowGameSuccessUI();
                }
            }
        }

        private bool CollectButterflyComplete()
        {
            foreach (var flower in butterflys)
            {
                if (flower.gameObject.activeSelf == false) continue;

                if (flower.AnimationComplete == false)
                {
                    return false;
                }
            }

            return true;
        }

        public ItemUI_CollectButterfly GetButterflyBy(int butterflyId)
        {
            foreach (var item in butterflys)
            {
                if (item.flowerId == butterflyId)
                { 
                    return item;
                }
            }

            return null;
        }

        protected override bool IsGameSuccess()
        {
            foreach (var flower in butterflys)
            {
                if (flower.gameObject.activeSelf == false) continue;

                if (flower.LogicComplete == false)
                {
                    return false;
                }
            }
            return true;
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            butterflys = transform.GetComponentsInChildren<ItemUI_CollectButterfly>(true).ToList();
        }
#endif
    }
}