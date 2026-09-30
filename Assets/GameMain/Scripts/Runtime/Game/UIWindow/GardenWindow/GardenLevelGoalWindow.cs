using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameMain.Garden
{
    public class GardenLevelGoalWindow : ABaseWindow
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.GameObject mImgGoalBg;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.GameObject mImgGoal_Collect_Butterfly;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.GameObject mImgGoal_CollectScore;

        [LabelText("收集蝴蝶通关目标")]
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public List<ItemUI_CollectButterfly> goal_collectButterflys;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public TMPro.TextMeshProUGUI mTxtCollectScore;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.GameObject mImgStar;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.Image mImgMask;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.Animator mGardenLevelGoalWindow;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.ContentSizeFitter mImgGoal_Collect_Butterfly_fiter;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.HorizontalLayoutGroup mImgGoal_Collect_Butterfly_group;

        protected override void OnShow(object parameters)
        {
            base.OnShow(parameters);

            int levelId = (int)parameters;

            var gardenLevelConfig = ConfigMgr.Instance.Tables.GardenLevelCategory.GetOrDefault(levelId);

            if (gardenLevelConfig.CollectScore > 0)
            {
                mImgGoal_Collect_Butterfly.SetActive(false);
                mImgGoal_CollectScore.SetActive(true);

                mTxtCollectScore.text = gardenLevelConfig.CollectScore.ToString();

                StartCoroutine(DelayCloseWindow());
            }
            else
            {
                mImgGoal_Collect_Butterfly.SetActive(true);
                mImgGoal_CollectScore.SetActive(false);

                var keys = gardenLevelConfig.CollectFlower.Keys.ToList();
                for (int i = 0; i < goal_collectButterflys.Count; i++)
                {
                    if (keys.Count > i)
                    {
                        goal_collectButterflys[i].gameObject.SetActive(true);

                        int flowerId = keys[i];
                        var flowerLevelConfig = ConfigMgr.Instance.Tables.GardenFlowerLevelCategory.Get(flowerId, 1);
                        goal_collectButterflys[i].Initialize(flowerId, gardenLevelConfig.CollectFlower[keys[i]], flowerLevelConfig.FlowerSprite);
                    }
                    else
                    { 
                        goal_collectButterflys[i].gameObject.SetActive(false);
                    }
                }

                StartCoroutine(ButterflyToGoal());
            }

        }

        private IEnumerator DelayCloseWindow()
        {
            yield return new WaitForSeconds(0.3f);
            AudioMgr.Instance.PlayAudioOneShot(AudioName.GaedenGameGoalAudio);
            yield return new WaitForSeconds(mGardenLevelGoalWindow.GetCurrentAnimatorStateInfo(0).length);
            CloseSelfWindow();
        }

        private IEnumerator ButterflyToGoal()
        {
            yield return new WaitForSeconds(0.3f);
            AudioMgr.Instance.PlayAudioOneShot(AudioName.GaedenGameGoalAudio);
            yield return new WaitForSeconds(mGardenLevelGoalWindow.GetCurrentAnimatorStateInfo(0).length - 0.3f);

            mImgGoal_Collect_Butterfly_fiter.enabled = false;
            mImgGoal_Collect_Butterfly_group.enabled = false;
            mGardenLevelGoalWindow.enabled = false;
            yield return null;

            for (int i = 0; i < goal_collectButterflys.Count; i++)
            {
                if (goal_collectButterflys[i].gameObject.activeSelf == false) break;

                DoFlyTarget(goal_collectButterflys[i]);
            }

            yield return new WaitForSeconds(0.4f);
            mImgGoalBg.SetActive(false);
            mImgMask.SetAlpha(0f);

            yield return new WaitForSeconds(0.5f);
            CloseSelfWindow();
        }

        private void DoFlyTarget(ItemUI_CollectButterfly butterflyUI)
        {
            var flyTarget = UIWindowMgr.Instance.GetWindow<GardenWindow>().mCollectFlowerRule.GetButterflyBy(butterflyUI.flowerId);
            if (flyTarget != null)
            {
                float bezierTime = 0.7f;
                var sequence = DOTween.Sequence();
                sequence.AppendInterval(0.1f);
                var t3 = butterflyUI.transform.DOScale(Vector3.one * 0.7f, bezierTime - 0.1f);
                sequence.AppendCallback(() =>
                {
                    StartCoroutine(UtilityTool.DoBezierMove(butterflyUI.transform, flyTarget.transform.position, bezierTime, -0.8f, -0.8f, () =>
                    {
                        butterflyUI.gameObject.SetActive(false);
                        flyTarget.DoHitScaleAnimation();
                    }));
                });
                sequence.Join(t3);
            }
        }

#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();
            goal_collectButterflys = transform.GetComponentsInChildren<ItemUI_CollectButterfly>(true).ToList();
        }

#endif
    }
}