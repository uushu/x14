using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameMain.FlowerBlock
{
    public partial class FlowerBlockMap
    {    
        public void Initialize_Generator()
        {
        }

        public void UnInitialize_Generator()
        {
        }

        public void OnStartGame(int levelId)
        {
            inMapBlockItems.Clear();
            HideDragImage();
            GeneratorInitMap();
        }

        private async void GeneratorInitMap()
        {
            //生成等待区滑块
            BlockGameRoot.Instance.waitBlock.ReadyNextRefreshBlocks();

            var rowCount = uicustomGrids.rowCount;

            //默认初始行数
            int initRowCount = 1;

            BlockGameRoot.Instance.StartAnimation();

            //前面的行数默认显示在地图上
            for (int i = rowCount - initRowCount; i < rowCount; i++)
            {
                await GeneratorByRule(i);
            }

            //更新滑块的数据
            TryEliminateBlock(true, () =>
            {
                BlockGameRoot.Instance.StopAnimation();
            });
        }

        public async UniTask GeneratorByRule(int rowIndex)
        {
            //根据配置生成格子
            var result = BlockGameRoot.Instance.waitBlock.GetRefresh();

            //生成等待区的格子
            for (int i = 0; i < result.Count; i++)
            {
                await CreateBlockItem(result[i], rowIndex);
            }

            //生成等待区滑块
            BlockGameRoot.Instance.waitBlock.ReadyNextRefreshBlocks();
        }

        private async UniTask CreateBlockItem(RefreshBlockItemData refresh, int rowIndex)
        {
            var startGridX = refresh.startGridX;
            var headPoint = BlockGameRoot.Instance.blockMap.uicustomGrids.GridXYToLocalPosition(new Vector2Int(startGridX, rowIndex));

            var blockFlower = BlockGameRoot.Instance.pool.GetBlockFromPool();

            blockFlower.transform.SetParent(flowerRoot);
            blockFlower.transform.localScale = Vector3.one;

            await blockFlower.Initialize(this, refresh.blockTypeId, refresh.mainFlowerId, refresh.flowersId);
            var xOffset = (refresh.Config.GridData.Count - 1) * 0.5f * uicustomGrids.gridWidth;
            blockFlower.transform.localPosition = new Vector2(headPoint.x + xOffset, headPoint.y);
            blockFlower.name = $"BlockFlower-{refresh.waveIndex}-{startGridX}";

            AddBlockItem(blockFlower);
        }
    }
}