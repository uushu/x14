using System;
using System.Linq;
using UnityEngine;

namespace GameMain.Garden
{
    public class GardenEndlessRule : ABaseGardenSuccessRule
    {
        public override EPassGameRule PassGameRule => EPassGameRule.Endless;

        public GardenEndlessScoreProgress endlessScoreProgress;

        protected int _addScore;

        public Action<int> OnScoreChangeCallBack { get; set; }

        public override void Initialize(int levelId, object parameter)
        {
            base.Initialize(levelId, parameter);

            GardenGameRoot.Instance.flowerGridsMap.GeneratorButterfly += OnGeneratorButterfly;
            endlessScoreProgress.Initialize(levelId);
            _addScore = 0;
        }

        public override void UnInitialize()
        {
            base.UnInitialize();
            GardenGameRoot.Instance.flowerGridsMap.GeneratorButterfly -= OnGeneratorButterfly;
        }

        protected override void OnPlaceToMapBefore(FlowerRemoveResult result)
        {
            base.OnPlaceToMapBefore(result);

            var addScore = GetAddScore(result);
            _addScore += addScore;

            if (result.HasRowColRemove == false)
            {
                //不会飞蝴蝶,刷新一次放置的格子的分数
                TryDoAddScoreAnimation(false);
            }
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

            GardenGameRoot.Instance.flowerGridsMap.DoButterfly_FlyAnimation(butterFly, endlessScoreProgress.transform, true, (_) =>
            {
                AudioMgr.Instance.PlayAudioOneShot(AudioName.ButterflyHitAudio);
                TryDoAddScoreAnimation(true);
            });
        }

        private void TryDoAddScoreAnimation(bool heart)
        {
            if (_addScore <= 0) return;

            endlessScoreProgress.AddScore(_addScore, heart, () =>
            {
                OnScoreChangeCallBack?.Invoke(endlessScoreProgress.currentScore);
            });
            _addScore = 0;
        }

        protected override bool IsGameSuccess()
        {
            return false;
        }

        private int GetAdditionScore(int rowOrColCount)
        {
            int addScore = 0;
            var addScoreDict = ConfigMgr.Instance.Tables.GlobalConfigCategory.GardenEndlessRowColAddScore;
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

            //基础分数
            var oneFlowerScore = ConfigMgr.Instance.Tables.GlobalConfigCategory.GardenEndlessOneGridScore;
            addScore += result.putInMapFlowers.Count * oneFlowerScore;

            if (result.HasRowColRemove)
            {
                //消除行列额外加分
                var additionAddScore = GetAdditionScore(result.rowList.Count + result.colList.Count);
                addScore += additionAddScore;

                //连击加分
                addScore += _currentComboCount * ConfigMgr.Instance.Tables.GlobalConfigCategory.GardenEndlessComboBaseScore;
            }

            return addScore;
        }

        protected override void OnPlaceToMapAfter()
        {
            base.OnPlaceToMapAfter();


        }
    }
}