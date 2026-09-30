using System;
using System.Collections;
using System.Collections.Generic;

namespace GameMain.FlowerBlock
{
    public partial class Block_FlowerSingle
    {
        public void Butterfly_BubbleBreak(Action breakComplete)
        {
            if (FlowerType != EFlowerType.BubbleButterFly)
            {
                return;
            }

            flowerSpine.AnimationState.SetAnimation(0, "2_born", false);

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

            flowerSpine.AnimationState.SetAnimation(0, "1_run", true);
        }
    }
}