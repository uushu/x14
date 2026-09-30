using GameMain.FlowerLine;
using GameMain.Garden;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameMain
{
    public class FlowerLinePool : ILogicBase
    {
        [LabelText("鲜花的Pool")]
        public MultiEntityPool<Line_FlowerSingle> multiPools;

        [LabelText("格子高亮Pool")]
        public ObjPool gridLightPool;

        [LabelText("蝴蝶的Pool")]
        public EntityPool<Butterfly> buffterFly;

        [LabelText("鲜花爆炸的特效Pool")]
        public ObjPool flowerBloomPool;

        [LabelText("高亮的格子")]
        public GameObject highLightPrefab;

        [LabelText("鲜花爆炸的预制体")]
        public GameObject bloomFlowerEffectPrefab;

        [LabelText("蝴蝶的预制体")]
        public GameObject buffterflyPrefab;

        public Transform flowerPoolRoot;

        public Transform gridLightPoolRoot;

        public Transform flowerBloomPoolRoot;

        public Transform butterflyPoolRoot;

        public override void Initialize()
        {
            multiPools = new MultiEntityPool<Line_FlowerSingle>(flowerPoolRoot);
            gridLightPool = new ObjPool(highLightPrefab, gridLightPoolRoot);
            flowerBloomPool = new(bloomFlowerEffectPrefab, flowerBloomPoolRoot);
            buffterFly = new(buffterflyPrefab, butterflyPoolRoot);
        }

        public void DisableAllEntities()
        {
            multiPools.DisableAllEntities();
            gridLightPool.DisableAllEntities();
            flowerBloomPool.DisableAllEntities();
            buffterFly.DisableAllEntities();
        }

        public void PushToFlowerPool(Line_FlowerSingle flower)
        {
            multiPools.PushToPool(flower);
        }

        public void PushToGridLightPool(GameObject gridLight)
        {
            gridLightPool.PushToPool(gridLight);
        }

        public void PushBloomEffectToPool(GameObject bloomEffect)
        {
            flowerBloomPool.PushToPool(bloomEffect);
        }

        public override void UnInitialize()
        {
            multiPools.DestroyAllEntity();
            gridLightPool.DestroyAllEntity();
            flowerBloomPool.DestroyAllEntity();
            buffterFly.DestroyAllEntity();
        }
    }
}