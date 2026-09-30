using cfg;
using Cysharp.Threading.Tasks;
using GameMain.Garden;
using Luban;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameMain.FlowerBlock
{
    public class ItemUI_FlowerBlock : ARecoverable, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [LabelText("格子类型")]
        public BlockGridTypeId gridTypeId;

        [LabelText("花的主类型")]
        public BlockFlowerId mainFlowerId;

        [LabelText("格子鲜花类型")]
        public List<int> flowersId;

        [LabelText("自定义格子类型")]
        public UICustomGrids customGrids;

        [LabelText("鲜花")]
        public List<Block_FlowerSingle> flowers;

        [LabelText("滑块的背景")]
        public Image imgBlockBg;

        private FlowerBlockMap _map;

        private Vector2 _downOffset;
        private RectTransform _parent;

        public BlockGridTypeConfig BlockGridTypeConfig { get; private set; }

        public BlockFlowerTypeConfig FlowerTypeConfig { get; private set; }

        public RectTransform SelfRect { get; private set; }

        public bool IsSpecicalBlockItem => FlowerTypeConfig.FlowerType == 3;

        public void DestroyAllChildren()
        {
            gameObject.DestroyAllChildren();
        }

        public async UniTask Initialize(FlowerBlockMap map, int gridTypeId, int mainFlowerId, List<int> flowersId)
        {
            this.gridTypeId = gridTypeId;
            this.mainFlowerId = mainFlowerId;
            this.flowersId = flowersId;
            imgBlockBg.enabled = true;

            _map = map;
            SelfRect = GetComponent<RectTransform>();
            _parent = transform.parent.GetComponent<RectTransform>();

            BlockGridTypeConfig = ConfigMgr.Instance.Tables.BlockGridTypeCategory.GetOrDefault(gridTypeId);
            FlowerTypeConfig = ConfigMgr.Instance.Tables.BlockFlowerTypeCategory.GetOrDefault(mainFlowerId);
            await GeneratorBlock(flowersId);

            //刷新滑块的背景
            ResourceMgr.Instance.SetImageSprite(imgBlockBg, FlowerTypeConfig.GameBlockItemBg, false);
        }

        private bool HasFlowerId(int flowerId)
        {
            return flowersId.Contains(flowerId);
        }

        public bool IsCanChangeToFlowerId(int butterflyId)
        {
            if (HasFlowerId(butterflyId)) return false;
            if (flowersId.Count <= 1) return false;
            if(IsSpecicalBlockItem) return false;
            return true;
        }

        //随机一个格子换成蝴蝶
        public void RandomButterflyId(int butterflyId)
        {
            var randomIndex = UtilityTool.RandomNumber(0, flowersId.Count);
            flowersId[randomIndex] = butterflyId;

            BlockGameRoot.Instance.pool.PushFlowerToPool(this);
            GeneratorBlock(flowersId).Forget();
        }

        private async UniTask GeneratorBlock(List<int> flowersId)
        {
            flowers.Clear();

            await customGrids.GeneratorGrids(BlockGridTypeConfig.GridData, 1, BlockGridTypeConfig.GridData.Count, flowersId, async (flowerId) =>
            {
                var  flower = await GetFlowerFromPool(flowerId);
                flowers.Add(flower);
                return flower.gameObject;
            });

            SelfRect.sizeDelta = new Vector2(customGrids.Width, customGrids.Heigth);
        }

        public async UniTask<Block_FlowerSingle> GetFlowerFromPool(int flowerId)
        {
            var flowerConfig = ConfigMgr.Instance.Tables.BlockFlowerTypeCategory.Get(flowerId);

            var gridInstance = await BlockGameRoot.Instance.pool.multiPools.GetEntityAsync(flowerConfig.BlockSpinePrefab);
            var flower = gridInstance.GetComponent<Block_FlowerSingle>();
            flower.Initialize_Flower(flowerConfig, false);
            return flower;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (BlockGameRoot.Instance.IsForbidenInput) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parent, eventData.position, UIWindowMgr.Instance.uiCamera, out var localPoint);
            //限制Y方向的移动
            transform.localPosition = new Vector3(localPoint.x + _downOffset.x, transform.localPosition.y, 0);
            _map.OnDrag(this, eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (BlockGameRoot.Instance.IsForbidenInput) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parent, eventData.position, UIWindowMgr.Instance.uiCamera, out var localPoint);
            _downOffset = new Vector2(transform.localPosition.x, transform.localPosition.y) - localPoint;

            _map.OnPointerDown(this,eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (BlockGameRoot.Instance.IsForbidenInput) return;

            _map.OnPointerUp(this, eventData);
        }
        
        public void Flower_PlayMove(EFlowerBlockMoveDirection direction)
        {
            foreach (var flower in flowers)
            {
                flower.Flower_PlayMove(direction);
            }
        }
      
#if UNITY_EDITOR
        public const string flowerFloder = "Assets/GameMain/AssetBundle/Prefab/FlowerPrefab/Flower.Block";

        [Button("生成格子")]
        private void GeneratorGrid()
        {
            var bytes = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameMain/AssetBundle/LubanData/Bytes/BlockGridTypeCategory.bytes");
            var category = new BlockGridTypeCategory(new ByteBuf(bytes.bytes));
            var gridTypeConfig = category.GetOrDefault(gridTypeId);

            var bytes2 = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameMain/AssetBundle/LubanData/Bytes/BlockFlowerTypeCategory.bytes");
            var category2 = new BlockFlowerTypeCategory(new ByteBuf(bytes2.bytes));
            var flowerConfig = category2.GetOrDefault(mainFlowerId);

            var bytes3 = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameMain/AssetBundle/LubanData/Bytes/GardenFlowerLevelCategory.bytes");
            var category3 = new GardenFlowerLevelCategory(new ByteBuf(bytes3.bytes));

            var prefab = AssetLoader.LoadGameObject(flowerFloder, flowerConfig.BlockSpinePrefab);

            customGrids.rowCount = 1;
            customGrids.colCount = gridTypeConfig.GridData.Count;
            customGrids.gridItemPrefab = prefab;
            customGrids.GeneratorGrids(null, null);

            imgBlockBg.sprite = AssetLoader.LoadSprite("Assets/GameMain/AssetBundle/Sprite/BlockWindow/BlockBg", flowerConfig.GameBlockItemBg);

            GetComponent<RectTransform>().sizeDelta = customGrids.GetComponent<RectTransform>().sizeDelta;
        }
#endif
    }
}