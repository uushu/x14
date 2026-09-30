using GameMain.Garden;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameMain.FlowerBlock
{
    public class BlockCollectFlowerRule : MonoBehaviour, IBlockPassGameRule
    {
        public List<ItemUI_CollectButterfly> flowers;

        public Animation completeAnimation;

        public Action OnGameSuccessCallBack { get; set; }
        public Action OnGameFailedCallBack { get; set; }

        public int LevelId { get; private set; }

        public void Initialize(int levelId, Dictionary<int, int> collect)
        {
            LevelId = levelId;
            var keys = collect.Keys.ToList();

            for (int i = 0; i < flowers.Count; i++)
            {
                if (i < keys.Count)
                {
                    int flowerId = keys[i];
                    var flowerLevelConfig = ConfigMgr.Instance.Tables.BlockFlowerTypeCategory.Get(flowerId);
                    flowers[i].Initialize(keys[i], collect[keys[i]], flowerLevelConfig.FlowerSprite);
                    flowers[i].gameObject.SetActive(true);
                }
                else
                {
                    flowers[i].gameObject.SetActive(false);
                }
            }

            completeAnimation.gameObject.SetActive(false);
            BlockGameRoot.Instance.blockMap.OnBeforeRemoveFlowerBlockCallBack += OnBeforeRemoveFlowerBlock;
            BlockGameRoot.Instance.blockMap.OnFlowerBlockTopFullCallBack += OnFlowerBlockTopFull;
        }


        public void UnInitialize()
        {
            BlockGameRoot.Instance.blockMap.OnBeforeRemoveFlowerBlockCallBack -= OnBeforeRemoveFlowerBlock;
            BlockGameRoot.Instance.blockMap.OnFlowerBlockTopFullCallBack -= OnFlowerBlockTopFull;
        }

        private void JudgeGameIsSuccess()
        {
            if (CollectButterflyAnimationIsComplete())
            {
                Debugger.Log("游戏成功 - CollectFlowerRule");
                if (OnGameSuccessCallBack != null)
                {
                    OnGameSuccessCallBack.Invoke();
                    GameEventDefine.SendGameSuccessResult(EMiniGameType.BlockFlower, LevelId);
                    OnGameSuccessCallBack = null;

                    var collectFlower = ConfigMgr.Instance.Tables.BlockLevelCategory.GetOrDefault(LevelId).CollectFlower;
                    GameEventDefine.SendCollectButterfly(EMiniGameType.BlockFlower, LevelId, collectFlower);
                }
            }
        }

        private void OnFlowerBlockTopFull()
        {
            OnGameFailedCallBack?.Invoke();
        }

        private void OnBeforeRemoveFlowerBlock(FlowerBlockRemoveResult result)
        {
            foreach (var blockItem in result.removeBlockItems)
            {
                foreach (var flower in blockItem.flowers)
                {
                    AddFlowerCollect(flower.FlowerConfigId, 1);
                }
            }

            if (CollectButterflyLogicIsComplete() && BlockGameRoot.Instance.currentStatus != EStatus.Success)
            {
                BlockGameRoot.Instance.ChangeStatus(EStatus.Success);

                //弹出complete动画
                StartCoroutine(PlayCompleteAnimation());
            }
        }

        private IEnumerator PlayCompleteAnimation()
        {
            completeAnimation.gameObject.SetActive(true);
            completeAnimation.Play("CompleteAnimation");
            yield return new WaitForSeconds(1.5f);
            completeAnimation.gameObject.SetActive(false);
        }

        public void AddFlowerCollect(int flowerId, int flowerCount)
        {
            var flower = GetFlowerBy(flowerId);

            if (flower != null)
            {
                flower.AddLogic_CollectCount(flowerCount);
            }
        }

        public void SubFlowerCollect(int flowerId, int subCount)
        {
            var flower = GetFlowerBy(flowerId);

            if (flower != null)
            {
                flower.SubAnimation_LastCollectCount(subCount);
            }

            JudgeGameIsSuccess();
        }

        public ItemUI_CollectButterfly GetFlowerBy(int flowerId)
        {
            foreach (var item in flowers)
            {
                if (item.flowerId == flowerId)
                {
                    return item;
                }
            }

            return null;
        }

        public bool CollectButterflyAnimationIsComplete()
        {
            foreach (var flower in flowers)
            {
                if (flower.AnimationComplete == false)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CollectButterflyLogicIsComplete()
        {
            foreach (var flower in flowers)
            {
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
            flowers = transform.GetComponentsInChildren<ItemUI_CollectButterfly>().ToList();
        }
#endif
    }
}