using Cysharp.Threading.Tasks;
using GameMain.BTree;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.Garden
{
    public class GardenDragToFixGrid : ABaseActionNode
    {
        public int OnlyCanDragIndex;
        public List<Vector2Int> GridsXY;

        public GardenDragToFixGrid(int onlyCanDragIndex, List<Vector2Int> gridsXY)
        {
            OnlyCanDragIndex = onlyCanDragIndex;
            GridsXY = gridsXY;
        }

        private void SetForbiden()
        {
            var waitAreas = GardenGameRoot.Instance.waitingArea.waitingFlowerShape;

            for (int i = 0; i < waitAreas.Count; i++)
            {
                var isForbidenDrag = i != OnlyCanDragIndex;

                var waitArea = waitAreas[i];
                waitArea.ForbidenDrag = isForbidenDrag;
            }
        }
        private Vector3 CalcAvgPostion()
        {
            var sunPosition = Vector3.zero;
            foreach (var gridXY in GridsXY)
            {
                var gridChild = GardenGameRoot.Instance.flowerGridsMap.customGrids.GetChildByIndex(gridXY);

                if (gridChild == null)
                {
                    Debugger.LogError($"不存在的子节点 gridXY:{gridXY}");
                    continue;
                }

                sunPosition += gridChild.transform.position;
            }
            var avgPositoon = sunPosition / GridsXY.Count;
            return avgPositoon;
        }

        private void UnFreezeDragForbiden()
        {
            if (GardenGameRoot.Instance.currentStatus == EStatus.Success) return;

            var waitAreas = GardenGameRoot.Instance.waitingArea.waitingFlowerShape;

            for (int i = 0; i < waitAreas.Count; i++)
            {
                var waitArea = waitAreas[i];
                waitArea.ForbidenDrag = false;
            }
        }

        protected override async void OnStart()
        {
            SetForbiden();

            var guideWindow = await UIWindowMgr.Instance.OpenWindowAsync<GuideWindow>();

            var targetPosition = CalcAvgPostion();
            var waitAreas = GardenGameRoot.Instance.waitingArea.waitingFlowerShape;
            var waitAreaShape = waitAreas[OnlyCanDragIndex];

            //引导目标的宽高
            var rowCount = waitAreaShape.GridTypeConfig.Row;
            var colCount = waitAreaShape.GridTypeConfig.Col;
            var gridWidth = GardenGameRoot.Instance.flowerGridsMap.customGrids.gridWidth + 5f;
            var gridHeight = GardenGameRoot.Instance.flowerGridsMap.customGrids.gridHeight + 10f;
            var sizeDelta = new Vector2(gridWidth * colCount, gridHeight * rowCount);

            guideWindow.DoFingerDragAnimation(waitAreaShape.transform.position, targetPosition, sizeDelta);

            GardenGameRoot.Instance.flowerGridsMap.SetConstraintDragToGrids(GridsXY);

            GardenGameRoot.Instance.flowerGridsMap.OnPlaceToMapAfterCallBack += OnPlaceToMapAfter;
            GardenGameRoot.Instance.flowerGridsMap.OnPlaceToMapBeforeCallBack += OnPlaceToMapBefore;
        }

        private void OnPlaceToMapAfter()
        {
            GardenGameRoot.Instance.flowerGridsMap.OnPlaceToMapAfterCallBack -= OnPlaceToMapAfter;
            GardenGameRoot.Instance.flowerGridsMap.OnPlaceToMapBeforeCallBack -= OnPlaceToMapBefore;

            SetFinish(true);
        }

        private void OnPlaceToMapBefore(FlowerRemoveResult result)
        {
            CloseFingerDrag(false);
        }

        protected override void SetFinish(bool success)
        {
            UnFreezeDragForbiden();

            base.SetFinish(success);
        }

        private void CloseFingerDrag(bool unfreezeDrag)
        {
            var guideWindow = UIWindowMgr.Instance.GetWindow<GuideWindow>();
            guideWindow.StopFingerAnimation();
            UIWindowMgr.Instance.CloseWindow<GuideWindow>(false).Forget();

            if (unfreezeDrag)
            {
                UnFreezeDragForbiden();
            }
        }

        protected override void OnCancel()
        {
            CloseFingerDrag(true);
        }

    }
}