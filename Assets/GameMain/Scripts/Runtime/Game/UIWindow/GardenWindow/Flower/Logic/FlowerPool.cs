using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.Garden
{
    public class FlowerPool : ILogicBase
    {
        [LabelText("游戏入口")]
        public GardenGameRoot flowerRoot;

        [LabelText("鲜花的Pool")]
        public MultiEntityPool<Garden_FlowerSingle> multiPools;

        [LabelText("可防止鲜花格子的阴影")]
        public EntityPool<FlowerMask> flowerMaskPool;

        [LabelText("蝴蝶的Pool")]
        public EntityPool<Butterfly> buffterFly;

        [LabelText("鲜花爆炸的特效Pool")]
        public ObjPool flowerBloomPool;

        [LabelText("旋转的特效Pool")]
        public ObjPool rotateEffectPool;

        [LabelText("蝴蝶的飞行特效Pool")]
        public ObjPool butterFlyEffectPool;

        [LabelText("鲜花格子阴影预制体")]
        public GameObject flowerMaskPrefab;

        [LabelText("鲜花爆炸的预制体")]
        public GameObject bloomFlowerEffectPrefab;

        [LabelText("蝴蝶的预制体")]
        public GameObject buffterflyPrefab;

        [LabelText("旋转的特效预制体")]
        public GameObject rotateEffectPrefab;

        [LabelText("蝴蝶飞行的特效")]
        public GameObject butterflyEffectPrefab;

        public Transform flowerPoolRoot;

        public Transform flowerMaskPoolRoot;

        public Transform flowerBloomPoolRoot;

        public Transform butterflyPoolRoot;

        public Transform rotateEffectPoolRoot;

        public Transform butterflyEffectRoot;

        public override void Initialize()
        {
            multiPools = new MultiEntityPool<Garden_FlowerSingle>(flowerPoolRoot);
            flowerMaskPool = new EntityPool<FlowerMask>(flowerMaskPrefab, flowerMaskPoolRoot);
            flowerBloomPool = new(bloomFlowerEffectPrefab, flowerBloomPoolRoot);
            buffterFly = new(buffterflyPrefab, butterflyPoolRoot);
            rotateEffectPool = new(rotateEffectPrefab, rotateEffectPoolRoot);
            butterFlyEffectPool = new(butterflyEffectPrefab, butterflyEffectRoot);
        }

        public void DisableAllEntities()
        {
            multiPools.DisableAllEntities();
            flowerMaskPool.DisableAllEntities();
            flowerBloomPool.DisableAllEntities();
            buffterFly.DisableAllEntities();
            rotateEffectPool.DisableAllEntities();
            butterFlyEffectPool.DisableAllEntities();
        }

        public void PushToFlowerPool(Garden_FlowerSingle flower)
        {
            multiPools.PushToPool(flower);
        }

        public void PushToFlowerMaskPool(FlowerMask flowerMask)
        {
            flowerMaskPool.PushToPool(flowerMask);
        }

        public void PushBloomEffectToPool(GameObject bloomEffect)
        {
            flowerBloomPool.PushToPool(bloomEffect);
        }

        public override void UnInitialize()
        {
            multiPools.DestroyAllEntity();
            flowerMaskPool.DestroyAllEntity();
            flowerBloomPool.DestroyAllEntity();
            buffterFly.DestroyAllEntity();
            rotateEffectPool.DestroyAllEntity();
            butterFlyEffectPool.DestroyAllEntity();
        }
    }
}