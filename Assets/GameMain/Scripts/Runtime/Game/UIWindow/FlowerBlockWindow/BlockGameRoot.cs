using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace GameMain.FlowerBlock
{
    public class BlockGameRoot : AGameBaseRoot<BlockGameRoot>
    {
        [LabelText("鲜花地图")]
        public FlowerBlockMap blockMap;

        [LabelText("池子")]
        public FlowerBlockPool pool;

        [LabelText("等待区格子")]
        public FlowerBlockWaitArea waitBlock;

        //生成的配置的数量
        private IBlockRefreshRule _refreshRule;
        public IBlockRefreshRule RefreshRule => _refreshRule;

        public Action OnBeforeDragFlowerBlockItemCallback { get; set; }
        public Action OnAfterDragFlowerBlockItemCallback { get; set; }

        public override EMiniGameType eMiniGameType => EMiniGameType.BlockFlower;

        protected override void OnEnter()
        {
            base.OnEnter();

            waitBlock.Initialize();
            blockMap.Initialize();
            pool.Initialize();
        }

        public override void StartGame(int levelId, params object[] parameters)
        {
            
            base.StartGame(levelId, parameters);

            pool.DisableAllEntities();

            _refreshRule = (IBlockRefreshRule)parameters[0];
            _refreshRule.Initialize(levelId);

            waitBlock.OnStartGame(levelId);
            blockMap.OnStartGame(levelId);
        }

        public void ResetRefreshWaveIndex()
        {
            RefreshRule.ResetWaveIndex();
        }

        protected override void OnExit()
        {
            base.OnExit();

            blockMap.UnInitialize();
            pool.UnInitialize();
            waitBlock.UnInitialize();
        }
    }
}