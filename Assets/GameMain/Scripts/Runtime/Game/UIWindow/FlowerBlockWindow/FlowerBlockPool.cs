using Sirenix.OdinInspector;
using UnityEngine;

namespace GameMain.FlowerBlock
{
    public class FlowerBlockPool : ILogicBase
    {
        [LabelText("鲜花池子根节点")]
        public Transform flowerPoolRoot;

        [LabelText("鲜花格子类型根节点")]
        public Transform gridTypePoolRoot;

        [LabelText("蝴蝶飞行的特效")]
        public Transform butterflyEffectRoot;

        [LabelText("蝴蝶撞击特效")]
        public Transform rotateEffectPoolRoot;

        [LabelText("鲜花类型预制体")]
        public ItemUI_FlowerBlock gridTypePrefab;

        [LabelText("蝴蝶飞行的特效")]
        public GameObject butterflyEffectPrefab;

        [LabelText("旋转的特效预制体")]
        public GameObject rotateEffectPrefab;

        //格子类型池子
        public EntityPool<ItemUI_FlowerBlock> gridTypePool;

        //鲜花池子
        public MultiEntityPool<Block_FlowerSingle> multiPools;

        [LabelText("蝴蝶的飞行特效Pool")]
        public ObjPool butterFlyEffectPool;

        [LabelText("旋转的特效Pool")]
        public ObjPool rotateEffectPool;

        [LabelText("使用道具-击碎动画")]
        public GameObject crushPrefab;

        [LabelText("使用道具-刷新动画")]
        public GameObject refreshPrefab;

        [LabelText("使用道具-移除动画")]
        public GameObject removePrefab;

        public override void Initialize()
        {
            base.Initialize();

            gridTypePrefab.DestroyAllChildren();
            gridTypePool = new(gridTypePrefab.gameObject, gridTypePoolRoot);
            multiPools = new(flowerPoolRoot);
            butterFlyEffectPool = new(butterflyEffectPrefab, butterflyEffectRoot);
            rotateEffectPool = new(rotateEffectPrefab, rotateEffectPoolRoot);
        }

        public void PushGridTypeToPool(ItemUI_FlowerBlock blockItem)
        {
            //格子里面的花儿放入池子
            PushFlowerToPool(blockItem);

            //格子类型放入池子
            gridTypePool.PushToPool(blockItem);
        }

        public void PushFlowerToPool(ItemUI_FlowerBlock blockItem)
        {
            //格子里面的花儿放入池子
            foreach (var flower in blockItem.flowers)
            {
                multiPools.PushToPool(flower);
            }
            blockItem.flowers.Clear();
        }

        public ItemUI_FlowerBlock GetBlockFromPool()
        {
            return gridTypePool.GetFromPoolSync().Item1;
        }

        public void DisableAllEntities()
        {
            gridTypePool.DisableAllEntities();
            multiPools.DisableAllEntities();
            butterFlyEffectPool.DisableAllEntities();
            rotateEffectPool.DisableAllEntities();
        }

        public override void UnInitialize()
        {
            base.UnInitialize();

            gridTypePool.DestroyAllEntity();
            multiPools.DestroyAllEntity();
            butterFlyEffectPool.DestroyAllEntity();
            rotateEffectPool.DestroyAllEntity();
        }
    }
}