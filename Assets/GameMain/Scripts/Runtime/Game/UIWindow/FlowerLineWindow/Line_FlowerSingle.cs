using cfg;
using DG.Tweening;
using System;
using UnityEngine;

namespace GameMain.FlowerLine
{
    public class Line_FlowerSingle : ABaseFlowerSingle
    {
        public static Vector2Int InValidGridXY => new Vector2Int(int.MinValue, int.MinValue);

        [HideInInspector]
        public Vector2Int currentGridXY;

        public LineFlowerTypeConfig FlowerTypeConfig { get; private set; }

        public int FlowerConfigId => FlowerTypeConfig.Id;

        public void Initialize_Flower(LineFlowerTypeConfig config, bool animation)
        {
            FlowerTypeConfig = config;

            PlayIdleAnimation();
        }

        private void PlayIdleAnimation()
        {
            flowerSpine.AnimationState.SetAnimation(0, "1_idle", false);
        }

        public void DoMoveToNextGrid(Vector2Int nextGrid, Vector3 localPoint, float moveTime)
        {
            currentGridXY = nextGrid;
            transform.DOLocalMove(localPoint, moveTime).SetEase(Ease.Linear);
        }

        public void PlayBloomAnimation(Action complete)
        {
            transform.SetAsLastSibling();
            flowerSpine.AnimationState.SetAnimation(0, "0_born", false);
            StartCoroutine(DelayTime(flowerSpine.AnimationState.GetCurrent(0).Animation.Duration, complete));
        }
    }
}