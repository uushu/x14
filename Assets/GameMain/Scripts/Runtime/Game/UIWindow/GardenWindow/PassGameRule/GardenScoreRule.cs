using DG.Tweening;
using Spine.Unity;
using System.Linq;
using TMPro;
using UnityEngine;

namespace GameMain.Garden
{
    public class GardenScoreRule : ABaseGardenSuccessRule
    {
        public override EPassGameRule PassGameRule => EPassGameRule.CollectScore;

        public UICustomSlider progressSlider;

        public Transform scaleRoot;

        public float currentProgress;

        public float maxProgress;

        public GameObject completeRoot;

        public SkeletonGraphic completeSkeleton;

        public TextMeshProUGUI textCompleteScore;

        public override void Initialize(int levelId, object parameter)
        {
            base.Initialize(levelId, parameter);
            int maxProgress = (int)parameter;

            GardenGameRoot.Instance.flowerGridsMap.GeneratorButterfly += OnGeneratorButterfly;

            this.maxProgress = maxProgress;
            currentProgress = 0;
            progressSlider.SetProgress(0, 0, maxProgress);
            completeRoot.SetActive(false);
        }

        private void DoScaleAnimation()
        {
            if (DOTween.IsTweening(scaleRoot)) return;

            var sequence = DOTween.Sequence();
            var t1 = scaleRoot.transform.DOScale(Vector3.one * 1.05f, 0.1f);
            var t2 = scaleRoot.transform.DOScale(Vector3.one, 0.1f);
            sequence.Append(t1);
            sequence.Append(t2);
            sequence.SetTarget(scaleRoot);
        }

        private async void OnGeneratorButterfly(Vector3 worldPosition)
        {
            var butterflyId = UtilityTool.RandomNumber(2001, 2011);
            var butterflyConfig = ConfigMgr.Instance.Tables.GardenFlowerTypeCategory.GetOrDefault(butterflyId);

            var butterFly = await GardenGameRoot.Instance.flowerPool.multiPools.GetEntityAsync(butterflyConfig.GardenSpinePrefab);
            butterFly.Initialize_Flower(butterflyConfig, false);
            butterFly.transform.SetParent(GardenGameRoot.Instance.flowerPool.flowerPoolRoot);
            butterFly.transform.position = worldPosition;
            butterFly.transform.localScale = Vector3.one;

            GardenGameRoot.Instance.flowerGridsMap.DoButterfly_FlyAnimation(butterFly, progressSlider.transform, true, (_) =>
            {
                AudioMgr.Instance.PlayAudioOneShot(AudioName.ButterflyHitAudio);
                DoScaleAnimation();
                SetProgressTween();
            });
        }

        public override void UnInitialize()
        {
            base.UnInitialize();

            GardenGameRoot.Instance.flowerGridsMap.GeneratorButterfly -= OnGeneratorButterfly;
        }

        private void SetProgressTween()
        {
            if (progressSlider.CurrentValue != currentProgress)
            {
                progressSlider.SetProgressTween(currentProgress, 0, maxProgress, 1f, null, null);
            }
        }

        private int GetAdditionScore(int rowOrColCount)
        {
            int addScore = 0;
            var addScoreDict = ConfigMgr.Instance.Tables.GlobalConfigCategory.GardenRowColsAddScore;
            if (addScoreDict.TryGetValue(rowOrColCount, out addScore) == false)
            {
                if (rowOrColCount > addScoreDict.LastOrDefault().Key)
                {
                    addScore = addScoreDict.LastOrDefault().Value;
                }
            }

            return addScore;
        }

        protected override int GetAddScore(FlowerRemoveResult result)
        {
            var addScore = 0;

            foreach (var list in result.removeFlowers)
            {
                foreach (var flower in list.Value)
                {
                    addScore += flower.AddScore;
                }
            }

            if (result.HasRowColRemove)
            {
                //消除多行额外加分
                var additionScore = GetAdditionScore(result.rowList.Count + result.colList.Count);
                addScore += additionScore;

                //连击加分
                addScore += _currentComboCount * ConfigMgr.Instance.Tables.GlobalConfigCategory.GardenComboBaseScore;
            }

            return addScore;
        }

        protected override void OnPlaceToMapBefore(FlowerRemoveResult result)
        {
            var addScore = GetAddScore(result);
            currentProgress += addScore;

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
                textCompleteScore.text = UtilityTool.ShortNumberToString(progressSlider.MaxValue);
                StartCoroutine(DelayHide(completeRoot, 2f));

                //弹出结算界面
                ShowGameSuccessUI();
            }
        }

        protected override bool IsGameSuccess()
        {
            return currentProgress >= maxProgress;
        }
    }
}