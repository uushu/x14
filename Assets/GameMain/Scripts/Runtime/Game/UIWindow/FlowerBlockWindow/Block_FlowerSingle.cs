using cfg;
using Sirenix.OdinInspector;
using Spine;
using System;
using UnityEngine;

namespace GameMain.FlowerBlock
{
    public enum EFlowerBlockMoveDirection
    {
        Left,
        Right,
        Down,
        Up,
    }

    public partial class Block_FlowerSingle : ABaseFlowerSingle
    {
        public enum EFlowerType
        {
            [LabelText("普通鲜花")]
            NormalFlower = 1,
            [LabelText("气泡蝴蝶")]
            BubbleButterFly = 2,
            [LabelText("特殊类型")]
            SpecailFlower = 3,
        }

        public EFlowerType FlowerType => (EFlowerType)Config.FlowerType;

        public BlockFlowerTypeConfig Config { get; private set; }

        public int FlowerConfigId => Config.Id;

        public Canvas canvas;

        public void Initialize_Flower(BlockFlowerTypeConfig config, bool animation)
        {
            Config = config;
            canvas.sortingOrder = 161;

            Flower_PlayDefaultStatic();
        }

        private void Flower_PlayDefaultStatic()
        {
            switch (FlowerType)
            {
                case EFlowerType.SpecailFlower:
                case EFlowerType.NormalFlower:
                    flowerSpine.AnimationState.SetAnimation(0, "1_idle1", false);
                    break;
                case EFlowerType.BubbleButterFly:
                    flowerSpine.AnimationState.SetAnimation(0, "1_idle2", true);
                    break;
                default:
                    break;
            }
        }

        public void Flower_PlayBloom(Action complete)
        {
            canvas.sortingOrder = 162;

            flowerSpine.AnimationState.GetCurrent(0).Complete -= OnAnimationComplete;
            flowerSpine.AnimationState.SetAnimation(0, "2_born", false);

            var duration = flowerSpine.AnimationState.GetCurrent(0).Animation.Duration;
            StartCoroutine(DelayTime(duration, complete));
        }

        public void Flower_PlayMove(EFlowerBlockMoveDirection direction)
        {
            if (DirectionIsPlaying(direction)) return;

            switch (direction)
            {
                case EFlowerBlockMoveDirection.Left:

                    flowerSpine.AnimationState.SetAnimation(0, "1_left", false);
                    break;
                case EFlowerBlockMoveDirection.Right:
                    flowerSpine.AnimationState.SetAnimation(0, "1_right", false);
                    break;
                case EFlowerBlockMoveDirection.Down:
                    flowerSpine.AnimationState.SetAnimation(0, "1_down", false);
                    break;
                case EFlowerBlockMoveDirection.Up:
                    flowerSpine.AnimationState.SetAnimation(0, "1_up", false);
                    break;
                default:
                    Debugger.LogError($"未处理的方向:{direction}");
                    break;
            }

            flowerSpine.AnimationState.GetCurrent(0).Complete -= OnAnimationComplete;
            flowerSpine.AnimationState.GetCurrent(0).Complete += OnAnimationComplete;
        }

        private void OnAnimationComplete(TrackEntry trackEntry)
        {
            flowerSpine.AnimationState.GetCurrent(0).Complete -= OnAnimationComplete;
            Flower_PlayDefaultStatic();
        }

        private bool DirectionIsPlaying(EFlowerBlockMoveDirection direction)
        {
            var spineAnimation = flowerSpine.AnimationState.GetCurrent(0);
            if (spineAnimation.IsComplete) return false;

            switch (direction)
            {
                case EFlowerBlockMoveDirection.Left:
                    return spineAnimation.Animation.Name == "1_left";
                case EFlowerBlockMoveDirection.Right:
                    return spineAnimation.Animation.Name == "1_right";
                case EFlowerBlockMoveDirection.Down:
                    return spineAnimation.Animation.Name == "1_down";
                case EFlowerBlockMoveDirection.Up:
                    return spineAnimation.Animation.Name == "1_up";
                default:
                    return false;
            }
        }

#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();

            canvas = flowerSpine.transform.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = flowerSpine.gameObject.AddComponent<Canvas>();
            }
            canvas.overrideSorting = true;
            canvas.sortingOrder = 161;
        }
#endif
    }
}