using cfg;
using Sirenix.OdinInspector;
using Spine;
using System;

namespace GameMain.Garden
{
    public partial class Garden_FlowerSingle : ABaseFlowerSingle
    {
        public enum EFlowerType
        {
            [LabelText("普通鲜花")]
            NormalFlower = 1,
            [LabelText("气泡蝴蝶")]
            BubbleButterFly = 2,
            [LabelText("特殊类型")]
            SpecailFlower = 3,
            [LabelText("冰蝴蝶")]
            IceButterfly,
        }

        public GardenFlowerTypeConfig FlowerTypeConfig { get; private set; }

        public GardenFlowerLevelConfig LevelConfig { get; private set; }

        public int AddScore => LevelConfig.GardenAddScore;

        public EFlowerType FlowerType => (EFlowerType)FlowerTypeConfig.FlowerType;

        public int FlowerConfigId => FlowerTypeConfig.Id;

        public void Initialize_Flower(GardenFlowerTypeConfig config, bool isAnimation)
        {
            FlowerTypeConfig = config;

            //默认的等级1的花苞
            LevelConfig = ConfigMgr.Instance.Tables.GardenFlowerLevelCategory.Get(FlowerTypeConfig.Id, 1);

            switch (FlowerType)
            {
                case EFlowerType.NormalFlower:

                    Flower_DefaultAnimation(isAnimation);
                    break;

                case EFlowerType.BubbleButterFly:

                    Initialize_BuffterflyInternal(isAnimation);
                    break;

                case EFlowerType.IceButterfly:

                    Initialize_IceInternal();
                    break;
                default:
                    break;
            }

            SetGray(false);
        }

        private void Flower_DefaultAnimation(bool isAnimation)
        {
            if (isAnimation)
            {
                flowerSpine.AnimationState.SetAnimation(0, LevelConfig.DefaultSpineDynamic, false);
            }
            else
            {
                flowerSpine.AnimationState.SetAnimation(0, LevelConfig.DefaultSpineStatic, false);
            }
        }

        private void Initialize_BuffterflyInternal(bool isAnimation)
        {
            Butterfly_DefaultAnimation(isAnimation);
        }

        private void SetEmptyAnimation()
        {
            flowerSpine.AnimationState.SetEmptyAnimation(0, 0);
        }

        public void Flower_PlayDefaultAnimation()
        {
            switch (FlowerType)
            {
                case EFlowerType.NormalFlower:

                    Flower_DefaultAnimation(true);

                    break;
                case EFlowerType.BubbleButterFly:

                    Butterfly_DefaultAnimation(true);

                    break;
                case EFlowerType.IceButterfly:
                    break;
                default:
                    Debugger.LogError($"未处理的鲜花出生类型:{FlowerType}");
                    break;
            }

        }

        public void Flower_PlayDragLoop()
        {
            flowerSpine.AnimationState.SetAnimation(0, LevelConfig.DefaultSpineDynamic, false);

            flowerSpine.AnimationState.GetCurrent(0).Complete -= OnPutInMapComplete;
            flowerSpine.AnimationState.GetCurrent(0).Complete += OnPutInMapComplete;
        }

        private void OnPutInMapComplete(TrackEntry trackEntry)
        {
            flowerSpine.AnimationState.GetCurrent(0).Complete -= OnPutInMapComplete;
            flowerSpine.AnimationState.SetAnimation(0, LevelConfig.SpineDragLoop, true);
        }

        public void Flower_StopDragLoop()
        {
            flowerSpine.AnimationState.GetCurrent(0).Complete -= OnPutInMapComplete;
            flowerSpine.AnimationState.SetAnimation(0, LevelConfig.DefaultSpineStatic, false);
        }

        private void Flower_PutInMap(bool isAnimation)
        {
            //随机等级
            if (FlowerTypeConfig.MaxLevel > 1)
            {
                var randomLevel = UtilityTool.RandomNumber(FlowerTypeConfig.MinLevel, FlowerTypeConfig.MaxLevel + 1);
                LevelConfig = ConfigMgr.Instance.Tables.GardenFlowerLevelCategory.Get(FlowerTypeConfig.Id, randomLevel);
            }

            Flower_DefaultAnimation(isAnimation);
        }

        public void Flower_PutInMapRadndomLevel(bool isAnimation)
        {
            switch (FlowerType)
            {
                case EFlowerType.NormalFlower:

                    Flower_PutInMap(isAnimation);

                    break;
                case EFlowerType.BubbleButterFly:

                    //蝴蝶
                    Butterfly_PutInMap(isAnimation);

                    break;
                case EFlowerType.IceButterfly:

                    break;
                default:
                    Debugger.LogError($"未处理的鲜花开花类型:{FlowerType}");
                    break;
            }
        }

        public void Flower_PlayBloom(Action complete)
        {
            if (FlowerType != EFlowerType.NormalFlower)
            {
                Debugger.LogError($"鲜花类型={FlowerType}");
                return;
            }

            transform.SetAsLastSibling();
            flowerSpine.AnimationState.SetAnimation(0, LevelConfig.SpineBloom, false);

            var duration = flowerSpine.AnimationState.GetCurrent(0).Animation.Duration;
            StartCoroutine(DelayTime(duration, complete));
        }
    }
}