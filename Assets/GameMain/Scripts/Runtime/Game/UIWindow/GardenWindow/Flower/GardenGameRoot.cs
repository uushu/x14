using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.Garden
{
    public class GardenGameRoot : AGameBaseRoot<GardenGameRoot>
    {
        public override EMiniGameType eMiniGameType => EMiniGameType.GardenFlower;

        [LabelText("鲜花生成等待区")]
        public FlowerWaitingArea waitingArea;

        [LabelText("鲜花池子")]
        public FlowerPool flowerPool;

        [LabelText("鲜花地图网格")]
        public FlowerGridsMap flowerGridsMap;

        [LabelText("是否有花朵在拖动中")]
        public FlowerShape dragingFlower;

        private IGardenRefreshRule _refreshRule;
        public IGardenRefreshRule GardenRefreshRule => _refreshRule;

        protected override void OnEnter()
        {
            base.OnEnter();

            waitingArea.Initialize();
            flowerPool.Initialize();
            flowerGridsMap.Initialize();
        }

        protected override void OnExit()
        {
            base.OnExit();

            waitingArea.UnInitialize();
            flowerPool.UnInitialize();
            flowerGridsMap.UnInitialize();
        }

        public override void StartGame(int levelId, params object[] parameters)
        {
            _refreshRule = parameters[0] as IGardenRefreshRule;
            _refreshRule.Initialize(levelId);

            dragingFlower = null;
            base.StartGame(levelId);

            //清理地图数据
            flowerGridsMap.ClearMapData();

            //生成默认的地图上的格子
            flowerGridsMap.GeneratorInitGridsInMap();

            //等待区
            waitingArea.AutoRefreshWaitingShape(IGardenRefreshRule.LeastCountCanPutInMap);
        }

#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();

            waitingArea.flowerRoot = this;
            flowerPool.flowerRoot = this;
            flowerGridsMap.flowerRoot = this;
        }
#endif
    }
}