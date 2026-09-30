using System;
using System.Collections;
using System.Collections.Generic;

namespace GameMain.Garden
{
    public partial class Garden_FlowerSingle
    {
        public void Butterfly_BubbleBreak(Action breakComplete)
        {
            if (FlowerType != EFlowerType.BubbleButterFly)
            {
                return;
            }

            flowerSpine.AnimationState.SetAnimation(0, LevelConfig.SpineBloom, false);

            StartCoroutine(DelayTime(flowerSpine.AnimationState.GetCurrent(0).Animation.Duration, () =>
            {
                breakComplete?.Invoke();
            }));
        }

        public void Butterfly_Fly()
        {
            if (FlowerType != EFlowerType.BubbleButterFly)
            {
                return;
            }

            flowerSpine.AnimationState.SetAnimation(0, LevelConfig.SpineFly, true);
        }

        public void Butterfly_DefaultAnimation(bool isAnimation)
        {
            if (FlowerType != EFlowerType.BubbleButterFly)
            {
                return;
            }

            if (isAnimation)
            {
                flowerSpine.AnimationState.SetAnimation(0, LevelConfig.DefaultSpineDynamic, false);
            }
            else
            {
                flowerSpine.AnimationState.SetAnimation(0, LevelConfig.DefaultSpineStatic, false);
            }
        }

        public void Butterfly_PutInMap(bool isAnimation)
        {
            if (FlowerType != EFlowerType.BubbleButterFly)
            {
                return;
            }

            Butterfly_DefaultAnimation(isAnimation);
        }
    }
}